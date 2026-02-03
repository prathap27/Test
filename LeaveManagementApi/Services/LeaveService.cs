using LeaveManagementApi.Authorization;
using LeaveManagementApi.DTOs;
using LeaveManagementApi.Models;
using LeaveManagementApi.Repositories;
using Microsoft.Extensions.Options;

namespace LeaveManagementApi.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserStore _userStore;
    private readonly AuthSettings _authSettings;

    public LeaveService(
        ILeaveRepository leaveRepository,
        IApprovalRepository approvalRepository,
        IUnitOfWork unitOfWork,
        IUserStore userStore,
        IOptions<AuthSettings> authSettings)
    {
        _leaveRepository = leaveRepository;
        _approvalRepository = approvalRepository;
        _unitOfWork = unitOfWork;
        _userStore = userStore;
        _authSettings = authSettings.Value;
    }

    public async Task<LeaveRequestDto> CreateLeaveAsync(Guid employeeId, CreateLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        var request = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason
        };

        var approvals = BuildApprovalChain(request.Id);
        request.Approvals = approvals;

        await _leaveRepository.AddAsync(request, cancellationToken);
        await _approvalRepository.AddRangeAsync(approvals, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapLeave(request);
    }

    public async Task<List<LeaveRequestDto>> GetLeavesForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var leaves = await _leaveRepository.GetForEmployeeAsync(employeeId, cancellationToken);
        return leaves.Select(MapLeave).ToList();
    }

    public async Task<List<LeaveRequestDto>> GetPendingApprovalsAsync(Guid approverId, CancellationToken cancellationToken = default)
    {
        var approvals = await _approvalRepository.GetPendingForApproverAsync(approverId, cancellationToken);
        return approvals
            .Select(approval => approval.LeaveRequest)
            .Where(request => request is not null)
            .DistinctBy(request => request!.Id)
            .Select(request => MapLeave(request!))
            .ToList();
    }

    public async Task<LeaveRequestDto> DecideApprovalAsync(Guid approvalId, Guid approverId, DecideApprovalDto dto, CancellationToken cancellationToken = default)
    {
        var approval = await _approvalRepository.GetByIdAsync(approvalId, cancellationToken);
        if (approval is null)
        {
            throw new InvalidOperationException("Approval not found.");
        }

        if (approval.ApproverId != approverId)
        {
            throw new InvalidOperationException("Approver mismatch.");
        }

        if (!Enum.TryParse<ApprovalStatus>(dto.Decision, true, out var decision) || decision == ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Decision must be Approved or Rejected.");
        }

        approval.Status = decision;
        approval.Comment = dto.Comment;
        approval.DecisionAt = DateTime.UtcNow;

        var request = approval.LeaveRequest ?? await _leaveRepository.GetByIdAsync(approval.LeaveRequestId, cancellationToken);
        if (request is null)
        {
            throw new InvalidOperationException("Leave request not found.");
        }

        if (decision == ApprovalStatus.Rejected)
        {
            request.Status = LeaveStatus.Rejected;
        }
        else
        {
            var remaining = request.Approvals.Any(item => item.Status == ApprovalStatus.Pending);
            request.Status = remaining ? LeaveStatus.Pending : LeaveStatus.Approved;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapLeave(request);
    }

    private List<Approval> BuildApprovalChain(Guid leaveRequestId)
    {
        var approvals = new List<Approval>();

        for (var i = 0; i < _authSettings.ApprovalLevels.Count; i++)
        {
            var role = _authSettings.ApprovalLevels[i];
            var approver = _authSettings.Users.FirstOrDefault(user =>
                user.Role.Equals(role, StringComparison.OrdinalIgnoreCase));

            if (approver is null)
            {
                continue;
            }

            approvals.Add(new Approval
            {
                Id = Guid.NewGuid(),
                LeaveRequestId = leaveRequestId,
                ApproverId = approver.Id,
                Level = i + 1,
                Status = ApprovalStatus.Pending
            });
        }

        return approvals;
    }

    private static LeaveRequestDto MapLeave(LeaveRequest request)
    {
        return new LeaveRequestDto(
            request.Id,
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            request.Reason,
            request.Status.ToString(),
            request.CreatedAt,
            request.Approvals
                .OrderBy(approval => approval.Level)
                .Select(approval => new ApprovalDto(
                    approval.Id,
                    approval.ApproverId,
                    approval.Level,
                    approval.Status.ToString(),
                    approval.DecisionAt,
                    approval.Comment))
                .ToList());
    }
}

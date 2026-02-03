using LeaveManagementApi.DTOs;

namespace LeaveManagementApi.Services;

public interface ILeaveService
{
    Task<LeaveRequestDto> CreateLeaveAsync(Guid employeeId, CreateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<LeaveRequestDto>> GetLeavesForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<List<LeaveRequestDto>> GetPendingApprovalsAsync(Guid approverId, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> DecideApprovalAsync(Guid approvalId, Guid approverId, DecideApprovalDto dto, CancellationToken cancellationToken = default);
}

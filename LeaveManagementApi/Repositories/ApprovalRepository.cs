using LeaveManagementApi.Data;
using LeaveManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementApi.Repositories;

public class ApprovalRepository : IApprovalRepository
{
    private readonly LeaveManagementDbContext _context;

    public ApprovalRepository(LeaveManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Approval?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Approvals
            .Include(approval => approval.LeaveRequest)
            .FirstOrDefaultAsync(approval => approval.Id == id, cancellationToken);
    }

    public async Task<List<Approval>> GetPendingForApproverAsync(Guid approverId, CancellationToken cancellationToken = default)
    {
        return await _context.Approvals
            .Include(approval => approval.LeaveRequest)
            .Where(approval => approval.ApproverId == approverId && approval.Status == ApprovalStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Approval> approvals, CancellationToken cancellationToken = default)
    {
        await _context.Approvals.AddRangeAsync(approvals, cancellationToken);
    }
}

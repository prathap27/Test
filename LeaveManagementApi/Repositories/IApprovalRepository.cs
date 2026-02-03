using LeaveManagementApi.Models;

namespace LeaveManagementApi.Repositories;

public interface IApprovalRepository
{
    Task<Approval?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Approval>> GetPendingForApproverAsync(Guid approverId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Approval> approvals, CancellationToken cancellationToken = default);
}

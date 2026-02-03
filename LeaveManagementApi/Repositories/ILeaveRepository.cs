using LeaveManagementApi.Models;

namespace LeaveManagementApi.Repositories;

public interface ILeaveRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LeaveRequest>> GetForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveRequest request, CancellationToken cancellationToken = default);
}

using LeaveManagementApi.Data;
using LeaveManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementApi.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly LeaveManagementDbContext _context;

    public LeaveRepository(LeaveManagementDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .Include(request => request.Approvals)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
    }

    public async Task<List<LeaveRequest>> GetForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .Include(request => request.Approvals)
            .Where(request => request.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LeaveRequest request, CancellationToken cancellationToken = default)
    {
        await _context.LeaveRequests.AddAsync(request, cancellationToken);
    }
}

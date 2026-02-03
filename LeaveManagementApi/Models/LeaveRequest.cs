namespace LeaveManagementApi.Models;

public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Approval> Approvals { get; set; } = new();
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

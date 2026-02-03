namespace LeaveManagementApi.Models;

public class Approval
{
    public Guid Id { get; set; }
    public Guid LeaveRequestId { get; set; }
    public Guid ApproverId { get; set; }
    public int Level { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public DateTime? DecisionAt { get; set; }
    public string? Comment { get; set; }
    public LeaveRequest? LeaveRequest { get; set; }
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}

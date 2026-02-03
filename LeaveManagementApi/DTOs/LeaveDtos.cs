namespace LeaveManagementApi.DTOs;

public record CreateLeaveRequestDto(DateTime StartDate, DateTime EndDate, string Reason);

public record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string Reason,
    string Status,
    DateTime CreatedAt,
    List<ApprovalDto> Approvals);

public record ApprovalDto(
    Guid Id,
    Guid ApproverId,
    int Level,
    string Status,
    DateTime? DecisionAt,
    string? Comment);

public record DecideApprovalDto(string Decision, string? Comment);

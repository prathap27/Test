using System.Security.Claims;
using LeaveManagementApi.DTOs;
using LeaveManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementApi.Controllers;

[ApiController]
[Route("api/leaves")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveRequestsController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [Authorize(Roles = "Employee")]
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeave(CreateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        var employeeId = GetUserId();
        var result = await _leaveService.CreateLeaveAsync(employeeId, dto, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetMyLeaves(CancellationToken cancellationToken)
    {
        var employeeId = GetUserId();
        var result = await _leaveService.GetLeavesForEmployeeAsync(employeeId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager,HR")]
    [HttpGet("pending-approvals")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetPendingApprovals(CancellationToken cancellationToken)
    {
        var approverId = GetUserId();
        var result = await _leaveService.GetPendingApprovalsAsync(approverId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager,HR")]
    [HttpPost("approvals/{approvalId:guid}/decision")]
    public async Task<ActionResult<LeaveRequestDto>> DecideApproval(Guid approvalId, DecideApprovalDto dto, CancellationToken cancellationToken)
    {
        var approverId = GetUserId();
        var result = await _leaveService.DecideApprovalAsync(approvalId, approverId, dto, cancellationToken);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }
}

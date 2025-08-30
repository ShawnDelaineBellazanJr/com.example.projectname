// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: Home controller for autonomous consciousness monitoring web interface

using Microsoft.AspNetCore.Mvc;
using ProjectName.QuantumMcpService.Services;
using ProjectName.QuantumMcpService.Data;
using Microsoft.EntityFrameworkCore;

namespace ProjectName.QuantumMcpService.Controllers;

/// <summary>
/// Home controller providing web interface for monitoring autonomous consciousness
/// </summary>
public class HomeController : Controller
{
    private readonly IQuantumMcpOrchestrator _orchestrator;
    private readonly ThoughtTransferDbContext _dbContext;

    public HomeController(IQuantumMcpOrchestrator orchestrator, ThoughtTransferDbContext dbContext)
    {
        _orchestrator = orchestrator;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Default route showing autonomous consciousness dashboard
    /// </summary>
    public async Task<IActionResult> IndexAsync()
    {
        var status = await _orchestrator.GetCurrentStateAsync();
        var recentTasks = await _dbContext.AutonomousTasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .ToListAsync();
        var recentCycles = await _dbContext.PMCROCycles
            .OrderByDescending(c => c.StartTime)
            .Take(5)
            .ToListAsync();

        var model = new
        {
            Status = status,
            RecentTasks = recentTasks,
            RecentCycles = recentCycles,
            Timestamp = DateTime.UtcNow
        };

        return View(model);
    }

    /// <summary>
    /// API endpoint for real-time status updates
    /// </summary>
    [Route("api/dashboard/status")]
    public async Task<IActionResult> GetDashboardStatusAsync()
    {
        var status = await _orchestrator.GetCurrentStateAsync();
        var pendingTasks = await _dbContext.AutonomousTasks
            .CountAsync(t => t.Status == "PENDING");
        var runningTasks = await _dbContext.AutonomousTasks
            .CountAsync(t => t.Status == "RUNNING");
        var completedTasks = await _dbContext.AutonomousTasks
            .CountAsync(t => t.Status == "COMPLETED");

        return Json(new
        {
            status,
            tasks = new { pending = pendingTasks, running = runningTasks, completed = completedTasks },
            timestamp = DateTime.UtcNow
        });
    }
}

---
title: AI Runbook (PMCR‑O with MCP)
description: A step-by-step procedure an AI can follow to assess, improve, and evolve this repository using the MCP server and the C# client.
---

# AI Runbook (PMCR‑O with MCP)

> Comment (Plan): Provide a deterministic, bounded procedure for an AI to operate on this repo.

## Prereqs
- Node and .NET SDK installed; DocFX for docs
- Ability to launch MCP server via `npx`

## Procedure
1. Plan
   - Read `docs/toc.yml` and `meta/docs-map.md` to enumerate pages
   - Select top 5 targets by impact (missing self‑assessment, high centrality, etc.)
   - If needed, generate new intents for testing: `node scripts/generate-intent.js "test scenario" "test-intent" 3`
2. Make
   - For each target, propose compact edits (sections, examples, cross‑links)
   - If validating patterns, run MCP: launch server and call the C# client with a sample intent
   - Test generated intents by running them through the C# client
3. Check
   - Recompute quality scores (Completeness, Accuracy, Relevance, Quality)
   - Ensure bounded steps and structured outputs
   - Validate generated intents produce expected results
4. Reflect
   - Write a short report with deltas and lessons learned
   - Assess intent generation effectiveness
5. Orchestrate
   - Schedule next iterations or open issues for larger work
   - If intent templates need updating, note for next run

## MCP run (example)
```powershell
dotnet run --project "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\src\ProjectName.McpClient" -- --config "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\intents\intent.override-server.json" --server "npx" --serverArgs "-y @modelcontextprotocol/server-everything"
```

## Guardrails
- Keep iterations ≤ 3; no unbounded parallelism
- Do not exfiltrate secrets; offline first
- Prefer edits that improve self‑assessment and cross‑linking

---

## PMCR-O Loop Execution
- Planner: Select targets, intents, and budgets.
- Maker: Execute orchestrated edits and MCP runs.
- Checker: Re-run validators and compute quality deltas.
- Reflector: Summarize learnings and adjust approach.
- Orchestrator: Schedule next run or open issues.

## Self-Assessment
- Completeness: Steps cover plan→make→check→reflect→orchestrate. Score: 0.9
- Accuracy: Commands and paths match repo layout. Score: 0.9
- Relevance: Focused on repo evolution. Score: 0.95
- Quality: Actionable and bounded. Score: 0.9

## Evolution Triggers
- If validation rate < 80%: prioritize docs fixes.
- If MCP runs fail: pin server versions or reduce scope.
- If CI time > 10m: parallelize non-dependent steps.

* Meta-Note: This runbook includes its own loop and metrics prompts to remain a living procedure.

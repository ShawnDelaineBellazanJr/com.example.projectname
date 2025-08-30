---
title: MCP Orchestration (Node ↔ C#)
description: How the Node orchestration scripts coordinate the C# MCP client with MCP servers using deterministic, structured runs.
---

# MCP Orchestration (Node ↔ C#)

> Comment (Plan): Show how scripts call the C# client, select MCP servers, and capture structured outputs for PMCR‑O loops.

## Components
- C# client: `src/ProjectName.McpClient/Program.cs`
  - Flags: `--config`, `--intent`, `--out`, `--server|--serverCommand`, `--serverArgs`
  - Writes structured JSON: runId, intent.mode, steps[], artifacts, summary, metrics
- MCP server: `npx @modelcontextprotocol/server-everything` (overrideable)
- Node scripts: `scripts/orchestrate.js`, `scripts/validate-structure.js`, `scripts/self-assess.js`, `scripts/evolution-triggers.js`

## Typical flow (PMCR‑O)
- Plan: orchestrator picks an intent config and server override
- Make: C# client executes calls[], templating across steps
- Check: validators parse the output JSON, compute scores
- Reflect: summarizer writes notes and deltas
- Orchestrate: coordinator schedules the next run or evolution

## Example run (Windows PowerShell)
Optional usage to demonstrate overrides (adjust paths if needed):

```powershell
dotnet run --project "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\src\ProjectName.McpClient" -- --config "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\intents\intent.override-server.json" --server "npx" --serverArgs "-y @modelcontextprotocol/server-everything"
```

## Contract between scripts and client
- Input: path to an intent JSON; optional `--server`/`--serverArgs`
- Output: JSON at `out/*.result.json` with a stable schema
- Error handling: non‑zero exit or invalid JSON triggers a remediation path

## Self‑assessment
- Completeness: are server overrides and outputs captured?
- Accuracy: do scripts use the same schema across runs?
- Quality: are steps bounded and labeled?

## Evolution triggers
- If outputs grow: add hashing and artifact pruning
- If drift: pin server version; lock context snapshot before run

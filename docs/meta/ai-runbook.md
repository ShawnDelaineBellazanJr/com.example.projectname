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
2. Make
   - For each target, propose compact edits (sections, examples, cross‑links)
   - If validating patterns, run MCP: launch server and call the C# client with a sample intent
3. Check
   - Recompute quality scores (Completeness, Accuracy, Relevance, Quality)
   - Ensure bounded steps and structured outputs
4. Reflect
   - Write a short report with deltas and lessons learned
5. Orchestrate
   - Schedule next iterations or open issues for larger work

## MCP run (example)
```powershell
dotnet run --project "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\src\ProjectName.McpClient" -- --config "C:\Users\tooen\OneDrive\Desktop\ThoughtTransfer\intents\intent.override-server.json" --server "npx" --serverArgs "-y @modelcontextprotocol/server-everything"
```

## Guardrails
- Keep iterations ≤ 3; no unbounded parallelism
- Do not exfiltrate secrets; offline first
- Prefer edits that improve self‑assessment and cross‑linking

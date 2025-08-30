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

## Node orchestration script
```javascript
const { exec } = require('child_process');
const fs = require('fs');

function runIntent(configPath, outPath) {
  const command = `dotnet run --project src/ProjectName.McpClient -- --config ${configPath} --out ${outPath}`;
  
  exec(command, (error, stdout, stderr) => {
    if (error) {
      console.error(`Error: ${error.message}`);
      return;
    }
    console.log(`Output: ${stdout}`);
    
    // Parse and process the output JSON
    const result = JSON.parse(fs.readFileSync(outPath, 'utf8'));
    console.log('Run completed:', result.run.id);
  });
}

// Example
runIntent('intents/intent.loop.json', 'out/loop.result.json');
```

## Node validation script
```javascript
const fs = require('fs');

function validateOutput(outPath) {
  const data = JSON.parse(fs.readFileSync(outPath, 'utf8'));
  
  if (!data.run || !data.calls) {
    throw new Error('Invalid output structure');
  }
  
  console.log('Validation passed');
}

// Example
validateOutput('out/result.json');
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

## Intent Generation (Template-Driven)

The system supports declarative, template-driven intent generation from high-level descriptions. This embodies the PMCR‑O loop by allowing users to specify an app idea (e.g., "chop chop landscaping marketplace app") and automatically generating a structured intent JSON with bounded steps.

### Template System
- Base template: `intents/template.intent.json`
  - Server: configurable MCP server (defaults to everything server)
  - Calls: array of tool invocations with templated parameters
  - Output: structured result JSON
- Generator script: `scripts/generate-intent.js`
  - Takes description, name, and step count
  - Fills templates with generic app development steps
  - Replaces `{{description}}` with the provided text

### Example Generation
```bash
node scripts/generate-intent.js "chop chop landscaping marketplace app" "chop-chop-landscaping" 8
```

This creates `intents/intent.chop-chop-landscaping.json` with 8 echo calls, each analyzing a different aspect of the app.

### Orchestration Integration
The `orchestrate.js` can generate intents on-the-fly:

```bash
INTENT_DESCRIPTION="my new app idea" INTENT_NAME="my-app" INTENT_STEPS=5 node scripts/orchestrate.js
```

This adds `generate_intent` to the orchestration sequence, creating the intent before running assessments.

### Self-Assessment
- Declarative: templates ensure consistent structure
- Bounded: step count prevents runaway generation
- Reusable: same generator for any app description

### Evolution Triggers
- If templates stale: add more step templates for specific domains
- If generation too generic: integrate AI for custom step creation
- If orchestration grows: make generation a separate microservice

---

## PMCR-O Loop Execution
- Planner: Choose intent, server overrides, and output targets.
- Maker: Run C# client via Node scripts; emit structured result JSON.
- Checker: Validate schema, compute scores, and verify artifacts.
- Reflector: Summarize deltas; capture drift and remediation notes.
- Orchestrator: Schedule follow-up runs or template updates.

## Meta-Commentary
- Note: This document doubles as a runbook for the orchestrator and contains its own assessment and triggers so the validator can close the loop.

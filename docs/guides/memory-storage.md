---
title: Memory & Persistence (SQLite)
description: How to persist MCP runs, artifacts, and indices in SQLite for retrieval, analytics, and evolution.
---

# Memory & Persistence (SQLite)

> Comment (Plan): Propose a minimal, practical schema for storing structured outputs, artifacts, and indices in SQLite; align with PMCR‑O and O‑dash‑O.

## Why store memory
- Reuse context across runs (optimizer/orchestrator)
- Enable audits and analytics (quality gates, drift detection)
- Power retrieval‑augmented flows with locked provenance

## Canonical envelope → tables
Structured run JSON fields map cleanly to relational tables:

- runs(run_id TEXT PRIMARY KEY, intent_name TEXT, mode TEXT, started_at TEXT, ended_at TEXT, server_name TEXT)
- steps(run_id TEXT, step_id TEXT, label TEXT, tool TEXT, input_json TEXT, output_text TEXT, output_json TEXT, score REAL, selected INTEGER, PRIMARY KEY(run_id, step_id))
- artifacts(run_id TEXT, artifact_id TEXT, type TEXT, path TEXT, sha256 TEXT, meta_json TEXT, PRIMARY KEY(run_id, artifact_id))
- summary(run_id TEXT PRIMARY KEY, decision TEXT, rationale TEXT, notes_json TEXT)
- metrics(run_id TEXT PRIMARY KEY, duration_ms INTEGER, branches_tried INTEGER, tokens INTEGER)
- idx_text(run_id TEXT, step_id TEXT, token_count INTEGER, keywords TEXT)

Notes:
- Store JSON as TEXT; keep schema simple and portable.
- Add FKs if desired; SQLite doesn’t require for small setups.

## Ingest pattern
1) Run the C# MCP client and emit `out/*.result.json`.
2) Use a small ingestion script (Node/C#) to parse and upsert rows per table.
3) Optionally compute `idx_text.keywords` and token counts.

## SQLite ingestion script (Node.js)
```javascript
const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');
const path = require('path');

const db = new sqlite3.Database('mydatabase.db');

// Create tables if not exist
db.serialize(() => {
  db.run(`CREATE TABLE IF NOT EXISTS runs (
    run_id TEXT PRIMARY KEY,
    intent_name TEXT,
    mode TEXT,
    started_at TEXT,
    ended_at TEXT,
    server_name TEXT
  )`);

  db.run(`CREATE TABLE IF NOT EXISTS steps (
    run_id TEXT,
    step_id TEXT,
    label TEXT,
    tool TEXT,
    input_json TEXT,
    output_text TEXT,
    output_json TEXT,
    score REAL,
    selected INTEGER,
    PRIMARY KEY(run_id, step_id)
  )`);

  // Add other tables similarly
});

// Ingest from JSON file
function ingestResult(jsonPath) {
  const data = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));
  
  const runId = data.run.id;
  db.run(`INSERT OR REPLACE INTO runs VALUES (?, ?, ?, ?, ?, ?)`, 
    [runId, data.intent, data.mode, data.started_at, data.ended_at, data.server.name]);

  data.calls.forEach((call, idx) => {
    db.run(`INSERT OR REPLACE INTO steps VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [runId, `step-${idx}`, call.label || '', call.tool, JSON.stringify(call.params), call.text, '', null, null]);
  });
}

// Example usage
ingestResult('out/result.json');
```

## Query script (Node.js)
```javascript
const db = new sqlite3.Database('mydatabase.db');

db.all(`SELECT run_id, intent_name, started_at FROM runs ORDER BY started_at DESC LIMIT 10`, (err, rows) => {
  if (err) console.error(err);
  else console.log('Latest runs:', rows);
});
```

## Query examples
- Latest successful runs per intent:
  SELECT run_id, intent_name, mode, started_at FROM runs ORDER BY started_at DESC LIMIT 10;
- Top‑scoring steps across runs:
  SELECT run_id, step_id, label, score FROM steps WHERE score IS NOT NULL ORDER BY score DESC LIMIT 20;
- Artifacts without checksums (needs remediation):
  SELECT run_id, artifact_id FROM artifacts WHERE sha256 IS NULL OR sha256 = '';

## MCP interop
- Treat the DB as a memory substrate: load summaries for context; lock provenance via run_id.
- Orchestrator can pick prior best runs (by score) and feed them into new intents.

## Self‑assessment
- Completeness: do all envelope fields land in tables?
- Accuracy: are timestamps, hashes, and scores populated?
- Relevance: are queries aligned with optimization goals?
- Quality: can we reproduce a run from stored data?

## Evolution triggers
- If `artifacts.sha256` missing: add hashing to post‑run.
- If average score < τ: schedule rubric revision in prompts.
- If DB size > threshold: archive old runs to cold storage.

---

## PMCR-O Loop Execution
- Planner: Define the minimal schema and ingestion cadence.
- Maker: Ingest runs/artifacts/metrics into SQLite deterministically.
- Checker: Run integrity queries (null hashes, missing rows) and compute indices.
- Reflector: Compare query utility vs. cost; refine schema only when needed.
- Orchestrator: Schedule compaction/archival and refresh indices.

## Meta-Commentary
- Meta-Note: The page declares contracts and checks it expects from itself and the ingestion scripts to remain a living spec.

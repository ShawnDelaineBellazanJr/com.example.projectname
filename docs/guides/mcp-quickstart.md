# MCP Quickstart for ThoughtTransfer (PMCR‑O Aligned)

This guide shows how to activate and use the configured MCP servers in this repo, including the newly added `github-official` and `sqlite` servers.

## Overview
- Location: `.vscode/mcp.json`
- Ready without secrets: `telerikBlazorAssistant`, `telerik-maui-assistant`, `filesystem`, `memory`, `sequential-thinking`, `browser`, `code-runner`
- Require env tokens: `supabase`, `sentry`, `heroku`, `hubspot`, `browserstack`, `notion`, `youtube`, `apify`, `graphlit`, `github`, `github-official`
- Local DB server: `sqlite` → `${workspaceFolder}/mydatabase.db`

## Quick activation (Windows PowerShell)
Set any tokens you have for this session before starting your Chat session.

```powershell
# Examples (set only what you need)
$env:GITHUB_TOKEN = "<your_token>"
$env:SUPABASE_ACCESS_TOKEN = "<your_token>"
$env:NOTION_API_KEY = "<your_token>"
$env:BROWSERSTACK_USERNAME = "<user>"; $env:BROWSERSTACK_ACCESS_KEY = "<key>"
```

## Minimal usage samples
- GitHub (official) — requires `GITHUB_TOKEN`:
	- "List open issues in this repo with titles and numbers"
	- "Create an issue titled 'DocFX link fix' with body 'Update CHANGELOG README link'"
	- "Search code for 'PMCR-O' in this repository"
- SQLite — uses `${workspaceFolder}/mydatabase.db`:
	- "Run SQL: CREATE TABLE IF NOT EXISTS demo_items (id INTEGER PRIMARY KEY, name TEXT NOT NULL)"
	- "Run SQL: INSERT INTO demo_items (name) VALUES ('alpha'), ('beta')"
	- "Run SQL: SELECT COUNT(*) AS c FROM demo_items; then fetch first 5 rows"
- Telerik assistants: Ask for “Grid getting started (Blazor)” or “MAUI TabView usage”.

## PMCR‑O Loop Execution
- Planner: Decompose your immediate intent (e.g., “verify DB is reachable, then fetch docs”).
- Maker: Use appropriate MCP servers (SQLite, Telerik assistants) to execute.
- Checker: Validate outputs (table list non-empty, docs contain expected components).
- Reflector: Note frictions (missing tokens, access) and record improvements.
- Orchestrator (‑O): Choose chain/tree/graph of steps; prioritize novelty vs safety as needed.

## Self‑Assessment
- Completeness: Covers activation, usage, and PMCR‑O framing.
- Accuracy: Matches `.vscode/mcp.json` and repo files (`mydatabase.db`).
- Relevance: Focused on getting MCP running fast.
- Quality: Clear, concise; expandable via `docs/guides/vscode-mcp-integration.md`.

## Evolution Triggers
- If new servers are added: append to this guide with env keys and quick tests.
- If DB schema changes: include example queries for new tables.
- If tokens missing frequently: add a `.env.example.ps1` in future.

## Meta-Commentary
<!--
PMCR‑O Notes:
- This quickstart declares its own self-assessment and evolution triggers.
- It references `.vscode/mcp.json` and encourages synchronization with config changes.
- Future automation can parse this section to auto-raise PRs when servers change.
-->
This guide is intentionally self-referential and set up to evolve along with server configuration and usage patterns.

---
title: Agent Framework Patterns
description: Designing agent topologies and contracts aligned with O‑dash‑O and PMCR‑O.
---

# Agent Framework Patterns

> Comment (Plan): Provide reusable patterns for single, debate, ensemble, and orchestrator agents.

## Topologies
- Single agent (Chain/Optimizer)
- Debate + Judge (Tree/Graph)
- Ensemble → Synthesis (Graph)
- Orchestrator of agents (meta‑controller)

## Contracts
- Message: {id, role, content, contextRef, budget}
- Tool call: {name, params, expects}
- Output: {text|json, score?, selected?, artifacts}

## Guardrails
- Budgets: tokens/steps/time
- Safety: policy checks, rollback
- Audit: logs, lineage, checksums

## Metrics
- Quality score, coverage/diversity, latency, cost

## Self‑assessment & evolution
- If regressions: reduce budgets; add judge step
- If low diversity: add probes and merge
- If drift: lock context tighter

---

## PMCR-O Loop Execution
- Planner: Define agent topology, contracts, and guardrails for this use case.
- Maker: Implement steps and budgets; wire tool calls with deterministic params.
- Checker: Measure quality, diversity, latency, and cost; enforce rollback on policy failures.
- Reflector: Compare outcomes across topologies; record deltas and lessons.
- Orchestrator: Route tasks to single/debate/ensemble/orchestrator agents by policy.

## Self-Assessment
- Completeness: Covers topologies, contracts, guardrails, metrics. Score: 0.9
- Accuracy: Patterns align with current MCP intent schema. Score: 0.9
- Relevance: Focuses on practical patterns for this repo. Score: 0.9
- Quality: Skimmable with actionable checklists. Score: 0.9
- Improvement Suggestions: Add runnable intent examples per topology.

## Evolution Triggers
- If cost spikes >20%: tighten budgets or reduce branches.
- If diversity < threshold: add probes or alternate prompts.
- If drift detected: enforce stricter locked-context snapshots.

* Meta-Note: This page declares how it evolves itself and how to audit its usage.

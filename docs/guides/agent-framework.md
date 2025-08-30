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

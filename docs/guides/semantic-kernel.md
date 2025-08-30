---
title: Semantic Kernel Integration
description: Mapping O‑dash‑O and PMCR‑O to Semantic Kernel Process and Agent frameworks.
---

# Semantic Kernel Integration

> Comment (Plan): Show concrete mappings so the same patterns work in SK and MCP.

## Process framework mapping
- Chain → pipeline of functions; pass context; cap steps.
- Tree → parallel function variants; judge function selects.
- Graph → DAG of functions; merge nodes synthesize.
- Optimizer → loop with improve/evaluate/refine; stop at K or τ.

## Agent framework mapping
- Orchestrator agent decomposes tasks, calls worker agents (chain/tree/graph), aggregates.
- Contracts: message schema {role, content, contextRef, budget}.
- Guardrails: per‑agent budgets; deterministic caps; audit logs.

## Interop with MCP
- Treat MCP intents as tools callable from SK; pass snapshots and read artifacts.
- Reuse the structured output envelope for traceability.

## Self‑assessment & evolution
- Validate caps, outputs, and selection logic.
- If agent drift rises: increase context locking and reduce degrees of freedom.

---

## PMCR-O Loop Execution
- Planner: Map O‑O shapes to SK processes/agents and define budgets.
- Maker: Implement pipelines/agents with deterministic caps.
- Checker: Assert envelope fields, scores, and selections.
- Reflector: Compare SK vs. MCP parity; note divergences.
- Orchestrator: Route tasks to SK or MCP based on policy and readiness.

## Meta-Commentary
- Meta-Note: This page declares its own audit loop so SK integration remains aligned with MCP patterns over time.

## Self-Assessment
- Completeness: Mappings for process/agent frameworks, MCP interop, PMCR-O execution, and meta commentary are present (score: 0.9).
- Accuracy: Aligns with current SK concepts and repo conventions; avoids unsupported features (score: 0.85).
- Relevance: Directly actionable for wiring SK to existing MCP intents and envelopes (score: 0.9).
- Quality: Concise, skimmable, and testable via example pipelines (score: 0.9).

## Evolution Triggers
- If SK pipelines exceed planned step caps: reduce depth or parallel fan-out; add stricter budgets.
- If envelope fields are missing in traces: add assertions and fail builds in CI.
- If MCP and SK outputs diverge > 10% on shared tasks: review prompts and unify selection logic.

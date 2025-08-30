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

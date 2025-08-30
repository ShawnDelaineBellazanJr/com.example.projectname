---
title: Prompt Engineering Guide
description: Deterministic, auditable prompting with O‑dash‑O topologies and PMCR‑O loops for MCP intents and agents.
---

# Prompt Engineering Guide

> Comment (Plan): Provide contracts, patterns, and safeguards to keep prompts bounded, portable, and evaluable across tools and agents.

## Overview
This guide standardizes prompt design for ThoughtTransfer. Goals: determinism, bounded budgets, explicit roles, and structured outputs that feed downstream steps.

## Contract
- Inputs: task, constraints (tokens/steps/time), context snapshot, role, success criteria.
- Outputs: step output {text|json}, optional score/selected, artifacts, summary note.
- Error modes: ambiguity, context drift, overlong outputs, policy conflicts.
- Success: meets criteria, within budget, verifiable from steps/artifacts.

## O‑dash‑O mappings
- Chain: stepwise prompts referencing {{last.Text}}; 3–5 steps.
- Tree: generate H1/H2/H3; judge prompt scores and selects.
- Graph: independent branches; merge prompt synthesizes multiple prior outputs.
- Orchestrator: planner prompt decomposes task into child intents; aggregates.
- Optimizer: propose→evaluate→refine; termination at K≤3 or score≥τ.

## Patterns
- Role framing: set audience, tone, constraints in 1–2 lines; avoid over‑scoping.
- Budgeting: instruct max tokens, step caps, and explicit stop criteria.
- Locked context: reference only provided snapshot; forbid unstated assumptions.
- Scoring rubric: list 3–5 criteria; numeric [0–1] or [0–100].
- Selection: require selected=true only when score ≥ τ; record rationale.
- Safety: include “do not access external resources” unless explicitly allowed.

## Mini‑templates
- Hypotheses (ToT): “Produce 3 distinct solutions (H1–H3)… stay within X tokens each.”
- Judge: “Score H1–H3 using criteria A/B/C; choose one winner; output JSON {scores, selected}.”
- Merge: “Synthesize H1 and H2; preserve constraints; output max N bullets.”
- Refine (Optimizer): “Improve the current best; do not increase scope; target weaknesses only.”

## MCP usage
- Place prompts in intent calls[] params with placeholders; keep them short and explicit.
- Emit structured outputs; avoid freeform prose without headings.

## Self‑assessment
- Completeness: role+budget+criteria present?
- Accuracy: constraints reflected in outputs?
- Relevance: prompt fits chosen O‑O topology?
- Quality: outputs parseable and audit‑friendly?

## Evolution triggers
- If average score < τ across runs: tighten rubric or reduce scope.
- If outputs exceed budgets: shorten prompts; enforce bullet limits.
- If drift detected: add context lock and provenance note.

---

## PMCR-O Loop Execution
- Planner: Select topology, budgets, and output contracts.
- Maker: Author prompts with deterministic placeholders and constraints.
- Checker: Validate outputs against rubric and budget adherence.
- Reflector: Capture weaknesses and reduce scope where needed.
- Orchestrator: Standardize templates across intents and agents.

## Meta-Commentary
- Meta-Note: This guide is both a style manual and a compliance checklist; it contains loops and triggers so automation can evolve it.
---
title: Prompt Engineering for Bounded, Auditable Systems
description: Patterns for deterministic, self-referential prompting with O‑dash‑O shapes and MCP intents.
---

# Prompt Engineering for Bounded, Auditable Systems

> Comment (Plan): Define locked prompts, bounded budgets, deterministic placeholders, and O‑O shape alignment for MCP intents.

## Goals
- Deterministic, reproducible prompts with explicit budgets.
- Traceable flows: every step auditable via structured outputs.
- Portable patterns across MCP client, webcam/Brock, and orchestrations.

## Core patterns
- Locked Context: Pin the minimal facts that must not drift; treat as read‑only input.
- Budgeted Reasoning: Cap steps/branches/tokens; surface limits in the prompt.
- Explicit Topology: Name the O‑O mode (chain/tree/graph/orchestrator/optimizer) in the instruction.
- Deterministic Placeholders: Use {{last.Text}} and {{Calls[i].Text}} to pass prior results.
- Scoring & Selection: When branching, require a judge with criteria and selected=true on the winner.

## Prompt template skeleton
Instruction: "You are executing an O‑O = {chain|tree|graph|orchestrator|optimizer}. Stay within {maxSteps} steps and {maxTokens} tokens. Only use the locked context and the provided inputs."

Locked Context: "{facts}"

Inputs: "{inputs}"

Output Contract: "Return a concise answer, and if judging, include a numeric score in [0,1]."

## Examples
- Chain (3 steps): analyze → summarize → finalize using {{last.Text}}.
- Tree (3 branches + judge): H1/H2/H3, then judge scores and selects; record steps[].score and selected.
- Graph (merge): two branches then a synthesis step consuming both outputs.

## Self‑assessment
- Completeness: Prompts specify topology, budgets, and contracts.
- Accuracy: Placeholders resolve to existing steps.
- Relevance: Context minimized to avoid drift.
- Quality: Outputs map cleanly to structured fields.

## Evolution triggers
- If outputs exceed size limit, add stricter budget and summarization instruction.
- If judges disagree across runs, refine criteria and add tie‑breakers.
- If branches rarely improve results, reduce branch count.

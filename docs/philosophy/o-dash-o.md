---
title: O‑dash‑O (O‑O): A unifying topology for thought orchestration
description: Formal definitions and practical mappings of O‑dash‑O (Chain/Tree/Graph/Orchestrator/Optimizer of Thought) within ThoughtTransfer, aligned to PMCR‑O.
---

# O‑dash‑O (O‑O): A unifying topology for thought orchestration

> Comment (Plan): Purpose is to define O‑dash‑O precisely, map it to MCP intent patterns, and embed self‑assessment and evolution triggers.

O‑dash‑O (short: O‑O) is a family of interoperable thinking topologies used by ThoughtTransfer to structure, run, and improve reasoning. “O” denotes the organizing shape of cognition; the dash emphasizes abstraction over a single method. The common variants are:

- Chain of Thought (CoT): Linear sequence of bounded steps.
- Tree of Thought (ToT): Branching hypotheses with evaluation and selection.
- Graph of Thought (GoT): Interconnected branches with merges/syntheses.
- Orchestrator of Thought: A coordinator that schedules child runs and aggregates.
- Optimizer of Thought: Iterative improvement with propose→evaluate→refine cycles.

All variants are bounded, auditable, and portable across apps by sharing a single structured output envelope.

## Formal definitions

- Chain of Thought (CoT): A directed path G = (V,E) where |V| ≤ N and E forms a Hamiltonian path over V without cycles; each node consumes outputs from its immediate predecessor only.
- Tree of Thought (ToT): A rooted tree T with branching factor b and depth d, with an explicit evaluation function s: V → ℝ and a selector that marks one or more nodes as selected ∈ V* under a budget constraint.
- Graph of Thought (GoT): A directed acyclic graph (DAG) with multiple ingress/egress nodes; merge nodes consume multiple predecessors and emit synthesized outputs; evaluation and selection can occur at any layer under budget N.
- Orchestrator: A meta‑controller O that composes and schedules subgraphs Gi under policy π, collects their artifacts, and emits a single aggregate result.
- Optimizer: A bounded iterative process over state x_k with operators {propose, evaluate, refine}; termination when k = K or score ≥ τ; returns argmax_x s(x).

Constraints: determinism via fixed budgets (N,K,b,d), explicit scoring, and serial execution if parallelism is unavailable.

## Structured output envelope (canonical)

Every run yields JSON with these fields:

- runId: string (ULID/UUID)
- intent: { name: string, mode: "chain|tree|graph|orchestrator|optimizer" }
- server: { name: string, command: string, args: string[] }
- steps: [{ id: string, label: string, tool: string, inputs: object, output: { text?: string, json?: object }, score?: number, selected?: boolean }]
- artifacts: [{ type: string, path?: string, dataRef?: string, sha256?: string, meta?: object }]
- summary: { decision: string, rationale: string, notes?: string[] }
- metrics: { durationMs?: number, branchesTried?: number, tokens?: number }

## MCP intent mapping

- CoT: Encode as calls[]; use placeholders like {{last.Text}}; limit steps to 3–5.
- ToT: Create H1/H2/H3 calls; add a Judge call that scores and sets selected=true on the winner.
- GoT: Add merge step that consumes {{Calls["H1"].Text}} and {{Calls["H2"].Text}}; produce synthesis.
- Orchestrator: One meta‑intent or external script invokes child intents; aggregate their artifacts by runId.
- Optimizer: Loop for K≤3: propose→evaluate→refine; keep best‑so‑far and mark selected.

## Cross‑app portability (webcam/Brock)

Keep the same envelope; change tools/labels only:

- steps: [capture, preprocess, analyze, summarize]
- artifacts: frame paths, hashes, analysis JSON
- summary: concise outcome, bounded metrics

## Self‑assessment

Use these checks after authoring an O‑O flow:

- Completeness: Are all envelope fields present? Are steps labeled and bounded?
- Accuracy: Do placeholders reference valid step outputs? Are scores computed where needed?
- Relevance: Does the topology match the problem (chain vs tree vs graph)?
- Quality: Is the summary auditable from steps and artifacts?

Target ≥85% on automated quality checks.

## Evolution triggers

- If branchesTried > planned budget, reduce b or d and tighten prompts.
- If selected score < τ for two runs, adjust propose or evaluation prompts and retry.
- If artifacts grow > X MB, add hashing and prune raw payloads.
- If steps >5 repeatedly, refactor to orchestrator with child intents.

> Comment (Check): Verified definitions map to existing MCP client features without new dependencies.

> Comment (Reflect): The page unifies O‑O shapes as topologies with a single output envelope, enabling consistent ops and evaluation.

> Comment (Optimize): Next revision could add JSON‑Path style templating for nested outputs and small examples per topology.

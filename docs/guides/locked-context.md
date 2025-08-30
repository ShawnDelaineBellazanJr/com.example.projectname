---
title: Locked Context Guide
description: Building safe, reproducible context snapshots with provenance and drift control.
---

# Locked Context Guide

> Comment (Plan): Define how to bind runs to explicit context and verify provenance to prevent drift.

## Snapshot
- Capture: inputs, versions, hashes, time, scope.
- Store: artifacts[] entries with sha256 and meta (source, schema version).
- Freeze: disallow adding unseen sources mid‑run; require explicit opt‑in.

## Verification
- Provenance: verify hashes; record mismatches.
- Scope: enforce whitelist of allowed sources.
- Drift: compare snapshot to current; block if changed beyond threshold.

## Usage in MCP intents
- Step 0: context‑audit tool → emits snapshot.
- Steps consume only snapshot; placeholders reference artifact IDs.
- Summary notes any opt‑ins and their rationale.

## Self‑assessment
- Completeness: snapshot present? hashes? scope?
- Accuracy: hashes match? sources whitelisted?
- Relevance: only needed context included?
- Quality: audit trail sufficient?

## Evolution triggers
- If drift > threshold: tighten scope or pin versions.
- If provenance gaps: require source registration before use.
---
title: Locked Context: Making Reasoning Stable
description: Techniques to pin critical facts, snapshots, and constraints so reasoning stays bounded and auditable.
---

# Locked Context: Making Reasoning Stable

> Comment (Plan): Define locked context, snapshots, scoping, and verification; map to MCP intent schema.

## What is locked context?
Locked context is the minimal, authoritative information a run is allowed to rely on. It does not change during the run and is explicitly separated from open‑world inference.

## Techniques
- Snapshots: Hash and timestamp inputs; store in artifacts with sha256.
- Scoping: Limit context to task‑critical facts and citations.
- Isolation: Keep locked context read‑only; pass it verbatim to each step.
- Provenance: Record source and integrity checks.

## MCP mapping
- Include locked context in each step’s inputs.
- Store the snapshot as an artifact with type="locked-context".
- Reference locked context via placeholders instead of re‑querying external sources.

## Verification
- Add a “context‑audit” step that lists which parts of the output trace back to locked inputs.
- Fail the run if outputs cite facts not present in locked context (unless explicitly allowed).

## Edge cases
- Large context: summarize with citations, then lock the summary.
- Streaming inputs: create rolling snapshots; annotate which snapshot a step used.

## Self‑assessment
- Completeness: Snapshot, provenance, and scope recorded.
- Accuracy: Outputs reference locked items.
- Relevance: Only necessary facts included.
- Quality: Hashes and timestamps present.

## Evolution triggers
- If drift detected, narrow scope or strengthen summarization policy.
- If missing provenance, block downstream steps until fixed.

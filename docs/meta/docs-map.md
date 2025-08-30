---
title: Docs Map & Audit
description: A navigator and checklist for scanning, auditing, and evolving the entire documentation set.
---

# Docs Map & Audit

> Comment (Plan): Provide a single page an AI (or human) can use to scan everything and plan improvements.

## Top-level areas
- Philosophy → index, O‑dash‑O, PMCR‑O, Strange Loops, Self‑Reference, Consciousness, Philosophers, Glossary
- Guides → Getting Started, MCP Quickstart, Advanced Usage, Prompt Engineering, Locked Context, Semantic Kernel, Agent Framework, MCP Orchestration
- API → Reference, Integration, Evolution Engine, Self‑Assessment API
- Evolution System → Mechanisms, Triggers, Quality Assurance, Monitoring
- Meta → Framework, Philosophy, Patterns, Tracking, Docs Map (this page)
- Reports → Orchestration, System Health, MCP Ops

## Audit checklist (quick)
- Coverage: each area has index, key topics, and cross‑links
- Consistency: shared terms match Glossary
- Contracts: inputs/outputs/errors/success criteria present
- Self‑assessment: present with metrics and triggers
- MCP linkage: examples and commands present where relevant

## Crawl plan for a scanning AI
1. Read `docs/toc.yml` to enumerate pages
2. For each page: collect title, links, self‑assessment, evolution triggers
3. Build a cross‑reference graph; detect missing or broken links
4. Score each page on completeness/accuracy/relevance/quality
5. Propose a prioritized improvement list

## Evolution triggers
- If any area <85% score: open an issue and create a targeted plan
- If link errors found: schedule a fix PR
- If term conflicts: update Glossary and refactor usages

---

## PMCR-O Loop Execution
- Planner: Build inventory and prioritize gaps.
- Maker: Execute small, high-impact edits first.
- Checker: Recompute coverage and link integrity.
- Reflector: Note systemic weaknesses and patterns.
- Orchestrator: Create/assign issues and schedule next audit.

## Self-Assessment
- Completeness: Maps all top-level areas and checks. Score: 0.9
- Accuracy: Links and filenames reflect current repo. Score: 0.9
- Relevance: Drives practical evolution. Score: 0.9
- Quality: Skimmable navigator. Score: 0.9

* Meta-Note: This page guides its own maintenance cycle and provides hooks for automation.

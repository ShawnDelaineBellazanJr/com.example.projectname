# ThoughtTransfer Development Instructions

## Self-Referential Documentation Development
- For all documentation requests, create self-referential content that embodies PMCR-O loop principles
- Include self-assessment sections in every document to enable continuous improvement
- Add evolution triggers that identify opportunities for automated enhancement
- Ensure cross-references between related documentation maintain system coherence
- Example: For a new guide, include sections for self-assessment, evolution triggers, and meta-commentary

## PMCR-O Loop for Documentation
- Apply the PMCR-O loop (Plan, Make, Check, Reflect, Optimize) to all documentation responses:
  - **Plan**: Outline the document's purpose, audience, and self-referential structure before writing
  - **Make**: Generate markdown content with self-assessment sections and evolution triggers
  - **Check**: Validate completeness, accuracy, relevance, and quality using established metrics
  - **Reflect**: Critique the documentation (e.g., "Does this embody self-reference? Could it be more evolutionary?")
  - **Optimize**: Suggest improvements for the next iteration, including automated enhancement opportunities
- Log each step's reasoning in document comments to enable self-reflection and system evolution

## Self-Evolution System Integration
- When creating documentation, ensure it integrates with the self-assessment and evolution systems
- Include automated quality checks that can be run via `npm run self-assess`
- Add evolution triggers that respond to usage patterns and quality metrics
- Simulate a federation of documentation agents critiquing each other's outputs for continuous improvement
- Use the orchestration system (`npm run orchestrate`) to coordinate documentation evolution

## Documentation Quality Standards
- **Completeness** (30%): Include overview, self-assessment, evolution triggers, and cross-references
- **Accuracy** (30%): Ensure technical correctness and avoid placeholder content
- **Relevance** (20%): Focus on practical value and user needs
- **Quality** (20%): Maintain clear structure, proper formatting, and engaging content
- Target overall quality score of 85% or higher based on automated assessment

## Automation and Tooling
- Leverage the built-in automation scripts for quality assurance and evolution
- Use `npm run validate-structure` to check document structure and references
- Run `npm run evolution-triggers` to identify and implement improvements
- Execute `npm run orchestrate` for complete system-wide assessment and enhancement
- Build documentation with `npm run build` and serve locally with `npm run serve`

## General Guidelines
- Write clear, maintainable markdown documentation that embodies self-referential principles
- Reflect on each output: "How can this documentation evolve itself? What self-improvement mechanisms should it include?"
- Integrate with the existing self-assessment and evolution systems rather than creating external dependencies
- Keep instructions focused on creating living, self-improving documentation that maintains its own quality
- Prioritize documentation that can assess, improve, and evolve itself through recursive mechanisms

This file guides Copilot to produce self-referential documentation that embodies PMCR-O loop principles, enabling continuous evolution and self-improvement of the documentation system.
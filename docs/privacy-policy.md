---
title: Privacy Policy
---

# Privacy Policy

This project prioritizes user privacy. The default apps and templates do not collect personal data unless explicitly configured.

## Data Collection
- Optional analytics can be enabled via `wwwroot/data/site.json` (analytics section).
- No background data collection occurs by default.

## Data Storage
- Local settings may be stored on-device for preferences.

## Contact
See `docs/README.md` for maintainer contact details.

## PMCR-O loop execution
### Plan
Provide a publishable privacy policy suitable for app stores and static hosting.

### Make
Markdown doc with clear sections and minimal legalese aligned to current code behavior.

### Check
Confirm defaults (no telemetry) and configuration points (`analytics` in `site.json`).

### Reflect
This policy must evolve with any analytics or telemetry additions.

### Optimize
Automate checks to flag divergence between policy and configuration.

## Self-Assessment
- Completeness: Covers collection, storage, contact, and PMCR-O context.
- Accuracy: Matches current implementation (no data collection by default).
- Relevance: Focused on end-user transparency and store requirements.
- Quality: Clear headings; minimal jargon.

## Evolution Triggers
- If `analytics.enabled == true` in `site.json`, trigger a docs update task to include provider and data categories.
- On new data flows (e.g., crash reporting), create a changelog entry and append a section here.
- If app distribution targets change (e.g., new store), validate policy URL and format requirements.

## Meta-Commentary
This page is self-referential and tied to configuration-driven behavior. It should be validated by `npm run validate-structure` and kept in sync with `site.json` via build-time checks.
# Privacy Policy

Last updated: {{DATE}}

This Privacy Policy describes how {{BUSINESS_NAME}} ("we", "our", or "us") collects, uses, and protects your information when you use our applications and websites.

## What we collect
- Basic device and usage analytics (for app performance and improvements)
- Contact information you voluntarily provide (e.g., email, phone) for support or booking
- Content you submit within the app (e.g., requests, feedback)

We do not sell personal information.

## How we use your information
- To provide and improve the app experience
- To communicate support updates or service notices
- To analyze aggregate usage trends (e.g., via GA4 if configured)

## Data retention
We retain information only as long as necessary to provide the service and meet legal obligations.

## Your choices
- You may request deletion of your data by contacting us at {{EMAIL}}
- You may opt-out of non-essential analytics in your device settings if available

## Security
We use reasonable technical and organizational measures to protect your information. No method of transmission or storage is 100% secure.

## Children’s privacy
Our services are not directed to children under 13. If you believe a child provided us information, contact {{EMAIL}} to request deletion.

## Contact
If you have questions about this policy, contact:

- Business: {{BUSINESS_NAME}}
- Location: {{CITY}}, {{STATE}}
- Email: {{EMAIL}}
- Phone: {{PHONE}}

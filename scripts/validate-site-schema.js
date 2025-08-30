#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

function fail(msg) {
  console.error(`❌ Site schema validation failed: ${msg}`);
  process.exit(1);
}

function warn(msg) {
  console.warn(`⚠️  ${msg}`);
}

const explicit = process.argv[2];
const sitePath = explicit || process.env.SITE_JSON || path.resolve(process.cwd(), 'src', 'ProjectName.WebApp', 'wwwroot', 'data', 'site.json');

if (!fs.existsSync(sitePath)) {
  fail(`site.json not found at ${sitePath}. Set SITE_JSON or pass a path argument.`);
}

let json;
try {
  json = JSON.parse(fs.readFileSync(sitePath, 'utf8'));
} catch (e) {
  fail(`Invalid JSON at ${sitePath}: ${e.message}`);
}

// Required top-level mirrors
if (!json.name || typeof json.name !== 'string' || !json.name.trim()) {
  fail('Missing or empty top-level "name"');
}
if (!json.hero || typeof json.hero !== 'object') {
  fail('Missing top-level "hero" object');
}
if (json.about === undefined) {
  fail('Missing top-level "about" field');
}

// Metrics
if (!json.metrics || typeof json.metrics !== 'object') {
  fail('Missing top-level "metrics" object');
}
const { buildTimestampUtc, generator, docsScore } = json.metrics;
if (!buildTimestampUtc || typeof buildTimestampUtc !== 'string') {
  fail('metrics.buildTimestampUtc must be a string');
}
// Basic ISO check
if (isNaN(Date.parse(buildTimestampUtc))) {
  fail('metrics.buildTimestampUtc is not a valid date string');
}
if (!generator || typeof generator !== 'string') {
  fail('metrics.generator must be a string');
}
if (docsScore !== undefined && typeof docsScore !== 'number') {
  warn('metrics.docsScore is present but not a number; continuing');
}

console.log(`✅ Site schema validation passed: ${sitePath}`);

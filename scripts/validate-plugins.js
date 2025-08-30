#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

const pluginsRoot = process.env.PLUGINS_DIR || path.resolve(process.cwd(), 'src', 'ProjectName.WebApp', 'plugins');

if (!fs.existsSync(pluginsRoot)) {
  console.log('ℹ️  Plugins directory not found; skipping plugin validation.');
  process.exit(0);
}

const entries = fs.readdirSync(pluginsRoot, { withFileTypes: true });
let failures = 0;

for (const entry of entries) {
  if (!entry.isDirectory()) continue;
  const dir = path.join(pluginsRoot, entry.name);
  const files = fs.readdirSync(dir).filter(f => f.endsWith('.json'));
  if (files.length === 0) {
    console.warn(`⚠️  No JSON manifest found in ${dir}`);
    continue;
  }
  for (const f of files) {
    const full = path.join(dir, f);
    try {
      const data = JSON.parse(fs.readFileSync(full, 'utf8'));
      if (data.tools && !Array.isArray(data.tools)) {
        console.error(`❌ ${full}: "tools" should be an array when present.`);
        failures++;
      }
    } catch (e) {
      console.error(`❌ Invalid JSON in ${full}: ${e.message}`);
      failures++;
    }
  }
}

if (failures > 0) {
  console.error(`❌ Plugin validation failed with ${failures} issue(s).`);
  process.exit(1);
}

console.log('✅ Plugin manifests validation passed.');

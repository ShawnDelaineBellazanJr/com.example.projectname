#!/usr/bin/env node

/**
 * Intent Queue Runner
 *
 * Watches a queue directory for *.json intents and executes each using the C# MCP client.
 * Outputs results to ./out and archives processed intents.
 */

const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');

const projectRoot = path.join(__dirname, '..');
const queueDir = path.join(projectRoot, 'intents', 'queue');
const outDir = path.join(projectRoot, 'out');
const archiveDir = path.join(queueDir, 'archive');
const clientCsproj = path.join(projectRoot, 'src', 'ProjectName.McpClient', 'ProjectName.McpClient.csproj');

function ensureDirs() {
  [queueDir, outDir, archiveDir].forEach((d) => {
    if (!fs.existsSync(d)) fs.mkdirSync(d, { recursive: true });
  });
}

function listIntentFiles() {
  if (!fs.existsSync(queueDir)) return [];
  return fs.readdirSync(queueDir)
    .filter((f) => f.toLowerCase().endsWith('.json'))
    .map((f) => path.join(queueDir, f));
}

function runIntentConfig(configPath) {
  return new Promise((resolve) => {
    const child = spawn('dotnet', [
      'run',
      '--project', clientCsproj,
      '--configuration', 'Release',
      '--no-build',
      '--',
      '--config', configPath
    ], { cwd: projectRoot, stdio: 'pipe' });

    let output = '';
    let errorOutput = '';

    child.stdout.on('data', (d) => (output += d.toString()))
    child.stderr.on('data', (d) => (errorOutput += d.toString()))

    child.on('close', (code) => {
      resolve({ code, output, errorOutput });
    });
  });
}

async function processIntentFile(file) {
  const base = path.basename(file);
  console.log(`▶ Processing intent: ${base}`);
  const result = await runIntentConfig(file);
  const ts = new Date().toISOString().replace(/[:.]/g, '-');
  const logFile = path.join(outDir, `${base}.${ts}.log.txt`);
  fs.writeFileSync(logFile, `${result.output}\n${result.errorOutput}`);
  const dest = path.join(archiveDir, `${base}.${ts}`);
  fs.renameSync(file, dest);
  const status = result.code === 0 ? 'OK' : `ERR(${result.code})`;
  console.log(`✓ ${base} -> ${status}; log: ${path.relative(projectRoot, logFile)}`);
}

async function runOnce() {
  ensureDirs();
  const files = listIntentFiles();
  for (const f of files) {
    await processIntentFile(f);
  }
}

function watchQueue() {
  ensureDirs();
  console.log(`👀 Watching ${path.relative(projectRoot, queueDir)} for intents (*.json)...`);
  fs.watch(queueDir, { persistent: true }, async (_event, _fname) => {
    // Debounce: process all at once
    setTimeout(runOnce, 200);
  });
}

(async () => {
  const mode = process.argv[2] || 'once';
  if (mode === 'watch') {
    await runOnce();
    watchQueue();
  } else {
    await runOnce();
  }
})();

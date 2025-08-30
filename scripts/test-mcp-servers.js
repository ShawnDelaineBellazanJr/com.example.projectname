#!/usr/bin/env node

/**
 * MCP Server Test Suite for ThoughtTransfer Framework
 * Tests all configured MCP servers and generates comprehensive report
 */

const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

class MCPTester {
    constructor() {
        this.mcpConfig = require('../.vscode/mcp.json');
        this.results = {
            timestamp: new Date().toISOString(),
            servers: {},
            summary: {
                total: 0,
                working: 0,
                failed: 0,
                skipped: 0
            }
        };
    }

    async testAllServers() {
        console.log('🧪 Starting MCP Server Test Suite...\n');

        const servers = Object.keys(this.mcpConfig.servers);
        this.results.summary.total = servers.length;

        for (const serverName of servers) {
            console.log(`🔍 Testing ${serverName}...`);
            const result = await this.testServer(serverName);
            this.results.servers[serverName] = result;

            if (result.status === 'working') {
                this.results.summary.working++;
                console.log(`✅ ${serverName}: ${result.message}\n`);
            } else if (result.status === 'failed') {
                this.results.summary.failed++;
                console.log(`❌ ${serverName}: ${result.message}\n`);
            } else {
                this.results.summary.skipped++;
                console.log(`⏭️  ${serverName}: ${result.message}\n`);
            }
        }

        this.generateReport();
        this.checkMissingComponents();
    }

    async testServer(serverName) {
        const serverConfig = this.mcpConfig.servers[serverName];

        try {
            // Test if the package can be executed
            const result = await this.runCommand('npx', ['--help'], { timeout: 10000 });

            if (!result.success) {
                return {
                    status: 'failed',
                    message: 'npx not available',
                    error: result.error
                };
            }

            // Test the specific server package
            const testArgs = [...serverConfig.args];
            if (serverConfig.args.includes('${workspaceFolder}')) {
                testArgs[testArgs.indexOf('${workspaceFolder}')] = process.cwd();
            }

            // Replace environment variable placeholders
            const processedArgs = testArgs.map(arg => {
                if (arg.includes('${env:')) {
                    const envVar = arg.match(/\$\{env:([^}]+)\}/)?.[1];
                    return process.env[envVar] || arg;
                }
                return arg;
            });

            const serverResult = await this.runCommand('npx', processedArgs, { timeout: 15000 });

            if (serverResult.success) {
                return {
                    status: 'working',
                    message: 'Server initialized successfully',
                    tools: serverResult.stdout ? 'Tools available' : 'No tools detected'
                };
            } else {
                // Check if it's a missing dependency vs actual failure
                if (serverResult.error.includes('Cannot find package') ||
                    serverResult.error.includes('not found')) {
                    return {
                        status: 'skipped',
                        message: 'Package not installed - run npm install to enable',
                        error: serverResult.error
                    };
                } else if (serverResult.error.includes('API key') ||
                          serverResult.error.includes('token') ||
                          serverResult.error.includes('auth')) {
                    return {
                        status: 'skipped',
                        message: 'Requires API key/token configuration',
                        error: serverResult.error
                    };
                } else {
                    return {
                        status: 'failed',
                        message: 'Server failed to start',
                        error: serverResult.error
                    };
                }
            }

        } catch (error) {
            return {
                status: 'failed',
                message: 'Test execution failed',
                error: error.message
            };
        }
    }

    runCommand(command, args, options = {}) {
        return new Promise((resolve) => {
            const child = spawn(command, args, {
                stdio: ['pipe', 'pipe', 'pipe'],
                ...options
            });

            let stdout = '';
            let stderr = '';

            child.stdout.on('data', (data) => {
                stdout += data.toString();
            });

            child.stderr.on('data', (data) => {
                stderr += data.toString();
            });

            const timeout = setTimeout(() => {
                child.kill();
                resolve({
                    success: false,
                    error: 'Command timed out'
                });
            }, options.timeout || 30000);

            child.on('close', (code) => {
                clearTimeout(timeout);
                resolve({
                    success: code === 0,
                    stdout,
                    stderr,
                    error: stderr || (code !== 0 ? `Exit code: ${code}` : null)
                });
            });

            child.on('error', (error) => {
                clearTimeout(timeout);
                resolve({
                    success: false,
                    error: error.message
                });
            });
        });
    }

    generateReport() {
        console.log('\n📊 MCP Server Test Results Summary:');
        console.log('=' .repeat(50));
        console.log(`Total Servers: ${this.results.summary.total}`);
        console.log(`✅ Working: ${this.results.summary.working}`);
        console.log(`❌ Failed: ${this.results.summary.failed}`);
        console.log(`⏭️  Skipped: ${this.results.summary.skipped}`);
        console.log('=' .repeat(50));

        // Save detailed report
        const reportPath = path.join(process.cwd(), 'docs', 'reports', 'mcp-test-report.json');
        fs.writeFileSync(reportPath, JSON.stringify(this.results, null, 2));
        console.log(`\n📄 Detailed report saved to: ${reportPath}`);
    }

    checkMissingComponents() {
        console.log('\n🔍 Missing Components Analysis:');

        const missing = [];

        // Check for Ollama
        if (!this.results.servers.ollama) {
            missing.push({
                component: 'Ollama MCP Server',
                description: 'Local AI model integration',
                setup: 'Install Ollama and add MCP server configuration'
            });
        }

        // Check for environment variables
        const envVars = [
            'GITHUB_TOKEN',
            'SUPABASE_ACCESS_TOKEN',
            'HUBSPOT_ACCESS_TOKEN',
            'NOTION_API_KEY',
            'YOUTUBE_API_KEY'
        ];

        envVars.forEach(envVar => {
            if (!process.env[envVar]) {
                missing.push({
                    component: `${envVar} Environment Variable`,
                    description: `Required for ${envVar.split('_')[0].toLowerCase()} integration`,
                    setup: 'Add to .env file or system environment variables'
                });
            }
        });

        if (missing.length > 0) {
            console.log('\n⚠️  Missing Components:');
            missing.forEach((item, index) => {
                console.log(`${index + 1}. ${item.component}`);
                console.log(`   ${item.description}`);
                console.log(`   Setup: ${item.setup}\n`);
            });
        } else {
            console.log('✅ All required components are configured!');
        }

        // Recommendations
        console.log('\n💡 Recommendations:');
        console.log('1. Install missing MCP server packages: npm install');
        console.log('2. Configure required API keys in .env file');
        console.log('3. Add Ollama MCP server for local AI integration');
        console.log('4. Test individual servers with specific use cases');
        console.log('5. Monitor server performance and resource usage');
    }
}

// Run the test suite
if (require.main === module) {
    const tester = new MCPTester();
    tester.testAllServers().catch(console.error);
}

module.exports = MCPTester;

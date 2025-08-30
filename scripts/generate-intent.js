#!/usr/bin/env node

/**
 * Intent Generator
 *
 * Generates intent JSON files from high-level descriptions using templates.
 * Follows PMCR-O principles for declarative, template-driven intent creation.
 */

const fs = require('fs');
const path = require('path');

class IntentGenerator {
  constructor() {
    this.projectRoot = path.join(__dirname, '..');
    this.templatesDir = path.join(this.projectRoot, 'intents');
    this.templateFile = path.join(this.templatesDir, 'template.intent.json');
  }

  /**
   * Generate intent from description
   * @param {string} description - High-level intent description
   * @param {string} name - Intent name (used in output file)
   * @param {number} steps - Number of steps to generate
   */
  generateIntent(description, name, steps = 8) {
    console.log(`🎯 Generating intent for: ${description}`);
    console.log(`📝 Intent name: ${name}`);
    console.log(`🔢 Steps: ${steps}\n`);

    // Read template
    const template = fs.readFileSync(this.templateFile, 'utf8');

    // Generate calls array
    const calls = this.generateCalls(description, steps);

    // Replace placeholders
    let intentJson = template
      .replace('{{calls}}', calls)
      .replace('{{name}}', name);

    // Validate JSON
    try {
      JSON.parse(intentJson);
      console.log('✅ Generated intent JSON is valid');
    } catch (error) {
      console.error('❌ Generated intent JSON is invalid:', error.message);
      throw error;
    }

    // Write to file
    const outputPath = path.join(this.templatesDir, `intent.${name}.json`);
    fs.writeFileSync(outputPath, intentJson);

    console.log(`💾 Intent saved to: ${outputPath}`);

    return {
      path: outputPath,
      content: intentJson
    };
  }

  /**
   * Generate calls array for the intent
   */
  generateCalls(description, steps) {
    const calls = [];

    // Generic app development steps template
    const stepTemplates = [
      "Analyze {{description}} market opportunities and AI integration potential",
      "Research current {{description}} industry trends, competitor analysis, and customer pain points",
      "Design AI-powered features: automated workflows, intelligent recommendations, user personalization",
      "Define technical architecture: modern web/mobile stack, cloud infrastructure, database design",
      "Create comprehensive data models and API specifications for {{description}} functionality",
      "Develop user experience wireframes and interaction flows for {{description}}",
      "Implement security measures, authentication, and data protection for {{description}}",
      "Plan deployment strategy, scalability considerations, and monitoring for {{description}}"
    ];

    for (let i = 0; i < Math.min(steps, stepTemplates.length); i++) {
      const thought = stepTemplates[i].replace(/\{\{description\}\}/g, description);

      calls.push({
        tool: "echo",
        params: {
          message: thought
        }
      });
    }

    // Convert to JSON string with proper formatting for array insertion
    return calls.map(call => JSON.stringify(call, null, 2)).join(',\n    ');
  }
}

// CLI interface
if (require.main === module) {
  const args = process.argv.slice(2);

  if (args.length < 2) {
    console.log('Usage: node generate-intent.js <description> <name> [steps]');
    console.log('Example: node generate-intent.js "landscaping marketplace app" "chop-chop-landscaping" 8');
    process.exit(1);
  }

  const description = args[0];
  const name = args[1];
  const steps = args[2] ? parseInt(args[2]) : 8;

  const generator = new IntentGenerator();

  try {
    const result = generator.generateIntent(description, name, steps);
    console.log('\n🎉 Intent generation completed successfully!');
    console.log(`📄 File: ${result.path}`);
  } catch (error) {
    console.error('❌ Intent generation failed:', error.message);
    process.exit(1);
  }
}

module.exports = IntentGenerator;

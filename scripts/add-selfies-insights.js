const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('./mydatabase.db');

db.serialize(() => {
  // Insert self-referential insight about SELFIES integration
  db.run(`INSERT INTO business_insights
    (category, insight, priority_level, source)
    VALUES (?, ?, ?, ?)`,
    ['Technology', 'SELFIES molecular encoding provides perfect self-reference model for documentation evolution', 'High', 'docs/meta/selfies-integration.md']);

  // Insert AI opportunity related to self-reference
  db.run(`INSERT INTO ai_opportunities
    (opportunity_name, description, priority, estimated_value)
    VALUES (?, ?, ?, ?)`,
    ['Self-Referential Documentation AI', 'AI system that generates and validates self-referential documentation using SELFIES principles', 'High', '$500K+']);

  console.log('✅ Added self-referential insights to database');
});

db.close();

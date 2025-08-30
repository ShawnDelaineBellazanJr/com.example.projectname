const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('./mydatabase.db');

db.serialize(() => {
  console.log('Checking database schema...');

  db.all("PRAGMA table_info(business_insights)", (err, rows) => {
    if (err) {
      console.error('Error checking business_insights:', err);
    } else {
      console.log('business_insights schema:');
      console.table(rows);
    }

    db.all("PRAGMA table_info(ai_opportunities)", (err, rows) => {
      if (err) {
        console.error('Error checking ai_opportunities:', err);
      } else {
        console.log('ai_opportunities schema:');
        console.table(rows);
      }
      db.close();
    });
  });
});

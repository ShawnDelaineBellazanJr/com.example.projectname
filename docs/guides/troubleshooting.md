# Troubleshooting Guide

## Overview

This guide provides solutions for common issues encountered when working with the self-referential documentation system, including assessment problems, evolution trigger issues, and build failures.

## Assessment Issues

### Assessment Scores Not Updating

**Problem**: Assessment scripts run but scores don't change
**Symptoms**:
- Assessment reports show same scores repeatedly
- No new suggestions generated
- Evolution triggers not firing

**Solutions**:
1. **Check Assessment Configuration**
   ```bash
   # Verify assessment script is using correct metrics
   npm run self-assess --verbose
   ```

2. **Validate Document Structure**
   ```bash
   # Check for required self-assessment sections
   npm run validate-structure
   ```

3. **Review Assessment Criteria**
   - Ensure documents have proper self-assessment sections
   - Verify evolution triggers are present and valid
   - Check cross-references are working

### Inaccurate Assessment Results

**Problem**: Assessment scores don't match document quality
**Symptoms**:
- High scores for poor quality content
- Low scores for excellent content
- Inconsistent scoring across similar documents

**Solutions**:
1. **Calibrate Assessment Weights**
   ```javascript
   // Adjust metric weights in assessment script
   const weights = {
     completeness: 0.3,  // Adjust based on needs
     accuracy: 0.3,
     relevance: 0.2,
     quality: 0.2
   };
   ```

2. **Review Assessment Criteria**
   - Check if criteria match your quality standards
   - Update scoring algorithms if needed
   - Validate against known good examples

## Evolution Trigger Problems

### Triggers Not Firing

**Problem**: Evolution triggers exist but don't execute
**Symptoms**:
- Conditions are met but no actions taken
- Evolution reports show no trigger activity
- System health doesn't improve

**Solutions**:
1. **Check Trigger Conditions**
   ```javascript
   // Verify trigger conditions are properly defined
   const trigger = {
     condition: (doc) => doc.score < 70,  // Ensure this evaluates correctly
     action: (doc) => improveDocument(doc)
   };
   ```

2. **Validate Trigger Priority**
   - High priority triggers should fire before medium/low
   - Check for conflicting triggers
   - Ensure trigger scope is appropriate

3. **Monitor Trigger Execution**
   ```bash
   # Run evolution system with detailed logging
   npm run evolution-triggers --debug
   ```

### Conflicting Evolution Actions

**Problem**: Multiple triggers trying to modify same content
**Symptoms**:
- Inconsistent document updates
- Evolution conflicts in logs
- Quality degradation instead of improvement

**Solutions**:
1. **Implement Trigger Prioritization**
   ```javascript
   const triggers = [
     { name: 'critical_fixes', priority: 'high' },
     { name: 'quality_improvements', priority: 'medium' },
     { name: 'engagement_boosts', priority: 'low' }
   ];
   ```

2. **Add Conflict Resolution**
   - Define clear trigger scopes
   - Implement sequential execution
   - Add manual override capabilities

## Build and Deployment Issues

### DocFX Build Failures

**Problem**: Documentation build fails with errors
**Symptoms**:
- Build process stops with error messages
- Missing output files
- Broken links in generated site

**Solutions**:
1. **Check File Structure**
   ```bash
   # Validate all required files exist
   find docs/ -name "*.md" | wc -l
   ```

2. **Fix Broken References**
   ```bash
   # Check for broken links in TOC
   npm run validate-structure
   ```

3. **Update DocFX Configuration**
   ```json
   {
     "build": {
       "content": [
         {
           "files": ["**/*.md"],
           "src": "docs",
           "dest": "docs"
         }
       ]
     }
   }
   ```

### Site Generation Problems

**Problem**: Site builds but has display issues
**Symptoms**:
- Navigation not working properly
- Search functionality broken
- Styling issues in generated HTML

**Solutions**:
1. **Clear Build Cache**
   ```bash
   # Remove old build artifacts
   rm -rf _site/
   rm -rf obj/
   ```

2. **Update Dependencies**
   ```bash
   # Ensure DocFX and dependencies are current
   dotnet tool update docfx
   ```

3. **Check Template Configuration**
   - Verify custom templates are properly configured
   - Check for template syntax errors
   - Validate CSS and JavaScript includes

## Performance Issues

### Slow Assessment Execution

**Problem**: Assessment scripts take too long to run
**Symptoms**:
- Long execution times for assessment
- System becomes unresponsive during assessment
- Timeout errors in automation

**Solutions**:
1. **Implement Caching**
   ```javascript
   // Cache assessment results
   const cache = new Map();
   if (cache.has(documentPath)) {
     return cache.get(documentPath);
   }
   ```

2. **Optimize Assessment Logic**
   - Use parallel processing for multiple documents
   - Implement incremental assessment for changed files only
   - Add early termination for obvious cases

3. **Schedule During Low Usage**
   ```bash
   # Run assessments during off-peak hours
   0 2 * * * npm run self-assess  # Daily at 2 AM
   ```

### Memory Usage Problems

**Problem**: System runs out of memory during large assessments
**Symptoms**:
- Out of memory errors
- System slowdowns
- Assessment process crashes

**Solutions**:
1. **Implement Streaming Processing**
   ```javascript
   // Process documents in batches
   const batchSize = 10;
   for (let i = 0; i < documents.length; i += batchSize) {
     const batch = documents.slice(i, i + batchSize);
     await processBatch(batch);
   }
   ```

2. **Add Memory Monitoring**
   - Monitor memory usage during assessment
   - Implement garbage collection hints
   - Add memory limits and cleanup

## Integration Issues

### Version Control Conflicts

**Problem**: Assessment and evolution conflict with version control
**Symptoms**:
- Merge conflicts with assessment-generated content
- Evolution changes overwrite manual edits
- Version control history becomes cluttered

**Solutions**:
1. **Separate Assessment Branches**
   ```bash
   # Use separate branch for assessment changes
   git checkout -b assessment-updates
   npm run self-assess
   git commit -m "Assessment updates"
   ```

2. **Implement Change Tracking**
   - Track which changes are automated vs manual
   - Use commit messages to distinguish change types
   - Implement approval workflows for automated changes

### External Tool Integration

**Problem**: Integration with external tools fails
**Symptoms**:
- API calls to external services fail
- Data synchronization issues
- Authentication problems

**Solutions**:
1. **Check API Configuration**
   ```javascript
   const config = {
     api: {
       endpoint: 'https://api.example.com',
       timeout: 30000,
       retries: 3
     }
   };
   ```

2. **Implement Error Handling**
   - Add retry logic for failed requests
   - Implement fallback mechanisms
   - Log detailed error information

## PMCR-O Loop Execution
### Windows: Python runner not found

Problem: Running Python code in the code runner fails with "Python was not found; run without arguments to install from the Microsoft Store, or disable this shortcut from Settings > Apps > Advanced app settings > App execution aliases."

Fixes:
- Install Python 3.11+ from python.org, and ensure "Add python.exe to PATH" is checked.
- Or disable the Windows Store App Execution Alias: Settings > Apps > Advanced app settings > App execution aliases > Turn off Python aliases.
- After installing, open a new terminal so PATH updates are applied.


## Meta-Commentary
Troubleshooting is iterative; these sections align fixes with the self-evolution system.

## Self-Assessment

**Completeness**: 85% - Common issues covered with practical solutions
**Accuracy**: 90% - Based on real-world troubleshooting experience
**Relevance**: 95% - Addresses actual user pain points
**Quality**: 85% - Clear problem/solution format, actionable advice

**Overall Score**: 89/100

## Evolution Triggers

- If new issues emerge: Add corresponding troubleshooting sections
- If solutions become outdated: Update with current best practices
- If user questions increase: Expand FAQ-style content
- If system complexity grows: Add advanced troubleshooting techniques

## Cross-References

- [Getting Started](getting-started.md) - Initial setup and basic usage
- [Best Practices](best-practices.md) - Preventive maintenance guidelines
- [Evolution Mechanisms](../evolution/mechanisms.md) - How the system handles issues
- [Quality Assurance](../evolution/quality-assurance.md) - Quality monitoring and issues

---

*This troubleshooting guide embodies self-reference by documenting solutions to its own potential issues.*

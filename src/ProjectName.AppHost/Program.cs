// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: This AppHost orchestrates Semantic Kernel + MCP + Ollama distributed AI system

var builder = DistributedApplication.CreateBuilder(args);

// Add Ollama container for local AI inference
var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama-api")
    .WithVolume("ollama-data", "/root/.ollama")
    .WithEnvironment("OLLAMA_HOST", "0.0.0.0");

// Add Redis for state management and caching
var redis = builder.AddRedis("redis")
    .WithRedisCommander();

// Add PostgreSQL for persistent storage
var postgresServer = builder.AddPostgres("postgres");
var postgres = postgresServer.AddDatabase("thoughttransfer");

// Add the main Quantum MCP Service with Semantic Kernel integration
var quantumMcpService = builder.AddProject<Projects.ProjectName_QuantumMcpService>("quantum-mcp-service")
    .WithReference(redis)
    .WithReference(postgres)
    .WithEnvironment("OLLAMA_ENDPOINT", ollama.GetEndpoint("ollama-api"))
    .WithEnvironment("OLLAMA_ENDPOINT", ollama.GetEndpoint("ollama-api"))
    .WithExternalHttpEndpoints();

// Start autonomous PMCR-O loop
builder.Build().Run();

// Copyright (c) 2025 ThoughtTransfer AI System. All rights reserved.
// Licensed under the MIT License.
// Self-Evolution: This AppHost integrates Semantic Kernel Agent Framework with MCP and .NET Aspire

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add Ollama for local AI inference
var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama-api")
    .WithVolume("ollama-data", "/root/.ollama")
    .WithEnvironment("OLLAMA_HOST", "0.0.0.0")
    .WithArgs("serve");

// Add Redis for state management
var redis = builder.AddRedis("redis")
    .WithRedisCommander();

// Add PostgreSQL for persistent storage
var postgres = builder.AddPostgres("postgres")
    .WithDatabase("thoughttransfer");

// Add the backend service with all dependencies
var backend = builder.AddProject<Projects.AspireSemanticKernelMcp_Backend>("backend")
    .WithReference(ollama)
    .WithReference(redis)
    .WithReference(postgres)
    .WithEnvironment("OLLAMA_ENDPOINT", ollama.GetEndpoint("ollama-api"))
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

// Add a simple frontend (optional)
var frontend = builder.AddNpmApp("frontend", "../frontend")
    .WithReference(backend)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ThreeP>("threep");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var sqlite = builder.AddSqlite("sqlite"); ;
builder.AddProject<Projects.ThreeP>("threep").WithReference(sqlite);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithLifetime(ContainerLifetime.Persistent);
                     });

var tableStorage = storage.AddTables("tables");

//var apiService = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice");

var apiService = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice")
    .WithReference(tableStorage);

builder.AddProject<Projects.AspireStarter_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("compose");

var openAi = builder.AddOpenAI("openai");

builder.AddProject<Projects.ItDepends_API>("itdepends-api")
	.WithReference(openAi);


builder.Build().Run();
using System;
using System.IO;
using System.Linq;
using Faithlife.Build;
using static Faithlife.Build.BuildUtility;
using static Faithlife.Build.DotNetRunner;

internal static class Build
{
	public static int Main(string[] args) => BuildRunner.Execute(args, build =>
	{
		var codegen = "fsdgenswagger";

		var dotNetBuildSettings = new DotNetBuildSettings
		{
			NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY"),
			DocsSettings = new DotNetDocsSettings
			{
				GitLogin = new GitLoginInfo("FacilityApiBot", Environment.GetEnvironmentVariable("BUILD_BOT_PASSWORD") ?? ""),
				GitAuthor = new GitAuthorInfo("FacilityApiBot", "facilityapi@gmail.com"),
				SourceCodeUrl = "https://github.com/FacilityApi/FacilitySwagger/tree/master/src",
				ProjectHasDocs = name => !name.StartsWith("fsdgen", StringComparison.Ordinal),
			},
		};

		build.AddDotNetTargets(dotNetBuildSettings);

		build.Target("codegen")
			.DependsOn("build")
			.Describe("Generates code from the FSD")
			.Does(() => CodeGen(verify: false));

		build.Target("verify-codegen")
			.DependsOn("build")
			.Describe("Ensures the generated code is up-to-date")
			.Does(() => CodeGen(verify: true));

		build.Target("test")
			.DependsOn("verify-codegen");

		void CodeGen(bool verify)
		{
			var configuration = dotNetBuildSettings!.BuildOptions!.ConfigurationOption!.Value;
			var toolPath = FindFiles($"src/{codegen}/bin/{configuration}/netcoreapp3.1/{codegen}.dll").First();

			var verifyOption = verify ? "--verify" : null;

			RunDotNet(toolPath, "example/ExampleApi.fsd", "example/output/swagger", "--json", "--newline", "lf", verifyOption);
			RunDotNet(toolPath, "example/ExampleApi.fsd", "example/output/swagger", "--newline", "lf", verifyOption);
			RunDotNet(toolPath, "example/output/swagger/ExampleApi.json", "example/output/swagger/fsd", "--fsd", "--newline", "lf", verifyOption);
			if (verify)
				RunDotNet(toolPath, "example/output/swagger/ExampleApi.yaml", "example/output/swagger/fsd", "--fsd", "--newline", "lf", verifyOption);

			foreach (var yamlPath in FindFiles("example/*.yaml"))
				RunDotNet(toolPath, yamlPath, "example/output/fsd", "--fsd", "--newline", "lf", verifyOption);

			Directory.CreateDirectory("example/output/fsd/swagger");
			foreach (var fsdPath in FindFiles("example/output/fsd/*.fsd"))
				RunDotNet(toolPath, fsdPath, $"example/output/fsd/swagger/{Path.GetFileNameWithoutExtension(fsdPath)}.yaml", "--newline", "lf", verifyOption);
		}
	});
}

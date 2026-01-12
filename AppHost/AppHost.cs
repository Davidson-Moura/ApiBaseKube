using System.Diagnostics;

var builder = DistributedApplication.CreateBuilder(args);

var k8s = builder.AddKubernetesEnvironment("k8s-env");

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cache);

#if DEBUG

var frontend = builder.AddExecutable(
    name: "frontend",
    command: "npm",
    args: ["run", "dev"],
    workingDirectory: "../Web/FrontEnd"
);
#else
RunNpmBuild();
#endif

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    ;

builder.Build().Run();

static void RunNpmBuild()
{
    var psi = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = "/c npm run build",
        WorkingDirectory = "../Web/FrontEnd",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };

    using var process = Process.Start(psi)!;

    process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
    process.ErrorDataReceived += (_, e) => Console.Error.WriteLine(e.Data);

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    process.WaitForExit();

    if (process.ExitCode != 0) throw new Exception("Erro ao executar npm build");
}

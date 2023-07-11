using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SqlAnalysis.Features.ProfilerReferences;
using SqlAnalysis.Features.SqlFileReferences;

namespace SqlAnalysis
{
    public static class Program
    {
        private static IConfiguration? s_configuration;
        private static IServiceCollection? s_serviceCollection;
        private static IServiceProvider? s_serviceProvider;

        public static void Main(string[] args)
        {
            BuildConfiguration();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(s_configuration!)
                .CreateLogger();
            ConfigureServices();

            ParseCommandLineOptions(args);
        }

        private static void ParseCommandLineOptions(string[] args)
        {
            Parser.Default
                .ParseArguments<
                    ProfilerReferencesOptions,
                    SqlFileReferencesOptions
                >(args)
                .WithParsed<ProfilerReferencesOptions>(options =>
                {
                    var verb = s_serviceProvider?.GetService<ProfilerReferencesVerb>();
                    verb?.Run(options).Wait();
                })
                .WithParsed<SqlFileReferencesOptions>(options =>
                {
                    var verb = s_serviceProvider?.GetService<SqlFileReferencesVerb>();
                    verb?.Run(options).Wait();
                })
                ;
        }

        private static void ConfigureServices()
        {
            s_serviceCollection = new ServiceCollection();

            // var appSettings = new AppSettings();
            // s_configuration.Bind("Settings", appSettings);

            s_serviceCollection.AddLogging(configure => configure.AddSerilog());

            s_serviceCollection.AddSqlParsingTools();
            s_serviceCollection.AddVerbs();

            s_serviceProvider = s_serviceCollection.BuildServiceProvider();
        }

        private static void BuildConfiguration()
        {
            ConfigurationBuilder configuration = new();
            var environmentName = GetEnvironmentName();

            s_configuration = configuration.AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static string GetEnvironmentName()
        {
            var environmentName = GetRawEnvironmentName();
            return environmentName switch
            {
                "dev" => "development",
                "test" => "development",
                "uat" => "development",
                "prod" => "production",
                _ => "Local"
            };
        }

        private static string GetRawEnvironmentName()
        {
            var environmentName = Environment.GetEnvironmentVariable("env") ?? "Local";
            return environmentName.Trim().ToLower();
        }
    }
}
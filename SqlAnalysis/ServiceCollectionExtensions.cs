using Microsoft.Extensions.DependencyInjection;
using SqlAnalysis.Features.Profiler;
using SqlAnalysis.Services;

namespace SqlAnalysis
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVerbs(this IServiceCollection? serviceCollection)
        {
            serviceCollection?
                .AddTransient<ProfilerVerb>()
                ;
        }
        
        public static void AddSqlParsingTools(this IServiceCollection? serviceCollection)
        {
            serviceCollection?
                .AddTransient<ISqlParser, SqlParser>()
                ;
        }
    }
}
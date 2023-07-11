using Microsoft.Extensions.DependencyInjection;
using SqlAnalysis.Features.ProfilerReferences;
using SqlAnalysis.Features.SqlFileReferences;
using SqlAnalysis.Services;

namespace SqlAnalysis
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVerbs(this IServiceCollection? serviceCollection)
        {
            serviceCollection?
                .AddTransient<ProfilerReferencesVerb>()
                .AddTransient<SqlFileReferencesVerb>()
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
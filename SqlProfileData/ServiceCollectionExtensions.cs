using Microsoft.Extensions.DependencyInjection;
using SqlAnalysis.Features.Profiler;

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
    }
}
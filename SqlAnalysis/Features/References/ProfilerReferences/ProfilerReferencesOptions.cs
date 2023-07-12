using CommandLine;

namespace SqlAnalysis.Features.References.ProfilerReferences
{
    [Verb("ProfilerReferences", HelpText = "Read profiler data from database")]
    public class ProfilerReferencesOptions : BaseReferenceOptions
    {
        [Option('t', nameof(TableName), Required = true, HelpText = "Table where queries are recorded")]
        public string TableName { get; set; } = null!;

        //[Option('s', nameof(StartTime), Required = false, HelpText = "Only get recorded queries after to this point")]
        //public DateTime? StartTime { get; set; }

        //[Option('e', nameof(EndTime), Required = false, HelpText = "Only get recorded queries prior to this point")]
        //public DateTime? EndTime { get; set; }
    }
}
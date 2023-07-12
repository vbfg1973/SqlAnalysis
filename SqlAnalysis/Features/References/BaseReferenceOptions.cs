using CommandLine;

namespace SqlAnalysis.Features.References
{
    public abstract class BaseReferenceOptions
    {
        [Option('o', nameof(OutputCsv), Required = true, HelpText = "CSV file to write output to")]
        public string OutputCsv { get; set; } = null!;
    }
}
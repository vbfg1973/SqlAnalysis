using CommandLine;

namespace SqlAnalysis.Features.References.SqlFileReferences
{
    [Verb("SqlFileReferences", HelpText = "Extracts tables and stored procedures referred to by a SQL file")]
    public class SqlFileReferencesOptions : BaseReferenceOptions
    {
        [Option('p', nameof(Path), Required = true, HelpText = "Path to SQL file")]
        public string Path { get; set; } = null!;
    }
}
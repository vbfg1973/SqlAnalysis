using CommandLine;

namespace SqlAnalysis.Features.References.SqlFileReferences
{
    [Verb("SqlFileReferences", HelpText = "Extracts tables and stored procedures referred to by a SQL file(s)")]
    public class SqlFileReferencesOptions : BaseReferenceOptions
    {
        [Option('f', nameof(File), HelpText = "Path to SQL file")]
        public string File { get; set; } = null!;

        [Option('d', nameof(Dir), HelpText = "Path to directory structure containing SQL files")]
        public string Dir { get; set; } = null!;
    }
}
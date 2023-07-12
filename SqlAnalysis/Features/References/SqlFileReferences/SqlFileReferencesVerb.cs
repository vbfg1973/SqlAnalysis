using Microsoft.Extensions.Logging;
using SqlAnalysis.Helpers;
using SqlAnalysis.Services;

namespace SqlAnalysis.Features.References.SqlFileReferences
{
    public class SqlFileReferencesVerb
    {
        private readonly ILogger<SqlFileReferencesVerb> _logger;
        private readonly ISqlParser _sqlParser;

        public SqlFileReferencesVerb(ISqlParser sqlParser, ILogger<SqlFileReferencesVerb> logger)
        {
            _sqlParser = sqlParser;
            _logger = logger;
        }

        public async Task Run(SqlFileReferencesOptions referencesOptions)
        {
            List<ReferenceDto> discoveredReferences = new();
            if (!string.IsNullOrEmpty(referencesOptions.File))
            {
                if (!File.Exists(referencesOptions.File))
                {
                    throw new ArgumentException("File does not exist", referencesOptions.File);
                }


                discoveredReferences.AddRange(await GetReferencesFromFile(referencesOptions.File));
            }

            else if (!string.IsNullOrEmpty(referencesOptions.Dir))
            {
                if (!Directory.Exists(referencesOptions.Dir))
                {
                    throw new ArgumentException("Directory does not exist", referencesOptions.Dir);
                }


                discoveredReferences.AddRange(await GetReferencesFromDir(referencesOptions.Dir));
            }

            else
            {
                throw new ArgumentException("Must specify either file or directory");
            }

            CsvHelpers.Write(referencesOptions.OutputCsv, discoveredReferences);
            await Console.Error.WriteLineAsync("The Id column refers to the file the reference was extracted from");
        }

        private async Task<List<ReferenceDto>> GetReferencesFromDir(string dirPath)
        {
            List<ReferenceDto> references = new();

            var files = Directory.EnumerateFiles(dirPath, "*.sql", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                await Console.Error.WriteLineAsync(file);
                references.AddRange(await GetReferencesFromFile(file));
            }

            return references;
        }

        private async Task<List<ReferenceDto>> GetReferencesFromFile(string filePath)
        {
            var sqlText = await File.ReadAllTextAsync(filePath);
            return GetReferences(sqlText, filePath);
        }

        private List<ReferenceDto> GetReferences(string sqlText, string identifier)
        {
            var discoveredReferences = new List<ReferenceDto>();

            discoveredReferences.AddRange(
                _sqlParser
                    .StoredProcedureNames(sqlText)
                    .Select(sp => new ReferenceDto
                    {
                        Id = identifier,
                        Name = sp,
                        ReferenceType = ReferenceType.StoredProcedure
                    })
            );


            discoveredReferences.AddRange(
                _sqlParser
                    .TableNames(sqlText)
                    .Select(sp => new ReferenceDto
                    {
                        Id = identifier,
                        Name = sp,
                        ReferenceType = ReferenceType.Table
                    })
            );

            return discoveredReferences;
        }
    }
}
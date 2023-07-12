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
            if (!File.Exists(referencesOptions.Path))
            {
                throw new ArgumentException("File does not exist", referencesOptions.Path);
            }

            var sqlText = await File.ReadAllTextAsync(referencesOptions.Path);
            var discoveredReferences = GetReferences(sqlText, referencesOptions.Path);

            CsvHelpers.Write(referencesOptions.OutputCsv, discoveredReferences);
            await Console.Error.WriteLineAsync($"The Id column refers to the file the reference was extracted from");
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
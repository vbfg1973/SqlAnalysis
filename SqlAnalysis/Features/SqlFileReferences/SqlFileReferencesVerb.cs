using Microsoft.Extensions.Logging;
using SqlAnalysis.Helpers;
using SqlAnalysis.Services;

namespace SqlAnalysis.Features.SqlFileReferences
{
    public class SqlFileReferencesVerb
    {
        private readonly ISqlParser _sqlParser;
        private readonly ILogger<SqlFileReferencesVerb> _logger;

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

            var storedProcedureNames = _sqlParser.StoredProcedureNames(sqlText);
            var tableNames = _sqlParser.TableNames(sqlText);

            if (storedProcedureNames.Any())
            {
                Console.WriteLine(SqlHelpers.NamesToString(sqlText, storedProcedureNames, "Stored Procedure"));
            }

            if (tableNames.Any())
            {
                Console.WriteLine(SqlHelpers.NamesToString(sqlText, tableNames, "Table"));
            }
        }
    }
}
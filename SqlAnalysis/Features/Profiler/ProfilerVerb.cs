using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using SqlAnalysis.Features.Profiler.Models;
using SqlAnalysis.Helpers;

namespace SqlAnalysis.Features.Profiler
{
    public class ProfilerVerb
    {
        private const string ConnectionStringEnvironmentVariable = "SQL_PROFILE_DB";
        private readonly ILogger<ProfilerVerb> _logger;

        public ProfilerVerb(ILogger<ProfilerVerb> logger)
        {
            _logger = logger;
        }

        public async Task Run(ProfilerOptions options)
        {
            try
            {
                var profileData = GetProfileData(options);

                foreach (var row in profileData)
                {
                    var tableNameParser = new ParseTableNames();

                    if (string.IsNullOrEmpty(row.TextData) || row.TextData.StartsWith("exec sp_reset_connection",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }


                    var sqlToParse = GetSqlToParseFromExecuteStmt(row);

                    Console.WriteLine(tableNameParser.ParsedSqlTabledNamesAsString(sqlToParse));
                }
            }

            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
            }
        }

        private string GetSqlToParseFromExecuteStmt(SqlProfileData row)
        {
            string sqlToParse;
            if (row.TextData!.StartsWith("exec sp_executesql", StringComparison.InvariantCultureIgnoreCase))
            {
                var c = "'";

                var firstOffset = row.TextData.IndexOfNth(c);
                var secondOffset = row.TextData.IndexOfNth(c, 1);

                sqlToParse = row.TextData.Substring(firstOffset + 1, secondOffset - firstOffset - 1);
            }

            else
            {
                sqlToParse = row.TextData;
            }

            return sqlToParse;
        }

        private List<SqlProfileData> GetProfileData(ProfilerOptions options)
        {
            var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable);

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("ConnectionString environment variable is not set");
                Environment.Exit(-1);
            }

            using var connection = new SqlConnection(connectionString);

            var sql = $"SELECT * FROM [{options.TableName}]";

            var profileData = connection.Query<SqlProfileData>(sql).ToList();
            return profileData;
        }
    }
}
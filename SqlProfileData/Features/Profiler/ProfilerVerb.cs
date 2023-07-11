using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using SqlAnalysis.Features.Profiler.Models;
using SqlAnalysis.Helpers;

namespace SqlAnalysis.Features.Profiler
{
    public class ProfilerVerb
    {
        private const string _connectionStringEnvironmentVariable = "SQL_PROFILE_DB";
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

                    var tableNames = new List<string>();
                    var sqlToParse = GetSqlToParse(row);

                    tableNames = tableNameParser.GetTableNamesFromQueryString(row.TextData);

                    Console.WriteLine(row.RowNumber);
                    Console.WriteLine(row.TextData);
                    Console.WriteLine(sqlToParse);

                    if (tableNames.Any())
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Table names: {string.Join(",", tableNames)}");
                    }

                    Console.WriteLine();
                }
            }

            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
            }
        }

        private string GetSqlToParse(SqlProfileData row)
        {
            string sqlToParse;
            if (row.TextData.StartsWith("exec sp_executesql", StringComparison.InvariantCultureIgnoreCase))
            {
                var firstOffset = row.TextData.IndexOfNth("'");
                var secondOffset = row.TextData.IndexOfNth("'", 1);

                Console.WriteLine($"Offsets: {firstOffset} - {secondOffset}");

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
            var connectionString = Environment.GetEnvironmentVariable(_connectionStringEnvironmentVariable);

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
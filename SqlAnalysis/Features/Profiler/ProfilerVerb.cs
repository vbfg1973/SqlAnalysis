﻿using System.Data.SqlClient;
using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using SqlAnalysis.Features.Profiler.Models;
using SqlAnalysis.Helpers;
using SqlAnalysis.Services;

namespace SqlAnalysis.Features.Profiler
{
    public class ProfilerVerb
    {
        private const string ConnectionStringEnvironmentVariable = "SQL_PROFILE_DB";
        private readonly ISqlParser _sqlParser;
        private readonly ILogger<ProfilerVerb> _logger;

        public ProfilerVerb(ISqlParser sqlParser, ILogger<ProfilerVerb> logger)
        {
            _sqlParser = sqlParser;
            _logger = logger;
        }

        public async Task Run(ProfilerOptions options)
        {
            try
            {
                var profileData = GetProfileData(options);

                foreach (var row in profileData)
                {
                    if (string.IsNullOrEmpty(row.TextData) || row.TextData.StartsWith("exec sp_reset_connection",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }


                    var sqlToParse = GetSqlToParseFromExecuteStmt(row);

                    var tableNames = _sqlParser.TableNames(sqlToParse);
                    if (tableNames.Any())
                    {
                        Console.WriteLine(NamesToString(sqlToParse, tableNames, "Table"));
                    }

                    var spNames = _sqlParser.StoredProcedureNames(sqlToParse);
                    if (spNames.Any())
                    {
                        Console.WriteLine(NamesToString(sqlToParse, spNames, "Stored Procedure"));
                    }
                }
            }

            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
            }
        }

        private string NamesToString(string sqlToParse, List<string> names, string prefix = "Unknown Prefix")
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append($"SQL: {sqlToParse}");
            strBuilder.AppendLine();
            strBuilder.AppendLine();
            if (!names.Any())
            {
                return strBuilder.ToString();
            }

            foreach (var name in names)
            {
                strBuilder.Append($"{prefix}: {name}\n");
            }

            return strBuilder.ToString();
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
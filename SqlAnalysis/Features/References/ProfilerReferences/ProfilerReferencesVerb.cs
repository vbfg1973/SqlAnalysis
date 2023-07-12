using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using SqlAnalysis.Features.References.ProfilerReferences.Models;
using SqlAnalysis.Helpers;
using SqlAnalysis.Services;

namespace SqlAnalysis.Features.References.ProfilerReferences
{
    public class ProfilerReferencesVerb
    {
        private const string ConnectionStringEnvironmentVariable = "SQL_PROFILE_DB";
        private readonly ILogger<ProfilerReferencesVerb> _logger;
        private readonly ISqlParser _sqlParser;

        public ProfilerReferencesVerb(ISqlParser sqlParser, ILogger<ProfilerReferencesVerb> logger)
        {
            _sqlParser = sqlParser;
            _logger = logger;
        }

        public async Task Run(ProfilerReferencesOptions referencesOptions)
        {
            try
            {
                var profileData = GetProfileData(referencesOptions);

                var discoveredReferences = new List<ReferenceDto>();

                foreach (var row in profileData)
                {
                    if (string.IsNullOrEmpty(row.TextData) || row.TextData.StartsWith("exec sp_reset_connection",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var sqlText = GetSqlToParse(row);

                    discoveredReferences.AddRange(GetReferences(sqlText, row.RowNumber.ToString()));
                }

                CsvHelpers.Write(referencesOptions.OutputCsv, discoveredReferences);
            }

            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
            }
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

        /// <summary>
        ///     Extracts sql to parse. Likely just returns the original string but in the case of an sp_executesql stored procedure
        ///     call (NHibernate and the like) will extract the query
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static string GetSqlToParse(SqlProfileData row)
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

        /// <summary>
        ///     Pulls the sql profiler data back from a database table
        /// </summary>
        /// <param name="referencesOptions"></param>
        /// <returns></returns>
        private List<SqlProfileData> GetProfileData(ProfilerReferencesOptions referencesOptions)
        {
            var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable);

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("ConnectionString environment variable is not set");
                Environment.Exit(-1);
            }

            using var connection = new SqlConnection(connectionString);

            var sql = $"SELECT * FROM [{referencesOptions.TableName}]";

            var profileData = connection.Query<SqlProfileData>(sql).ToList();
            return profileData;
        }
    }
}
using System.Data.SqlClient;
using Dapper;
using SqlProfileAnalysis.Models;

namespace SqlProfileAnalysis
{
    public static class Program
    {
        private static readonly string s_connectionStringEnvironmentVariable = "SQL_PROFILE_DB";

        public static void Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable(s_connectionStringEnvironmentVariable);

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.Error.WriteLine(
                    $"Connection string environment variable ({s_connectionStringEnvironmentVariable}) is not set");
            }

            else
            {
                Console.WriteLine(connectionString);
                using var connection = new SqlConnection(connectionString);

                var sql = "SELECT * FROM [InsiderReportGeneration - 2023-07-11]";

                var profileData = connection.Query<SqlProfileData>(sql).ToList();

                Console.WriteLine(profileData.Count);
            }
        }
    }
}
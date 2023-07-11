using System.Text;

namespace SqlAnalysis.Helpers
{
    public static class SqlHelpers
    {
        public static string NamesToString(string sqlToParse, List<string> names, string prefix = "Unknown Prefix")
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
    }
}
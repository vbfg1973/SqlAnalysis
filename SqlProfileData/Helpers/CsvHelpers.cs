using System.Globalization;
using CsvHelper;

namespace SqlAnalysis.Helpers
{
    public static class CsvHelpers
    {
        public static void Write<T>(string path, IEnumerable<T> records)
        {
            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "O" };
            csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>().Formats = new[] { "O" };

            csv.WriteRecords(records);
        }

        public static IEnumerable<T> Read<T>(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "O" };
            csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>().Formats = new[] { "O" };
            var records = csv.GetRecords<T>().ToList();

            return records;
        }
    }
}
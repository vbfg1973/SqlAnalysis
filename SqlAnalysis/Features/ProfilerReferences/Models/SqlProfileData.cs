namespace SqlAnalysis.Features.ProfilerReferences.Models
{
    public class SqlProfileData
    {
        public int RowNumber { get; set; }
        public int? EventClass { get; set; }
        public string? TextData { get; set; }
        public string? ApplicationName { get; set; }
        public string? NTUserName { get; set; }
        public string? LoginName { get; set; }
        public int? CPU { get; set; }
        public long? Reads { get; set; }
        public long? Writes { get; set; }
        public long? Duration { get; set; }
        public int? ClientProcessID { get; set; }
        public int? SPID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public byte[]? BinaryData { get; set; }
    }
}
﻿namespace SqlAnalysis.Dto
{
    public class ProfileDataDto
    {
        public int? EventClass { get; set; }
        public string? TextData { get; set; }
        public string? ApplicationName { get; set; }
        public string? NTUserName { get; set; }
        public string? LoginName { get; set; }
        public int? CPU { get; set; }
        public long? Reads { get; set; }
        public long? Writes { get; set; }
        public long? Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
namespace SqlAnalysis.Features.References
{
    public enum ReferenceType
    {
        Table,
        StoredProcedure
    }

    public class ReferenceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ReferenceType ReferenceType { get; set; }
    }
}
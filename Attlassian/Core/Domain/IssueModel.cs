namespace Attlassian.Core.Domain
{
    public class IssueModel
    {
        public string? Value { get; set; }
        public string? AssigneeUser { get; set; }
        public DateTime? Created { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }

    }
}

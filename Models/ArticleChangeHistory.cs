namespace InfoPoster_backend.Models
{
    public class ApplicationChangeHistory
    {
        public ApplicationChangeHistory() { }

        public ApplicationChangeHistory(Guid articleId, Guid applicationId, string? fieldName, string? oldValue, string newValue, Guid userId)
        {
            ArticleId = articleId;
            ApplicationId = applicationId;
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
            ChangedBy = userId;
            ChangedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid ApplicationId { get; set; }
        public string FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public Guid ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}

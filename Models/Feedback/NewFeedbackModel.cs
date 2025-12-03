namespace GrapheneTrace.Models.Feedback
{
    /// <summary>
    /// Model for passing details of new feedback
    /// </summary>
    public class NewFeedbackModel
    {
        public int ChunkId { get; set; }
        public required string Text { get; set; }
    }
}
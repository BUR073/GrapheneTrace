// SID: 2408078
using GrapheneTrace.Models.Feedback;

namespace GrapheneTrace.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<int?> DeleteFeedback(int feedbackId);

        Task<int?> AddFeedback(NewFeedbackModel model, int userId);

    }
}
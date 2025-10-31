using System;
using System.Collections.Generic;
using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Models.Feedback
{

    public class NewFeedbackModel
    {
        public int ChunkId { get; set; }
        public string Text { get; set; }
    }
}
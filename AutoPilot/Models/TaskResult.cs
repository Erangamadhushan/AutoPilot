using System;

namespace AutoPilot.Models
{
    public class TaskResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime ExecutedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public Exception Exception { get; set; }

        public static TaskResult Success(string message = "Task completed successfully")
        {
            return new TaskResult
            {
                IsSuccess = true,
                Message = message,
                ExecutedAt = DateTime.Now
            };
        }

        public static TaskResult Failure(string message, Exception ex = null)
        {
            return new TaskResult
            {
                IsSuccess = false,
                Message = message,
                Exception = ex,
                ExecutedAt = DateTime.Now
            };
        }
    }
}
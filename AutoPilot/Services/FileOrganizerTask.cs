using System.IO;
using System.Threading.Tasks;
using AutoPilot.Models;
using AutoSharp.Core;


namespace AutoPilot.Services
{
    public class FileOrganizerTask : BaseAutomationTask
    {
        public override string Name => "File Organizer Task";

        public override string Description => "Organizes files into categorized folders based on their extensions.";

        protected override async Task<TaskResult> ExecuteTaskAsync(AutomationConfig config)
        {
            // Implementation of the task logic goes here.  
            // For now, returning a successful TaskResult.  
            await Task.CompletedTask;
            return new TaskResult { Success = true };
        }

        private string GetCategoryFolder(string extension)
        {
            var categoryMapping = new Dictionary<string, string>
               {
                   { ".jpg", "Images" },
                   { ".png", "Images" },
                   { ".gif", "Images" },
                   { ".txt", "Documents" },
                   { ".docx", "Documents" },
                   { ".pdf", "Documents" },
                   { ".mp3", "Audio" },
                   { ".wav", "Audio" },
                   { ".mp4", "Videos" },
                   { ".avi", "Videos" }
               };

            return categoryMapping.TryGetValue(extension, out var category) ? category : "Others";
        }
    }
}
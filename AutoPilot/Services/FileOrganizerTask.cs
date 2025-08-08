using System.IO;
using System.Threading.Tasks;
using AutoPilot.Models;
using AutoSharp.Core;


namespace AutoPilot.Services
{
    public class FileOrganizerTask : BaseAutomationTask
    {
        private string GetCategoryFolder(string extension)
        {
            // Define a mapping of file extensions to category folders  
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

            // Return the category folder based on the extension, or "Others" if not found  
            return categoryMapping.TryGetValue(extension, out var category) ? category : "Others";
        }
    }
}
using System.IO;
using System.Threading.Tasks;
using AutoPilot.Models;
using AutoSharp.Core;


namespace AutoPilot.Services
{
    public class FileOrganizerTask : BaseAutomationTask
    {
        public override string Name => "FileOrganizer";
        public override string Description => "Organizes files in specified directory by file type";

        protected override async Task<TaskResult> ExecuteTaskAsync(AutomationConfig config)
        {
            // Get parameters from config
            var sourceDirectory = config.Parameters.GetValueOrDefault("SourceDirectory")?.ToString();
            var targetDirectory = config.Parameters.GetValueOrDefault("TargetDirectory")?.ToString();

            if (string.IsNullOrEmpty(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
                return TaskResult.Failure("Source directory not found or not specified");
            }

            if (string.IsNullOrEmpty(targetDirectory))
            {
                targetDirectory = Path.Combine(sourceDirectory, "Organized");
            }

            // Create target directory if it doesn't exist
            Directory.CreateDirectory(targetDirectory);

            var files = Directory.GetFiles(sourceDirectory);
            int processedFiles = 0;

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).ToLower();
                var categoryFolder = GetCategoryFolder(extension);
                var targetFolder = Path.Combine(targetDirectory, categoryFolder);

                Directory.CreateDirectory(targetFolder);

                var fileName = Path.GetFileName(file);
                var targetFile = Path.Combine(targetFolder, fileName);

                // Move file (or copy if you prefer)
                File.Move(file, targetFile);
                processedFiles++;

                _logger.Information("Moved file {FileName} to {Category}", fileName, categoryFolder);
            }

            await Task.CompletedTask; // Simulate async work

            return TaskResult.Success($"Organized {processedFiles} files successfully");
        }

        private string GetCategoryFolder(string extension)
        {
            return extension switch
            {
                ".txt" or ".doc" or ".docx" or ".pdf" => "Documents",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "Images",
                ".mp4" or ".avi" or ".mkv" or ".mov" => "Videos",
                ".mp3" or ".wav" or ".flac" => "Audio",
                ".zip" or ".rar" or ".7z" => "Archives",
                _ => "Others"
            };
        }

        public override bool CanExecute(AutomationConfig config)
        {
            return base.CanExecute(config) &&
                   config.Parameters.ContainsKey("SourceDirectory");
        }
    }
}
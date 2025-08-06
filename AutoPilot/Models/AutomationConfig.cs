using System.Collections.Generic;

namespace AutoPilot.Models
{
    public class AutomationConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public int IntervalMinutes { get; set; }
        public string TaskType { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public class AppConfig
    {
        public LoggingConfig Logging { get; set; } = new LoggingConfig();
        public List<AutomationConfig> AutomationTasks { get; set; } = new List<AutomationConfig>();
    }

    public class LoggingConfig
    {
        public string LogLevel { get; set; } = "Information";
        public string LogPath { get; set; } = "logs";
        public bool EnableConsoleLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
    }
}
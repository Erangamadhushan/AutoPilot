using System.Threading.Tasks;
using AutoPilot.Models;


namespace AutoSharp.Core
{
    public interface IAutomationTask
    {
        string Name { get; }
        string Description { get; }
        Task<TaskResult> ExecuteAsync(AutomationConfig config);
        bool CanExecute(AutomationConfig config);
    }
}
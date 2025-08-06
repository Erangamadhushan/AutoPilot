using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoPilot.Models;
using Serilog;

namespace AutoSharp.Core
{
    public abstract class BaseAutomationTask : IAutomationTask
    {
        protected readonly ILogger _logger;

        public abstract string Name { get; }
        public abstract string Description { get; }

        protected BaseAutomationTask()
        {
            _logger = Log.ForContext(GetType());
        }

        public async Task<TaskResult> ExecuteAsync(AutomationConfig config)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Information("Starting task: {TaskName}", Name);

                if (!CanExecute(config))
                {
                    var errorMsg = $"Task {Name} cannot be executed with current configuration";
                    _logger.Warning(errorMsg);
                    return TaskResult.Failure(errorMsg);
                }

                var result = await ExecuteTaskAsync(config);
                stopwatch.Stop();

                result.Duration = stopwatch.Elapsed;

                if (result.IsSuccess)
                {
                    _logger.Information("Task {TaskName} completed successfully in {Duration}ms",
                        Name, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.Error("Task {TaskName} failed: {Message}", Name, result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(ex, "Task {TaskName} threw an exception", Name);

                return new TaskResult
                {
                    IsSuccess = false,
                    Message = $"Task failed with exception: {ex.Message}",
                    Exception = ex,
                    ExecutedAt = DateTime.Now,
                    Duration = stopwatch.Elapsed
                };
            }
        }

        protected abstract Task<TaskResult> ExecuteTaskAsync(AutomationConfig config);

        public virtual bool CanExecute(AutomationConfig config)
        {
            return config != null && config.IsEnabled;
        }
    }
}
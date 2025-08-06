using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AutoPilot.Models;
using Serilog;

namespace AutoSharp.Core
{
    public class AutomationEngine : IDisposable
    {
        private readonly Dictionary<string, IAutomationTask> _tasks;
        private readonly Dictionary<string, Timer> _taskTimers;
        private readonly ILogger _logger;
        private bool _isRunning;

        public AutomationEngine()
        {
            _tasks = new Dictionary<string, IAutomationTask>();
            _taskTimers = new Dictionary<string, Timer>();
            _logger = Log.ForContext<AutomationEngine>();
        }

        public void RegisterTask(IAutomationTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            _tasks[task.Name] = task;
            _logger.Information("Registered automation task: {TaskName}", task.Name);
        }

        public async Task StartAsync(List<AutomationConfig> configs)
        {
            _logger.Information("Starting Automation Engine with {TaskCount} configured tasks", configs.Count);
            _isRunning = true;

            foreach (var config in configs.Where(c => c.IsEnabled))
            {
                if (_tasks.ContainsKey(config.TaskType))
                {
                    await StartTaskSchedule(config);
                }
                else
                {
                    _logger.Warning("Task type {TaskType} not found for config {ConfigName}",
                        config.TaskType, config.Name);
                }
            }
        }

        private async Task StartTaskSchedule(AutomationConfig config)
        {
            // Execute immediately
            await ExecuteTask(config);

            // Set up recurring execution
            if (config.IntervalMinutes > 0)
            {
                var timer = new Timer(config.IntervalMinutes * 60 * 1000); // Convert to milliseconds
                timer.Elapsed += async (sender, e) => await ExecuteTask(config);
                timer.AutoReset = true;
                timer.Start();

                _taskTimers[config.Name] = timer;
                _logger.Information("Scheduled task {TaskName} to run every {Interval} minutes",
                    config.Name, config.IntervalMinutes);
            }
        }

        private async Task ExecuteTask(AutomationConfig config)
        {
            if (!_isRunning) return;

            if (_tasks.TryGetValue(config.TaskType, out var task))
            {
                var result = await task.ExecuteAsync(config);

                if (!result.IsSuccess && result.Exception != null)
                {
                    _logger.Error("Task execution failed: {TaskName} - {Message}",
                        config.Name, result.Message);
                }
            }
        }

        public async Task ExecuteTaskOnceAsync(string taskName, AutomationConfig config)
        {
            if (_tasks.TryGetValue(taskName, out var task))
            {
                _logger.Information("Executing single task: {TaskName}", taskName);
                var result = await task.ExecuteAsync(config);

                Console.WriteLine($"Task Result: {(result.IsSuccess ? "SUCCESS" : "FAILED")}");
                Console.WriteLine($"Message: {result.Message}");
                Console.WriteLine($"Duration: {result.Duration.TotalMilliseconds}ms");
            }
            else
            {
                _logger.Warning("Task not found: {TaskName}", taskName);
            }
        }

        public void Stop()
        {
            _logger.Information("Stopping Automation Engine");
            _isRunning = false;

            foreach (var timer in _taskTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            _taskTimers.Clear();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
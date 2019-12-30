using System;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Runners;

namespace HomeCenter.NET.Services
{
    public partial class RunnerService
    {
        public class Process
        {
            public bool IsCompleted { get; set; }
            public bool IsCanceled { get; set; }
            public int Id { get; set; }
            public string? Name { get; set; }
            public Thread? Thread { get; set; }
            public Task? Task { get; set; }
            public RunInformation? Information { get; set; }

            public Process(string? name, Task task, Thread thread)
            {
                Name = name;
                Task = task;
                Id = task.Id;
                Thread = thread;
            }

            public Process(string? name, Exception exception)
            {
                Name = name;
                Information = new RunInformation(exception);
            }

            public void Cancel()
            {
                IsCanceled = true;
                IsCompleted = true;

                Thread?.Abort();
            }
        }
    }
}

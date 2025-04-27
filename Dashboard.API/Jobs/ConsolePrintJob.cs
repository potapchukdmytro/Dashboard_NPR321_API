using Quartz;

namespace Dashboard.API.Jobs
{
    public class ConsolePrintJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"[{DateTime.Now.Second}] Quart job is work");
            return Task.CompletedTask;
        }
    }
}

using Quartz;

namespace Dashboard.API.Jobs
{
    public class LogsClearJob : IJob
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LogsClearJob(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Task Execute(IJobExecutionContext context)
        {
            string logsDirPath = Path.Combine(_webHostEnvironment.ContentRootPath, "logs");
            if (Directory.Exists(logsDirPath))
            {
                var filesNames = Directory.GetFiles(logsDirPath);
                foreach (var fileName in filesNames)
                {
                    var file = new FileInfo(fileName);
                    var minutes = (DateTime.UtcNow - file.CreationTimeUtc).Minutes;
                    if(minutes > 5)
                    {
                        file.Delete();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}

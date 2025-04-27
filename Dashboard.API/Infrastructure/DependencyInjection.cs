using Dashboard.API.Jobs;
using Quartz;

namespace Dashboard.API.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddJobs(this IServiceCollection services, params (Type type, string cron)[] jobs)
        {
            services.AddQuartz(q =>
            {
                foreach (var job in jobs)
                {
                    var jobKey = new JobKey(job.type.Name);
                    q.AddJob(job.type, jobKey);

                    q.AddTrigger(opt => opt
                    .ForJob(jobKey)
                    .WithIdentity($"{job.type.Name}-trigger")
                    .WithCronSchedule(job.cron));
                }
            });
        }
    }
}

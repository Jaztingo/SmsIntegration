using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using SmsIntegration.Attibutes;
using SmsIntegration.BLL.Enums;
using SmsIntegration.BLL.Interfaces;
using SmsIntegration.DAL.IRepository;

namespace SmsIntegration.Jobs
{
    [RunnableJobAttribute]
    public class SmsRetryJobsJobBuilder
    {
        public async Task Run()
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = await sf.GetScheduler();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<SmsRetryJobs>()
                .WithIdentity("SmsIntegration.Jobs.SmsRetry")
                .Build();

            // Trigger the job to run on the next round minute
            Quartz.ITrigger trigger = Quartz.TriggerBuilder.Create()
                .WithIdentity("SmsIntegration.Jobs.SmsRetry")
                //.StartAt(DateBuilder.TodayAt(11, 0, 0))
                .StartAt(DateBuilder.EvenSecondDateAfterNow())
                .WithSimpleSchedule(s => s.WithIntervalInMinutes(1).RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            await sched.ScheduleJob(job, trigger);

            // Start up the scheduler (nothing can actually run until the
            // scheduler has been started)
            await sched.Start();
        }
    }
    public class SmsRetryJobs : IJob
    {
        private readonly ISmsRepository smsRepository;
        private readonly ISmsServices smsServices;
        private readonly ILogger logger;

        public SmsRetryJobs()
        {
            smsRepository = Program.GeneralJobServiceProvider.GetService<ISmsRepository>();
            smsServices = Program.GeneralJobServiceProvider.GetService<ISmsServices>();
            logger = Program.GeneralJobServiceProvider.GetService<ILogger>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var unSendedSmses = await smsRepository.GetAsync(el =>
                                el.Status != (int)SmsFlowStatus.Sended &&
                                el.AttamptCount <= 3 &&
                                el.CreateDate < DateTime.Now.AddMinutes(5));
                foreach (var item in unSendedSmses)
                {
                    await smsServices.SendMessage(item.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

        }
    }
}

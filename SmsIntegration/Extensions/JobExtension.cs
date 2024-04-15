using SmsIntegration.Attibutes;
using SmsIntegration.Jobs;
using System.Reflection;

namespace SmsIntegration.Extensions
{
    public static class JobExtension
    {
        public static void AttachJobs()
        {

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                        t.IsDefined(typeof(RunnableJobAttribute)))
                .ToList();
            foreach (var type in types)
            {
                try
                {
                    object jobInstance = Activator.CreateInstance(type, null);
                    MethodInfo run = type.GetMethod("Run");
                    run.Invoke(jobInstance, null);
                }
                catch (Exception ex)
                {

                }
                
            }
        }
    }
}

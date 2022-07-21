using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Samples
{
    public class Activities
    {

        [FunctionName(nameof(RunStaticActivityWithRandomDelay))]
        public static async Task RunStaticActivityWithRandomDelay([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var inputValue = context.GetInput<object>();
            logger.LogInformation($"Called activity with input {inputValue}");
            await Task.Delay((new Random().Next(1, 5) * 1000) / 2);        
        }

        [FunctionName(nameof(RunStaticActivityWithReturnValue))]
        public static string RunStaticActivityWithReturnValue([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var returnValue = new Random().Next(100, int.MaxValue).ToString();
            logger.LogInformation($"Called activity - returning with value {returnValue}");
            return returnValue;
        }

        [FunctionName(nameof(RunNonStaticActivityWithRandomDelay))]
        public async Task RunNonStaticActivityWithRandomDelay([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var inputValue = context.GetInput<object>();
            logger.LogInformation($"Called activity with input {inputValue}");
            await Task.Delay((new Random().Next(1, 5) * 1000) / 2);
        }

        [FunctionName(nameof(RunNonStaticActivityWithReturnValue))]
        public string RunNonStaticActivityWithReturnValue([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var returnValue = new Random().Next(100, int.MaxValue).ToString();
            logger.LogInformation($"Called activity - returning with value {returnValue}");
            return returnValue;
        }

        [FunctionName(nameof(RunStaticActivityWithRandomStatus))]
        public static string RunStaticActivityWithRandomStatus([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var statuses = new[] { "Running", "Completed" };
            var returnValue = statuses[new Random().Next(0, 2)];
            logger.LogInformation($"Called activity - returning with value {returnValue}");
            return returnValue;
        }

        [FunctionName(nameof(RunNonStaticActivityWithRandomStatus))]
        public string RunNonStaticActivityWithRandomStatus([ActivityTrigger] IDurableActivityContext context, ILogger logger)
        {
            var statuses = new[] { "Running", "Completed" };
            var returnValue = statuses[new Random().Next(0, 2)];
            logger.LogInformation($"Called activity - returning with value {returnValue}");
            return returnValue;
        }

    }
}

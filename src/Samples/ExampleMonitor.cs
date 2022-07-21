using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Samples
{
    internal class ExampleMonitor
    {

        [FunctionName(nameof(RunExampleMonitorOrchestration))]
        public async static Task<IActionResult> RunExampleMonitorOrchestration(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "example/monitor/{id?}")] HttpRequest req,
            string id,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();

            await durableClient.StartNewAsync(nameof(ExampleMonitorOrchestration), id);
            return new OkObjectResult(string.Empty);
        }

        [FunctionName(nameof(ExampleMonitorOrchestration))]
        public static async Task ExampleMonitorOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
            => await context
                    .AsFluentOrchestration(loggerBase)
                    .Then((ctx) => ctx.Logger.LogInformation("Running Orchestration"))
                    .ThenWaitUntilAsync<Activities, string>((c) => c.RunNonStaticActivityWithRandomStatus, (ctx) => "input", 
                        interval: TimeSpan.FromSeconds(2), 
                        expires: TimeSpan.FromSeconds(10),
                        (ctx) => ctx.LastStepResult.As<string>() == "Completed")
                    .Then((ctx) => ctx.Logger.LogInformation("Done"));

    }
}

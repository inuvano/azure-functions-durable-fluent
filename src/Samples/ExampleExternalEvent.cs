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
    internal class ExampleExternalEvent
    {

        [FunctionName(nameof(RunExampleExternalEventOrchestration))]
        public async static Task<IActionResult> RunExampleExternalEventOrchestration(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "example/external/{id?}")] HttpRequest req,
            string id,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();

            await durableClient.StartNewAsync(nameof(ExampleExternalEventOrchestration), id);
            return new OkObjectResult(string.Empty);
        }

        [FunctionName(nameof(ExampleExternalEventOrchestration))]
        public static async Task ExampleExternalEventOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
             => await context
                     .AsFluentOrchestration(loggerBase)
                     .Then((ctx) => ctx.Logger.LogInformation("Running Orchestration"))
                     .ThenWaitForAsync<object>("Approved", TimeSpan.FromMinutes(1))
                     .When((ctx) => ctx.LastStepResult == null, (ctx) => ctx.Logger.LogInformation("Approval Timeout"))
                     .When((ctx) => ctx.LastStepResult != null, (ctx) => ctx.Logger.LogInformation($"Approval Result: {ctx.LastStepResult.As<object>()}"))
                     .Then((ctx) => ctx.Logger.LogInformation("Done"));

    }
}

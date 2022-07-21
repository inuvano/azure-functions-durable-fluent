using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples
{
    internal class ExampleFanOutIn
    {

        [FunctionName(nameof(RunExampleFanOutInOrchestration))]
        public async static Task<IActionResult> RunExampleFanOutInOrchestration(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "example/fanoutin/{id?}")] HttpRequest req,
            string id,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();

            await durableClient.StartNewAsync(nameof(ExampleFanOutInOrchestration), id);
            return new OkObjectResult(string.Empty);
        }

        [FunctionName(nameof(ExampleFanOutInOrchestration))]
        public static async Task ExampleFanOutInOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
            => await context
                    .AsFluentOrchestration(loggerBase)
                    .Then((ctx) => ctx.Logger.LogInformation("Running Orchestration"))
                    .ThenFanOutAsync<Activities, string>((c) => c.RunNonStaticActivityWithReturnValue, (ctx) => Enumerable.Range(1, 3))
                    .Then((ctx) => ctx.Logger.LogInformation($"Fan Out result: {string.Join(", ", ctx.LastStepResult.As<IEnumerable<string>>())}"))
                    .Then((ctx) => ctx.Context.SetOutput(ctx.LastStepResult.As<IEnumerable<string>>()));

    }
}

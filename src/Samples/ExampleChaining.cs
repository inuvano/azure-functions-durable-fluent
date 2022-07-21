using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Samples
{
    internal class ExampleChaining
    {

        [FunctionName(nameof(RunExampleChainingOrchestration))]
        public async static Task<IActionResult> RunExampleChainingOrchestration(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "example/chaining")] HttpRequest req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            await durableClient.StartNewAsync(nameof(ExampleChainingOrchestration));
            return new OkObjectResult(string.Empty);
        }

        [FunctionName(nameof(ExampleChainingOrchestration))]
        public static async Task ExampleChainingOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
             => await context
                    .AsFluentOrchestration(loggerBase)
                    .ThenAsync(Activities.RunStaticActivityWithReturnValue)
                    .ThenAsync<Activities>((c) => c.RunNonStaticActivityWithReturnValue)
                    .ThenAsync<Activities>((c) => c.RunNonStaticActivityWithReturnValue, (ctx) => new { Name = "Bob Smith" })
                    .Then((ctx) => ctx.Logger.LogInformation("Done"));

    }
}

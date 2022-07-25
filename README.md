# Fluent Durable Functions
[![NuGet](http://img.shields.io/nuget/vpre/InuvanoLabs.Azure.Functions.FluentDurableTask.svg?label=NuGet)](https://www.nuget.org/packages/InuvanoLabs.Azure.Functions.FluentDurableTask/)

## What is Fluent Durable Functions?

### Supported Patterns

Chaining

```csharp
        [FunctionName(nameof(ExampleChainingOrchestration))]
        public static async Task ExampleChainingOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
             => await context
                    .AsFluentOrchestration(loggerBase)
                    .ThenAsync(Activities.RunStaticActivityWithReturnValue)
                    .ThenAsync<Activities>((c) => c.RunNonStaticActivityWithReturnValue)
                    .ThenAsync<Activities, string>((c) => c.RunNonStaticActivityWithReturnValue, (ctx) => new { Name = "Bob Smith" })
                    .Then((ctx) => ctx.Logger.LogInformation($"Activity Result: {ctx.LastStepResult.As<string>()}"))
                    .Then((ctx) => ctx.Logger.LogInformation("Done"));
```

Fan-Out/In

```csharp
        [FunctionName(nameof(ExampleFanOutInOrchestration))]
        public static async Task ExampleFanOutInOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
            => await context
                    .AsFluentOrchestration(loggerBase)
                    .Then((ctx) => ctx.Logger.LogInformation("Running Orchestration"))
                    .ThenFanOutAsync<Activities, string>((c) => c.RunNonStaticActivityWithReturnValue, (ctx) => Enumerable.Range(1, 3))
                    .Then((ctx) => ctx.Logger.LogInformation($"Fan Out result: {string.Join(", ", ctx.LastStepResult.As<IEnumerable<string>>())}"))
                    .Then((ctx) => ctx.Context.SetOutput(ctx.LastStepResult.As<IEnumerable<string>>()));
```

Monitor

```csharp
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
```

External Event


```csharp
        [FunctionName(nameof(ExampleExternalEventOrchestration))]
        public static async Task ExampleExternalEventOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger loggerBase)
             => await context
                     .AsFluentOrchestration(loggerBase)
                     .Then((ctx) => ctx.Logger.LogInformation("Running Orchestration"))
                     .ThenWaitForAsync<object>("Approved", TimeSpan.FromMinutes(1))
                     .When((ctx) => ctx.LastStepResult == null, (ctx) => ctx.Logger.LogInformation("Approval Timeout"))
                     .When((ctx) => ctx.LastStepResult != null, (ctx) => ctx.Logger.LogInformation($"Approval Result: {ctx.LastStepResult.As<object>()}"))
                     .Then((ctx) => ctx.Logger.LogInformation("Done"));
```

See the Samples project for more details.

## How do I get it?

Package available on NuGet https://www.nuget.org/packages/InuvanoLabs.Azure.Functions.FluentDurableTask

## Issues? Feedback?

Please file an issue above.
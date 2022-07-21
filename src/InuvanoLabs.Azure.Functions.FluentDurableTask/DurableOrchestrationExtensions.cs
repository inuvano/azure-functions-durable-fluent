using InuvanoLabs.Azure.Functions.FluentDurableTask;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    public static class DurableOrchestrationExtensions
    {
        public static Task<FluentOrchestrationContext> AsFluentOrchestration(this IDurableOrchestrationContext context)
        {
            return Task.FromResult(new FluentOrchestrationContext(context));
        }

        public static Task<FluentOrchestrationContext> AsFluentOrchestration(this IDurableOrchestrationContext context, ILogger baseLogger)
        {
            return Task.FromResult(new FluentOrchestrationContext(context, baseLogger));
        }

        public static async Task<FluentOrchestrationContext> When(this Task<FluentOrchestrationContext> instance, Func<FluentOrchestrationContext, bool> action, Func<FluentOrchestrationContext, Task> thenAction)
        {
            var context = await instance;
            return await context.When(action, thenAction);
        }

        public static async Task<FluentOrchestrationContext> When(this Task<FluentOrchestrationContext> instance, Func<FluentOrchestrationContext, bool> action, Action<FluentOrchestrationContext> thenAction)
        {
            var context = await instance;
            return await context.When(action, thenAction);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TActivityClass>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod)
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenAsync(activityMethod(activityClassInstance));
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TActivityClass>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod, Func<FluentOrchestrationContext, object> inputDelegate)
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenAsync(activityMethod(activityClassInstance), inputDelegate);
        }


        public static async Task<FluentOrchestrationContext> ThenAsync(this Task<FluentOrchestrationContext> instance, Delegate activityMethod)
        {
            var context = await instance;
            return await context.ThenAsync(activityMethod);
        }

        public static async Task<FluentOrchestrationContext> Then(this Task<FluentOrchestrationContext> instance, Action<FluentOrchestrationContext> action)
        {
            var context = await instance;
            return await context.Then(action);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TResult>(this Task<FluentOrchestrationContext> instance, Delegate activityMethod)
            where TResult : class
        {
            var context = await instance;
            return await context.ThenAsync<TResult>(activityMethod);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TActivityClass, TResult>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod)
            where TResult : class
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenAsync<TResult>(activityMethod(activityClassInstance));
        }

        public static async Task<FluentOrchestrationContext> ThenFanOutAsync(this Task<FluentOrchestrationContext> instance, Delegate activityMethod, Func<FluentOrchestrationContext, IEnumerable> inputDelegate)
        {
            var context = await instance;
            return await context.ThenFanOutAsync(activityMethod, inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenFanOutAsync<TResult>(this Task<FluentOrchestrationContext> instance, Delegate activityMethod, Func<FluentOrchestrationContext, IEnumerable> inputDelegate)
            where TResult : class
        {
            var context = await instance;
            return await context.ThenFanOutAsync<TResult>(activityMethod, inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenFanOutAsync<TActivityClass, TResult>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod, Func<FluentOrchestrationContext, IEnumerable> inputDelegate)
            where TResult : class
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenFanOutAsync<TResult>(activityMethod(activityClassInstance), inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync(this Task<FluentOrchestrationContext> instance, Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate)
        {
            var context = await instance;
            return await context.ThenAsync(activityMethod, inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TResult>(this Task<FluentOrchestrationContext> instance, Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate)
            where TResult : class
        {
            var context = await instance;
            return await context.ThenAsync<TResult>(activityMethod, inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenAsync<TActivityClass, TResult>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod, Func<FluentOrchestrationContext, object> inputDelegate)
            where TResult : class
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenAsync<TResult>(activityMethod(activityClassInstance), inputDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenWaitUntilAsync<TResult>(this Task<FluentOrchestrationContext> instance, Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate, TimeSpan interval, TimeSpan expires, Func<FluentOrchestrationContext, bool> untilDelegate)
            where TResult : class
        {
            var context = await instance;
            return await context.ThenWaitUntilAsync<TResult>(activityMethod, inputDelegate, interval, expires, untilDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenWaitUntilAsync<TActivityClass, TResult>(this Task<FluentOrchestrationContext> instance, Func<TActivityClass, Delegate> activityMethod, Func<FluentOrchestrationContext, object> inputDelegate, TimeSpan interval, TimeSpan expires, Func<FluentOrchestrationContext, bool> untilDelegate)
            where TResult : class
            where TActivityClass : class, new()
        {
            var context = await instance;
            var activityClassInstance = new TActivityClass();
            return await context.ThenWaitUntilAsync<TResult>(activityMethod(activityClassInstance), inputDelegate, interval, expires, untilDelegate);
        }

        public static async Task<FluentOrchestrationContext> ThenWaitForAsync<TResult>(this Task<FluentOrchestrationContext> instance, string externalEventName, TimeSpan expires)
            where TResult : class
        {
            var context = await instance;
            return await context.ThenWaitForAsync<TResult>(externalEventName, expires);
        }

    }
}

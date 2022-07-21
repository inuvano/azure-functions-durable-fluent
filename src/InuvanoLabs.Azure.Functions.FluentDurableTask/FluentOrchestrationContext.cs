using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace InuvanoLabs.Azure.Functions.FluentDurableTask
{
    public sealed class FluentOrchestrationContext
    {
        public IDurableOrchestrationContext Context { get; }
        public IStepResult? LastStepResult { get; private set; }
        public ILogger? Logger { get; private set; }

        public FluentOrchestrationContext(IDurableOrchestrationContext context)
        {
            Context = context;
        }

        public FluentOrchestrationContext(IDurableOrchestrationContext context, ILogger baseLogger) : this(context)
        {
            Logger = context.CreateReplaySafeLogger(baseLogger);
        }

        public async Task<FluentOrchestrationContext> When(Func<FluentOrchestrationContext, bool> action, Func<FluentOrchestrationContext, Task> thenAction)
        {
            var actionResult = action(this);
            if (actionResult)
            {
                await thenAction(this);
            }
            return this;
        }

        public Task<FluentOrchestrationContext> When(Func<FluentOrchestrationContext, bool> action, Action<FluentOrchestrationContext> thenAction)
        {
            var actionResult = action(this);
            if (actionResult)
            {
                thenAction(this);
            }
            return Task.FromResult(this);
        }

        public async Task<FluentOrchestrationContext> ThenAsync(Delegate activityMethod)
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            await Context.CallActivityAsync(functionName, null);
            LastStepResult = null;
            return this;
        }

        public async Task<FluentOrchestrationContext> ThenAsync(Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate)
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            await Context.CallActivityAsync(functionName, inputDelegate(this));
            LastStepResult = null;
            return this;
        }

        public async Task<FluentOrchestrationContext> ThenAsync<TResult>(Delegate activityMethod) where TResult : class
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            var result = await Context.CallActivityAsync<TResult>(functionName, null);
            LastStepResult = new StepResult<TResult>(result);
            return this;
        }

        public async Task<FluentOrchestrationContext> ThenAsync<TResult>(Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate) where TResult : class
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            var result = await Context.CallActivityAsync<TResult>(functionName, inputDelegate(this));
            LastStepResult = new StepResult<TResult>(result);
            return this;
        }

        public Task<FluentOrchestrationContext> Then(Action<FluentOrchestrationContext> action)
        {
            action(this);
            return Task.FromResult(this);
        }

        public async Task<FluentOrchestrationContext> ThenFanOutAsync(Delegate activityMethod, Func<FluentOrchestrationContext, IEnumerable> inputDelegate)
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            var tasks = new List<Task>();
            var inputs = inputDelegate(this);

            foreach (var input in inputs)
            {
                tasks.Add(Context.CallActivityAsync(functionName, input));
            }

            await Task.WhenAll(tasks);
            LastStepResult = null;
            return this;
        }

        public async Task<FluentOrchestrationContext> ThenFanOutAsync<TResult>(Delegate activityMethod, Func<FluentOrchestrationContext, IEnumerable> inputDelegate)
            where TResult : class
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            var tasks = new List<Task<TResult>>();
            var inputs = inputDelegate(this);

            foreach (var input in inputs)
            {
                tasks.Add(Context.CallActivityAsync<TResult>(functionName, input));
            }

            var results = await Task.WhenAll(tasks);
            LastStepResult = new StepResult<IEnumerable<TResult>>(results);
            return this;
        }

        public async Task<FluentOrchestrationContext> ThenWaitUntilAsync<TResult>(Delegate activityMethod, Func<FluentOrchestrationContext, object> inputDelegate, TimeSpan interval, TimeSpan expires, Func<FluentOrchestrationContext, bool> untilDelegate) where TResult : class
        {
            var target = activityMethod;
            var functionName = FindFunctionName(target.Method);
            if (string.IsNullOrEmpty(functionName))
                throw new InvalidOperationException($"Unable to find FunctionNameAttribute on method '${target.Method.Name}'");

            DateTime expiryTime = Context.CurrentUtcDateTime.Add(expires);
            var activityInput = inputDelegate(this);

            while (Context.CurrentUtcDateTime < expiryTime)
            {
                var result = await Context.CallActivityAsync<TResult>(functionName, activityInput);
                LastStepResult = new StepResult<TResult>(result);
                if (untilDelegate(this))
                    break;

                await Context.CreateTimer(Context.CurrentUtcDateTime.Add(interval), CancellationToken.None);
            }

            return this;
        }

        public async Task<FluentOrchestrationContext> ThenWaitForAsync<TResult>(string externalEventName, TimeSpan expires) where TResult : class
        {

            DateTime expiryTime = Context.CurrentUtcDateTime.Add(expires);

            using var timeoutCts = new CancellationTokenSource();
            Task durableTimeout = Context.CreateTimer(expiryTime, timeoutCts.Token);

            Task<TResult> externalEvent = Context.WaitForExternalEvent<TResult>(externalEventName);
            if (externalEvent == await Task.WhenAny(externalEvent, durableTimeout))
            {
                timeoutCts.Cancel();
                LastStepResult = new StepResult<TResult>(externalEvent.Result);
            }
            else
            {
                LastStepResult = null;
            }

            return this;
        }

        private string FindFunctionName(MethodInfo method)
        {
            FunctionNameAttribute? functionAttribute = method.GetCustomAttribute<FunctionNameAttribute>();
            var isActivityMethod = method.GetParameters().Any(x => x.GetCustomAttribute<ActivityTriggerAttribute>() != null);
            if (isActivityMethod && functionAttribute != null)
                return functionAttribute.Name;
            return string.Empty;
        }

    }
}

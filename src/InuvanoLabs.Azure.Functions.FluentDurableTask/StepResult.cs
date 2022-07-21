using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InuvanoLabs.Azure.Functions.FluentDurableTask
{
    public sealed class StepResult<TResult> : IStepResult where TResult : class
    {
        public object Value { get; }

        public StepResult(TResult value)
        {
            Value = value;
        }

        public TValue As<TValue>()
        {
            return (TValue)Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InuvanoLabs.Azure.Functions.FluentDurableTask
{
    public interface IStepResult
    {
        public object Value { get; }

        public TValue As<TValue>();
    }
}

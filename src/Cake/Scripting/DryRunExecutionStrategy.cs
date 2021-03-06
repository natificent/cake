﻿using System;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.Scripting
{
    internal sealed class DryRunExecutionStrategy : IExecutionStrategy
    {
        private readonly ICakeLog _log;
        private int _counter;

        public DryRunExecutionStrategy(ICakeLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            _log = log;
            _counter = 1;
        }

        public void PerformSetup(Action action)
        {
        }

        public void PerformTeardown(Action action)
        {
        }

        public void Execute(CakeTask task, ICakeContext context)
        {
            if (task != null)
            {
                _log.Information("{0}. {1}", _counter, task.Name);
                _counter++;
            }
        }

        public void Skip(CakeTask task)
        {
        }

        public void ReportErrors(Action<Exception> action, Exception exception)
        {
        }

        public void HandleErrors(Action<Exception> action, Exception exception)
        {
        }

        public void InvokeFinally(Action action)
        {
        }
    }
}

using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace EMCR.Utilities.Runtime
{
    public class CallContext
    {
        public CancellationTokenSource Cts { get; }
        public string TraceIdentifier { get; }
        public string ContextIdentifier { get; }
        public IServiceProvider Services { get; }

        public CallContext(IServiceProvider services, CancellationTokenSource cts, string traceIdentifier)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Cts = cts ?? throw new ArgumentNullException(nameof(cts));
            TraceIdentifier = traceIdentifier ?? throw new ArgumentNullException(nameof(traceIdentifier));
            ContextIdentifier = Guid.NewGuid().ToString("N");
        }

        [ContextStatic]
        private static CallContext? current;

        public static CallContext Current
        {
            get => current ?? new CallContext(new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()), new CancellationTokenSource(), string.Empty);
            set => current = value;
        }
    }
}

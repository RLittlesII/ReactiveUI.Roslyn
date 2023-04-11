using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Sample
{
    public class MultipleUsesOfSubscribeOnExample
    {

        public MultipleUsesOfSubscribeOnExample() => Observable
                                                     .Return(Unit.Default)
                                                     .SubscribeOn(TaskPoolScheduler.Default)
                                                     .SubscribeOn(ImmediateScheduler.Instance);
    }
}
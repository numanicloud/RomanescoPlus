using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Romanesco.Host2.ViewModels;

public class NoneViewModel : IDataViewModel
{
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}

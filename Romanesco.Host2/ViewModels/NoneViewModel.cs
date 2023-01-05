using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Romanesco.Host2.ViewModels;

public class NoneViewModel : IDataViewModel
{
    public string Title { get; } = "";
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}

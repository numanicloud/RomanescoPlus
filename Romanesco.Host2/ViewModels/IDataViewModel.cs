using System;
using System.Reactive;

namespace Romanesco.Host2.ViewModels;

public interface IDataViewModel
{
    public string Title { get; }
    IObservable<Unit> OpenDetail { get; }
}

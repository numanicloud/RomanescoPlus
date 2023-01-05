using System;
using System.Reactive;

namespace Romanesco.Host2.ViewModels;

public interface IDataViewModel
{
    IObservable<Unit> OpenDetail { get; }
}

using System;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public class IntViewModel : IDataViewModel
{
    public required IntModel Model { private get; init; }

    public string Title => Model.Title;
    public ReactiveProperty<int> Data => Model.Data;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Markup;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

[ContentProperty(nameof(Model))]
public class IntViewModel : IDataViewModel
{
    public required IntModel Model { private get; init; }

    public ReactiveProperty<int> Data => Model.Data;
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}

[ContentProperty(nameof(Model))]
public class StringViewModel : IDataViewModel
{
    public required StringModel Model { private get; init; }

    public ReactiveProperty<string> Data => Model.Data;
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}

[ContentProperty(nameof(Model))]
public class FloatViewModel : IDataViewModel
{
    public required FloatModel Model { private get; init; }

    public ReactiveProperty<float> Data => Model.Data;
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}

[ContentProperty(nameof(Model))]
public class BoolViewModel : IDataViewModel
{
    public required BoolModel Model { private get; init; }

    public ReactiveProperty<bool> Data => Model.Data;
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
}
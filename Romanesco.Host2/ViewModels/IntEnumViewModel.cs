using System;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

internal class IntEnumViewModel : IDataViewModel
{
    private readonly IntEnumModel _model;

    public string Title => _model.Title;
    public IObservable<Unit> OpenDetail { get; } = Observable.Never<Unit>();
    public ReactiveProperty<object?> SelectedValue { get; }
    public object[] Choices { get; }

    public IntEnumViewModel(IntEnumModel model)
    {
        _model = model;
        Choices = model.Choices;
        SelectedValue = model.Selected;
    }
}
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

internal class IntIdReferenceViewModel : IDataViewModel
{
    private readonly Subject<Unit> _openDetailSubject = new();

    public required IntModel Model { get; init; }
    public required ReadOnlyReactiveProperty<MasterDataZZ> Master { get; init; }
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    public ReactiveProperty<NamedArrayItemViewModel?> SelectedItem { get; } = new();

    public IntIdReferenceViewModel()
    {
        SelectedItem.FilterNull()
            .Select(x => x.Data)
            .OfType<ClassViewModel>()
            .Select(x => x.IdElement)
            .FilterNull()
            .SelectMany(x => x.Data)
            .Subscribe(m => Model!.Data.Value = m);
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
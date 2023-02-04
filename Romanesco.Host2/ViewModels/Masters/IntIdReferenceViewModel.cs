using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Romanesco.DataModel;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

internal class IntIdReferenceViewModel : IDataViewModel
{
    private readonly Subject<Unit> _openDetailSubject = new();

    public IntModel Model { get; }
    public ReadOnlyReactiveProperty<MasterData> Master { get; }
    public string Title => Model.Title;
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    public ReactiveProperty<INamedArrayItem?> SelectedItem { get; } = new();

    public IntIdReferenceViewModel(IntModel model, ReadOnlyReactiveProperty<MasterData> master)
    {
        Model = model;
        Master = master;

        SelectedItem.FilterNull()
            .Select(x => x.ViewModel)
            .OfType<NamedClassViewModel>()
            .Select(x => x.IdProvider?.IdModel)
            .FilterNull()
            .SelectMany(x => x.Data)
            .Subscribe(m => Model.Data.Value = m);

        Master.Subscribe(m =>
        {
            if (m is InitializedMasterData initialized)
            {
                SelectedItem.Value = initialized.Choices
                    .FirstOrDefault(x =>
                        x.ViewModel.IdProvider?.IdModel.Data.Value == model.Data.Value);
            }
        });
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
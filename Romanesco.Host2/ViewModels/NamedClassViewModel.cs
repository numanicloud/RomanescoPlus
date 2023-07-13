using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public class NamedClassViewModel : IDataViewModel
{
    private readonly Subject<Unit> _openDetailSubject = new ();
    public string Title { get; }
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    public PropertyViewModel[] Children { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public ClassIdProvider? IdProvider { get; }

    public IReadOnlyReactiveProperty<string> EntryName { get; }
    public NamedClassViewModel ViewModel => this;
    public NamedClassModel Model { get; }

    public NamedClassViewModel(NamedClassModel model, IViewModelFactory factory)
    {
        Children = model.Inner.Children
            .Select(x => new PropertyViewModel()
            {
                Model = x,
                Data = factory.Create(x, factory)
            })
            .ToArray();

        DetailedData = Children.Select(x => x.Data.OpenDetail.Select(_ => x.Data))
            .Merge()
            .ToReadOnlyReactiveProperty(new NoneViewModel());

        Title = model.Title;
        IdProvider = model.Inner.IdProvider;
        EntryName = model.EntryName;
        Model = model;
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;

namespace Romanesco.Host2.ViewModels;

internal class NamedArrayViewModel : IDataViewModel
{
    private readonly NamedArrayModel _model;
    private readonly Subject<Unit> _openDetailSubject = new();
    private readonly Subject<Unit> _closeDetailSubject = new();

    public ReadOnlyReactiveCollection<INamedArrayItem> Items { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public string Title => _model.Title;
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    public EditorCommand[] EditorCommands { get; init; } = Array.Empty<EditorCommand>();
    
    public ReactiveCommand NewCommand { get; } = new();
    public ReactiveCommand<INamedArrayItem> RemoveCommand { get; } = new();
    public ReactiveCommand<INamedArrayItem> MoveUpCommand { get; } = new();
    public ReactiveCommand<INamedArrayItem> MoveDownCommand { get; } = new();
    public ReactiveCommand<INamedArrayItem> DuplicateCommand { get; } = new();
    public ReactiveProperty<INamedArrayItem?> SelectedItem { get; } = new();

    public NamedArrayViewModel(NamedArrayModel model, IViewModelFactory factory)
    {
        _model = model;

        Items = model.Inner.Items
            .ToReadOnlyReactiveCollection(x =>
                new NamedClassViewModel(x, factory) as INamedArrayItem);

        DetailedData = SelectedItem
            .Where(x => x is not null)
            .Select(x => x?.ViewModel)
            .Merge<IDataViewModel?>(_closeDetailSubject.Select(x => new NoneViewModel()))
            .ToReadOnlyReactiveProperty()!;

        NewCommand.Subscribe(New);
        RemoveCommand.Subscribe(Remove);
        MoveUpCommand.Subscribe(MoveUp);
        MoveDownCommand.Subscribe(MoveDown);
        DuplicateCommand.Subscribe(Duplicate);
    }

    public void New()
    {
        _model.New();
    }

    public void Remove(INamedArrayItem item)
    {
        _model.Remove(item.ViewModel.Model);
        if (item == SelectedItem.Value)
        {
            _closeDetailSubject.OnNext(Unit.Default);
        }
    }

    public void MoveUp(INamedArrayItem item) => _model.MoveUp(item.ViewModel.Model);

    public void MoveDown(INamedArrayItem item) => _model.MoveDown(item.ViewModel.Model);

    public void Duplicate(INamedArrayItem item) => _model.Duplicate(item.ViewModel.Model);

    public void Edit() => _openDetailSubject.OnNext(Unit.Default);
}

internal interface INamedArrayItem
{
    IReadOnlyReactiveProperty<string> EntryName { get; }
    NamedClassViewModel ViewModel { get; }
}
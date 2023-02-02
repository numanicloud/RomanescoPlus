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
    private readonly ArrayModel _model;
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

    public NamedArrayViewModel(ArrayModel model, IViewModelFactory factory)
    {
        _model = model;

        Items = model.Items
            .ToReadOnlyReactiveCollection(x =>
            {
                if (x is ClassModel cm)
                {
                    if (cm.EntryName is not MutableEntryName name) throw new Exception();

                    return new NamedArrayItemViewModel()
                    {
                        Data = factory.Create(cm, factory),
                        EntryName = name.Name
                    } as INamedArrayItem;
                }
                else if (x is NamedClassModel ncm)
                {
                    return new NamedClassViewModel(ncm, factory);
                }
                else
                {
                    throw new Exception();
                }
            });

        DetailedData = SelectedItem
            .Where(x => x is not null)
            .Select(x => x?.Data)
            .Merge(_closeDetailSubject.Select(x => new NoneViewModel()))
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
        _model.RemoveAt(Items.IndexOf(item));
        if (item == SelectedItem.Value)
        {
            _closeDetailSubject.OnNext(Unit.Default);
        }
    }

    public void MoveUp(INamedArrayItem item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index - 1);
    }

    public void MoveDown(INamedArrayItem item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index + 1);
    }

    public void Duplicate(INamedArrayItem item)
    {
        var index = Items.IndexOf(item);
        _model.Duplicate(index);
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}

internal class NamedArrayItemViewModel : INamedArrayItem
{
    public required IReadOnlyReactiveProperty<string> EntryName { get; init; }
    public required IDataViewModel Data { get; init; }
}

internal interface INamedArrayItem
{
    IReadOnlyReactiveProperty<string> EntryName { get; }
    IDataViewModel Data { get; }
}
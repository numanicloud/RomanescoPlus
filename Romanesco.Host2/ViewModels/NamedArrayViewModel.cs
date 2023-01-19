using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

internal class NamedArrayViewModel : IDataViewModel
{
    private readonly ArrayModel _model;
    private readonly Subject<Unit> _openDetailSubject = new();

    public ReadOnlyReactiveCollection<NamedArrayItemViewModel> Items { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public string Title => _model.Title;
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    
    public ReactiveCommand NewCommand { get; } = new();
    public ReactiveCommand<NamedArrayItemViewModel> RemoveCommand { get; } = new();
    public ReactiveCommand<NamedArrayItemViewModel> MoveUpCommand { get; } = new();
    public ReactiveCommand<NamedArrayItemViewModel> MoveDownCommand { get; } = new();
    public ReactiveProperty<NamedArrayItemViewModel?> SelectedItem { get; } = new();

    public NamedArrayViewModel(ArrayModel model, IViewModelFactory factory)
    {
        _model = model;

        Items = model.Items
            .ToReadOnlyReactiveCollection(x =>
            {
                if (x is not ClassModel cm) throw new Exception();
                if (cm.EntryName is not MutableEntryName name) throw new Exception();

                return new NamedArrayItemViewModel()
                {
                    Data = factory.Create(cm, factory),
                    EntryName = name.Name
                };
            });

        DetailedData = SelectedItem
            .Where(x => x is not null)
            .Select(x => x?.Data)
            .ToReadOnlyReactiveProperty()!;

        NewCommand.Subscribe(New);
        RemoveCommand.Subscribe(Remove);
        MoveUpCommand.Subscribe(MoveUp);
        MoveDownCommand.Subscribe(MoveDown);
    }

    public void New()
    {
        _model.New();
    }

    public void Remove(NamedArrayItemViewModel item)
    {
        _model.RemoveAt(Items.IndexOf(item));
    }

    public void MoveUp(NamedArrayItemViewModel item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index - 1);
    }

    public void MoveDown(NamedArrayItemViewModel item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index + 1);
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}

internal class NamedArrayItemViewModel
{
    public required IReadOnlyReactiveProperty<string> EntryName { get; init; }
    public required IDataViewModel Data { get; init; }
}
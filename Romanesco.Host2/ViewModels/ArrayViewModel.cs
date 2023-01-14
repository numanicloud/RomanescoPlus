using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public class ArrayViewModel : IDataViewModel
{
    private readonly ArrayModel _model;
    private readonly Subject<Unit> _openDetailSubject = new();

    public string Title { get; }
    public ReadOnlyReactiveCollection<IDataViewModel> Items { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public IObservable<Unit> OpenDetail => _openDetailSubject;

    public ReactiveCommand NewCommand { get; } = new();
    public ReactiveCommand<IDataViewModel> RemoveCommand { get; } = new();
    public ReactiveCommand<IDataViewModel> MoveUpCommand { get; } = new();
    public ReactiveCommand<IDataViewModel> MoveDownCommand { get; } = new();

    public ArrayViewModel()
    {
        // デザインデータ
        _model = new ArrayModel()
        {
            ElementType = new TypeId(typeof(int[])),
            Prototype = new IntModel()
            {
                Title = "Item"
            },
            Title = "Design"
        };

        Title = _model.Title;

        Items = new ReactiveCollection<IDataViewModel>()
        {
            new IntViewModel()
            {
                Model = new IntModel()
                {
                    Data = { Value = 9 },
                    Title = "Item"
                },
            },
            new IntViewModel()
            {
                Model = new IntModel()
                {
                    Data = { Value = 19 },
                    Title = "Item"
                },
            },
        }.ToReadOnlyReactiveCollection();
        DetailedData = new ReactiveProperty<IDataViewModel>();
    }

    public ArrayViewModel(ArrayModel model, IViewModelFactory factory)
    {
        _model = model;
        Title = model.Title;
        
        Items = model.Items.ToReadOnlyReactiveCollection(x => factory.Create(x, factory));

        DetailedData = Items.ToCollectionChanged()
            .SelectMany(_ => Items.Select(x => x.OpenDetail.Select(_ => x)))
            .Merge()
            .ToReadOnlyReactiveProperty(new NoneViewModel());

        NewCommand.Subscribe(New);
        RemoveCommand.Subscribe(Remove);
        MoveUpCommand.Subscribe(MoveUp);
        MoveDownCommand.Subscribe(MoveDown);
    }

    public void New()
    {
        _model.New();
    }

    public void Remove(IDataViewModel item)
    {
        _model.RemoveAt(Items.IndexOf(item));
    }

    public void MoveUp(IDataViewModel item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index - 1);
    }

    public void MoveDown(IDataViewModel item)
    {
        var index = Items.IndexOf(item);
        _model.Move(index, index + 1);
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
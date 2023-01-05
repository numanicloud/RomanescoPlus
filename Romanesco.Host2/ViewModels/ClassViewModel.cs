using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;

namespace Romanesco.Host2.ViewModels;

public class ClassViewModel : IDataViewModel
{
    private readonly Subject<Unit> _openDetailSubject = new();
    private readonly IDataViewModel[] _children = Array.Empty<IDataViewModel>();

    public required IDataViewModel[] Children
    {
        get => _children;
        init
        {
            _children = value;
            DetailedData = _children.Select(x => x.OpenDetail.Select(_ => x))
                .Merge()
                .ToReadOnlyReactiveProperty(new NoneViewModel());
        }
    }

    public required string Title { get; init; }

    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; private init; }

    public IObservable<Unit> OpenDetail => _openDetailSubject;

    public ClassViewModel()
    {
        DetailedData = new ReactiveProperty<IDataViewModel>(new NoneViewModel());
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
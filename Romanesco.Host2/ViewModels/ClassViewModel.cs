using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public class ClassViewModel : IDataViewModel
{
    private readonly Subject<Unit> _openDetailSubject = new();

    public string Title { get; }
    public IDataViewModel[] Children { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public IObservable<Unit> OpenDetail => _openDetailSubject;

    // ClassModelをClassViewModelに紐づけているように見せるために、引数でClassModelをとる必要がある
    // だが、Factoryの呼び出しをコンストラクタで行うのは微妙な気がする
    // ArrayViewModelはどうせViewModel内でFactoryを呼び出すのでFactoryを使えばいいかも
    public ClassViewModel(ClassModel model, IViewModelFactory factory)
    {
        Title = model.Title;
        Children = model.Children.Select(x => factory.Create(x, factory)).ToArray();
        DetailedData = Children.Select(x => x.OpenDetail.Select(_ => x))
            .Merge()
            .ToReadOnlyReactiveProperty(new NoneViewModel());
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
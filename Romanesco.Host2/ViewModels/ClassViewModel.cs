﻿using System;
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
    public PropertyViewModel[] Children { get; }
    public IReadOnlyReactiveProperty<IDataViewModel> DetailedData { get; }
    public IObservable<Unit> OpenDetail => _openDetailSubject;
    public ClassIdProvider? IdProvider { get; }

    // ClassModelをClassViewModelに紐づけているように見せるために、引数でClassModelをとる必要がある
    // だが、Factoryの呼び出しをコンストラクタで行うのは微妙な気がする
    // ArrayViewModelはどうせViewModel内でFactoryを呼び出すのでFactoryを使えばいいかも
    public ClassViewModel(ClassModel model, IViewModelFactory factory)
    {
        Children = model.Children
            .Select(x => new PropertyViewModel()
            {
                Model = x,
                Data = factory.Create(x, factory),
            })
            .ToArray();

        Title = model.Title;
        IdProvider = model.IdProvider;

        DetailedData = Children.Select(x => x.Data.OpenDetail.Select(_ => x.Data))
            .Merge()
            .ToReadOnlyReactiveProperty(new NoneViewModel());
    }

    public void Edit()
    {
        _openDetailSubject.OnNext(Unit.Default);
    }
}
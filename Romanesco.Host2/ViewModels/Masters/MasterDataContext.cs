using System.Collections.Generic;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;

namespace Romanesco.Host2.ViewModels;

internal class MasterDataContext
{
    private readonly Dictionary<string, ReactiveProperty<MasterData>> _namedMasters = new();

    public ReactiveProperty<MasterData> GetMaster(string masterName)
    {
        if (_namedMasters.TryGetValue(masterName, out var value))
        {
            return value;
        }

        _namedMasters[masterName] = new ReactiveProperty<MasterData>(new NullMasterData());
        return _namedMasters[masterName];
    }

    public void RegisterMaster(NamedArrayViewModel viewModel, string masterName)
    {
        var master = new InitializedMasterData()
        {
            Choices = viewModel.Items
        };

        if (_namedMasters.TryGetValue(masterName, out var value))
        {
            value.Value = master;
            return;
        }

        _namedMasters[masterName] = new ReactiveProperty<MasterData>(master);
    }
}

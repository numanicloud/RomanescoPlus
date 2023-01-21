using System.Collections.Generic;
using Reactive.Bindings;

namespace Romanesco.Host2.ViewModels;

internal class MasterDataContext
{
    private readonly Dictionary<string, ReactiveProperty<MasterDataZZ>> _namedMasters = new();

    public ReactiveProperty<MasterDataZZ> GetMaster(string masterName)
    {
        if (_namedMasters.TryGetValue(masterName, out var value))
        {
            return value;
        }

        _namedMasters[masterName] = new ReactiveProperty<MasterDataZZ>(new NullMasterDataZZ());
        return _namedMasters[masterName];
    }

    public void RegisterMaster(NamedArrayViewModel viewModel, string masterName)
    {
        var master = new InitializedMasterDataZZ()
        {
            Choices = viewModel.Items
        };

        if (_namedMasters.TryGetValue(masterName, out var value))
        {
            value.Value = master;
            return;
        }

        _namedMasters[masterName] = new ReactiveProperty<MasterDataZZ>(master);
    }
}

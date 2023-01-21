using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public interface IViewModelFactory
{
    IDataViewModel Create(IDataModel model, IViewModelFactory factory);
    IDataViewModel Create(PropertyModel model, IViewModelFactory factory);
}

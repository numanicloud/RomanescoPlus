using System.Linq;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public interface IViewModelFactory
{
    IDataViewModel Create(IDataModel model, IViewModelFactory factory);
}

internal class ViewModelFactory : IViewModelFactory
{
    public IDataViewModel Create(IDataModel model)
    {
        return Create(model, this);
    }

    public IDataViewModel Create(IDataModel model, IViewModelFactory factory)
    {
        if (model is IntModel intModel)
        {
            return new IntViewModel()
            {
                Model = intModel
            };
        }

        if (model is ClassModel classModel)
        {
            return new ClassViewModel(classModel, factory);
        }

        if (model is ArrayModel arrayModel)
        {
            return new ArrayViewModel(arrayModel, factory);
        }

        return new NoneViewModel();
    }
}
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
            return new ClassViewModel()
            {
                Title = classModel.Title,
                Children = classModel.Children.Select(x => Create(x, factory)).ToArray()
            };
        }

        return new NoneViewModel();
    }
}
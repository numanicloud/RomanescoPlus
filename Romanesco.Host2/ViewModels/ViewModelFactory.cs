using System.Linq;
using Romanesco.DataModel.Entities;

namespace Romanesco.Host2.ViewModels;

public interface IViewModelFactory
{
    IDataViewModel Create(IDataModel model, IViewModelFactory factory);
    IDataViewModel Create(PropertyModel model, IViewModelFactory factory);
}

internal class ViewModelFactory : IViewModelFactory
{
    public IDataViewModel Create(PropertyModel model)
    {
        return Create(model, this);
    }

    public IDataViewModel Create(IDataModel model)
    {
        return Create(model, this);
    }

    public IDataViewModel Create(PropertyModel model, IViewModelFactory factory)
    {
        return model.Model switch
        {
            ArrayModel { Prototype: ClassModel { EntryName: MutableEntryName } } arrayModel =>
                new NamedArrayViewModel(arrayModel, factory)
                {
                    EditorCommands = model.Commands
                },
            _ => Create(model.Model, factory)
        };
    }
    
    public IDataViewModel Create(IDataModel model, IViewModelFactory factory)
    {
        return model switch
        {
            IntModel intModel => new IntViewModel { Model = intModel },
            StringModel stringModel => new StringViewModel { Model = stringModel },
            FloatModel floatModel => new FloatViewModel { Model = floatModel },
            BoolModel boolModel => new BoolViewModel { Model = boolModel },
            ClassModel classModel => new ClassViewModel(classModel, factory),
            ArrayModel { Prototype: ClassModel { EntryName: MutableEntryName } } arrayModel =>
                new NamedArrayViewModel(arrayModel, factory),
            ArrayModel arrayModel => new ArrayViewModel(arrayModel, factory),
            _ => new NoneViewModel()
        };
    }
}
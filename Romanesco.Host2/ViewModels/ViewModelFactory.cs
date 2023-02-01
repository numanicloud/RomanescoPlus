using System.Linq;
using Reactive.Bindings;
using Romanesco.DataModel;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Entities.Component;
using RomanescoPlus.Annotations;

namespace Romanesco.Host2.ViewModels;

internal class ViewModelFactory : IViewModelFactory
{
    private readonly MasterDataContext _masterDataContext = new();

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
        if (model.Model is IntModel intModel)
        {
            var attr = model.Attributes
                .Select(x => x.Data)
                .OfType<EditorReferenceAttribute>()
                .FirstOrDefault();
            if (attr is not null)
            {
                return new IntIdReferenceViewModel(intModel,
                    _masterDataContext.GetMaster(attr.MasterName)
                        .ToReadOnlyReactiveProperty(new NullMasterData()));
            }
        }

        return model.Model switch
        {
            ArrayModel { Prototype: NamedClassModel } namedArray2 =>
                InstantiateNamedArray(namedArray2),
            ArrayModel { Prototype: ClassModel { EntryName: MutableEntryName } } namedArray =>
                InstantiateNamedArray(namedArray),
            _ => Create(model.Model, factory)
        };

        NamedArrayViewModel InstantiateNamedArray(ArrayModel namedArray)
        {
            var vm = new NamedArrayViewModel(namedArray, factory)
            {
                EditorCommands = model.Commands
            };

            model.Attributes
                .Select(x => x.Data)
                .OfType<EditorMasterAttribute>()
                .FirstOrDefault()
                .IfNotNull(attr =>
                {
                    _masterDataContext.RegisterMaster(vm, attr.MasterName);
                });

            return vm;
        }
    }

    public IDataViewModel Create(IDataModel model, IViewModelFactory factory)
    {
        return model switch
        {
            IntModel intModel => new IntViewModel { Model = intModel },
            StringModel stringModel => new StringViewModel { Model = stringModel },
            FloatModel floatModel => new FloatViewModel { Model = floatModel },
            BoolModel boolModel => new BoolViewModel { Model = boolModel },
            NamedClassModel namedClass => new NamedClassViewModel(namedClass, factory),
            ClassModel classModel => new ClassViewModel(classModel, factory),
            ArrayModel { Prototype: ClassModel { EntryName: MutableEntryName } } arrayModel =>
                new NamedArrayViewModel(arrayModel, factory),
            ArrayModel arrayModel => new ArrayViewModel(arrayModel, factory),
            _ => new NoneViewModel()
        };
    }
}
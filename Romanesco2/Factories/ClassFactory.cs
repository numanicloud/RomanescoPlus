using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Serialization;

namespace Romanesco2.DataModel.Factories;

internal class ClassFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsClass) return null;

        var props = from p in type.GetProperties()
            where p.GetIndexParameters().Length == 0
            select loader.LoadType(p.Name, p.PropertyType, loader);

        return new ClassModel()
        {
            TypeId = new TypeId(type),
            Title = title,
            Children = props.FilterNull().ToArray(),
        };
    }

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is not ClassModel model
            || model.Title != data.Label
            || data is not SerializedClass serialized) return null;

        return new ClassModel()
        {
            Title = model.Title,
            TypeId = model.TypeId,
            Children = model.Children
                .Select(x => serialized.Children.Select(y => loader.LoadValue(x, y, loader))
                    .FilterNull()
                    .FirstOrDefault() ?? x)    // セマンティクス的にはCloneすべき
                .ToArray()
        };
    }
}
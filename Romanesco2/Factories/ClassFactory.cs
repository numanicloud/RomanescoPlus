using Romanesco2.DataModel.Entities;

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
            Children = props.ToArray(),
        };
    }
}
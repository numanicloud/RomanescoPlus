using System.Reflection;
using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Factories;

internal class ClassFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (!type.IsClass) return null;

        var props = from p in type.GetProperties()
            where p.GetIndexParameters().Length == 0
            where p.CanRead && p.CanWrite
            let order = p.GetCustomAttribute<OrderAttribute>()?.Value ?? 0
            orderby order
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
            || data is not SerializedClass serialized) return null;

        return new ClassModel()
        {
            Title = model.Title,
            TypeId = model.TypeId,
            Children = model.Children
                .Select(x => LoadMember(x, serialized.Children, loader) ?? x.Clone())
                .ToArray()
        };

        static IDataModel? LoadMember(
            IDataModel targetMember,
            SerializedMember[] members,
            IModelFactory loader)
        {
            return members.FirstOrDefault(x => x.Label == targetMember.Title) is { } toLoad
                ? loader.LoadValue(targetMember, toLoad.Data, loader)
                : null;
        }
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        if (source is not ClassModel model
            || new TypeId(targetType) != model.TypeId) return null;

        var instance = Activator.CreateInstance(targetType);

        foreach (var child in model.Children)
        {
            if (targetType.GetProperty(child.Title) is not { } p)
                throw new InvalidOperationException();

            p.SetValue(instance, decoder.Decode(child, p.PropertyType, decoder));
        }

        return instance;
    }
}
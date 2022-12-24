using Romanesco2.DataModel.Entities;

namespace Romanesco2.DataModel.Factories;

internal class PrimitiveFactory : IModelFactory
{
    public IDataModel? LoadType(string title, Type type, IModelFactory loader)
    {
        if (type == typeof(int))
            return new IntModel()
            {
                Title = title,
            };

        if (type == typeof(bool))
            return new BoolModel()
            {
                Title = title
            };

        if (type == typeof(string))
            return new StringModel()
            {
                Title = title,
            };

        if (type == typeof(float))
            return new FloatModel()
            {
                Title = title
            };

        return null;
    }
}
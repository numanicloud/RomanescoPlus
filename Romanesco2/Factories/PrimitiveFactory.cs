using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

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

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        return target switch
        {
            IntModel iModel when data is SerializedInt iData => new IntModel()
            {
                Title = iModel.Title, Data = { Value = iData.Value },
            },
            BoolModel bModel when data is SerializedBool bData => new BoolModel()
            {
                Title = bModel.Title, Data = { Value = bData.Value }
            },
            StringModel sModel when data is SerializedString sData => new StringModel()
            {
                Title = sModel.Title, Data = { Value = sData.Value }
            },
            FloatModel fModel when data is SerializedFloat fData => new FloatModel()
            {
                Title = fModel.Title, Data = { Value = fData.Value }
            },
            _ => null
        };
    }

    public object? Decode(IDataModel source, Type targetType, IModelFactory decoder)
    {
        return source switch
        {
            IntModel i when targetType == typeof(int) => i.Data.Value,
            BoolModel b when targetType == typeof(bool) => b.Data.Value,
            StringModel s when targetType == typeof(string) => s.Data.Value,
            FloatModel f when targetType == typeof(float) => f.Data.Value,
            _ => null
        };
    }
}
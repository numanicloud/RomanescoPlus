using Romanesco2.DataModel.Entities;
using Romanesco2.DataModel.Serialization;

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

    public IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader)
    {
        if (target is IntModel iModel && data is SerializedInt iData)
            return new IntModel()
            {
                Title = iModel.Title,
                Data = { Value = iData.Value },
            };

        if (target is BoolModel bModel && data is SerializedBool bData)
            return new BoolModel()
            {
                Title = bModel.Title,
                Data = { Value = bData.Value }
            };
        
        if (target is StringModel sModel && data is SerializedString sData)
            return new StringModel()
            {
                Title = sModel.Title,
                Data = { Value = sData.Value }
            };

        if (target is FloatModel fModel && data is SerializedFloat fData)
            return new FloatModel()
            {
                Title = fModel.Title,
                Data = { Value = fData.Value }
            };

        return null;
    }
}
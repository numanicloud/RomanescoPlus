using Romanesco.DataModel.Entities;
using Romanesco.DataModel.Serialization;

namespace Romanesco.DataModel.Factories;

// IModelManipulator みたいな名前の方が妥当？
public interface IModelFactory
{
    // MakeModel
    IDataModel? LoadType(string title, Type type, IModelFactory loader);

    // MergeRecord
    // シリアライズ値を読み込んだくらいでCloneするようでは挙動の理解が余分に難しすぎるかも。MergeというよりLoadな感じにしたい
    IDataModel? LoadValue(IDataModel target, SerializedData data, IModelFactory loader);
    
    // EmitRecord
    SerializedData? MakeData(IDataModel model, IModelFactory factory);

    // EmitValue
    object? Decode(IDataModel source, Type targetType, IModelFactory decoder);

    // LoadValue
    bool LoadRawValue(IDataModel source, object rawValue, IModelFactory loader);
}
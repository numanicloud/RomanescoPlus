using Romanesco.DataModel.Factories;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Entities.Component;

public sealed class SubtypingContext
{
    private readonly Dictionary<TypeId, TypeId[]> _idToDerived = new();
    private readonly Dictionary<TypeId, ClassModel> _idToPrototype = new();
    
    public required ClassFactory ClassFactory { get; init; }

    public TypeId[]? LoadUnionType(Type baseType, IModelFactory loader)
    {
        // キャッシュを使う
        var typeId = new TypeId(baseType);
        if (_idToDerived.TryGetValue(typeId, out var result))
        {
            return result;
        }

        // 属性をチェック
        var subtypes = from attr in baseType.GetCustomAttributes(false)
            let attrType = attr.GetType()
            where attrType.GetGenericTypeDefinition() == typeof(EditorUnionAttribute<>)
            let type = attrType.GenericTypeArguments[0]
            select (id: new TypeId(type), type);

        // 属性がついていなければ終了
        var subTypeArray = subtypes.ToArray();
        if (!subTypeArray.Any())
        {
            return null;
        }

        // プロトタイプをキャッシュ
        foreach (var (id, type) in subTypeArray)
        {
            var prototype = ClassFactory.LoadType("Prototype", type, loader);
            if (prototype is ClassModel cm)
            {
                _idToPrototype[id] = cm;
            }
        }

        // 派生クラスをキャッシュ
        var typeIdArray = subTypeArray.Select(x => x.id).ToArray();
        _idToDerived[typeId] = typeIdArray;
        return typeIdArray;
    }

    public ClassModel Instantiate(TypeId typeId, string? title)
    {
        return _idToPrototype[typeId].CloneStrict(null);
    }
}
namespace Romanesco.DataModel.Entities;

public class NamedArrayModel : IDataModel
{
    // 中身がNamedArrayModelであることが保証できるのだから、ArrayModel型で覆い隠すのは実際の型を分かりづらくするだけ
    public ArrayModel Inner { get; }

    public string Title => Inner.Title;

    public NamedArrayModel(ArrayModel inner)
    {
        if (inner.Prototype is not NamedClassModel)
        {
            throw new InvalidOperationException();
        }

        Inner = inner;
    }

    public IDataModel Clone(string? title = null)
    {
        return new NamedArrayModel(Inner.Clone(title) as ArrayModel ?? throw new Exception());
    }

    public void New()
    {
        Inner.New();
    }

    public void Remove(NamedClassModel item)
    {
        throw new NotImplementedException();
    }

    public void MoveUp(NamedClassModel item)
    {
        throw new NotImplementedException();
    }

    public void MoveDown(NamedClassModel item)
    {
        throw new NotImplementedException();
    }

    public void Duplicate(NamedClassModel item)
    {
        throw new NotImplementedException();
    }
}

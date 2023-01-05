using RomanescoPlus.Annotations;

namespace Romanesco.Try;

[EditorRoot]
public class MyRootType
{
    public int X { get; set; }
    public AnotherType Another { get; set; }
}

public class AnotherType
{
    public int Y { get; set; }
}
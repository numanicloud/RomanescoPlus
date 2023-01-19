using RomanescoPlus.Annotations;

namespace Romanesco.Try;

[EditorRoot]
public class MyRootType
{
    public int X { get; set; }
    public AnotherType Another { get; set; }
    public int[] Array { get; set; }
    public NamedClass[] Master { get; set; }
}

public class AnotherType
{
    public int Y { get; set; }
}

public class NamedClass
{
    [EditorName] public string Name { get; set; } = "DefaultName";
    public int Value { get; set; }
}
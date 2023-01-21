using RomanescoPlus.Annotations;

namespace Romanesco.Try;

[EditorRoot]
public class MyRootType
{
    public int X { get; set; }
    public bool Boolean { get; set; }
    public float Float { get; set; }
    public AnotherType Another { get; set; }
    public int[] Array { get; set; }
    
    [EditorCommandTarget(nameof(ApplyId))]
    public NamedClass[] Master { get; set; }
    
    public static NamedClass[] ApplyId(NamedClass[] self)
    {
        var constant = self.GroupBy(x => x.Id)
            .Where(x => !x.Skip(1).Any())
            .SelectMany(x => x)
            .ToArray();

        var targets = self.Except(constant);
        var nextId = constant.Any() ? constant.Max(x => x.Id) + 1 : 1;
        foreach (var item in targets)
        {
            item.Id = nextId++;
        }

        return self;
    }
}

public class AnotherType
{
    public int Y { get; set; }
}

public class NamedClass
{
    public int Id { get; set; }
    [EditorName] public string Name { get; set; } = "DefaultName";
    public int Value { get; set; }
}
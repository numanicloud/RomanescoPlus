using System.Text.Json.Serialization;

namespace Romanesco.DataModel.Serialization;

[JsonDerivedType(typeof(SerializedInt), "int")]
[JsonDerivedType(typeof(SerializedBool), "bool")]
[JsonDerivedType(typeof(SerializedString), "string")]
[JsonDerivedType(typeof(SerializedFloat), "float")]
[JsonDerivedType(typeof(SerializedClass), "class")]
[JsonDerivedType(typeof(SerializedArray), "array")]
public abstract class SerializedData
{
    public SerializedData()
    {
    }
}

public class SerializedInt : SerializedData
{
    public int Value { get; set; }

    public SerializedInt()
    {
    }

    public override string ToString() => Value.ToString();
}

public class SerializedBool : SerializedData
{
    public bool Value { get; set; }

    public SerializedBool()
    {
    }
    
    public override string ToString() => Value.ToString();
}

public class SerializedString : SerializedData
{
    public string Value { get; set; }

    public SerializedString()
    {
    }

    public override string ToString() => Value;
}

public class SerializedFloat : SerializedData
{
    public float Value { get; set; }

    public SerializedFloat()
    {
    }

    public override string ToString() => Value.ToString();
}

public class SerializedMember
{
    public string Label { get; set; }
    public SerializedData Data { get; set; }

    public SerializedMember()
    {
    }

    public override string ToString() => $"{Label} = {Data}";
}

public class SerializedClass : SerializedData
{
    public SerializedMember[] Children { get; set; }

    public SerializedClass()
    {
    }

    public override string ToString()
    {
        return "{" + string.Join(", ", Children.Select(x => x.ToString())) + "}";
    }
}

public class SerializedArray : SerializedData
{
    public SerializedData[] Items { get; set; }

    public SerializedArray()
    {
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", Items.Select(x => x.ToString())) + "]";
    }
}
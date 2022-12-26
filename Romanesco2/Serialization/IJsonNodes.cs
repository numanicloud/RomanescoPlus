namespace Romanesco2.DataModel.Serialization;

public abstract class SerializedData
{
}

public class SerializedInt : SerializedData
{
    public int Value { get; set; }

    public override string ToString() => Value.ToString();
}

public class SerializedBool : SerializedData
{
    public bool Value { get; set; }
    
    public override string ToString() => Value.ToString();
}

public class SerializedString : SerializedData
{
    public string Value { get; set; }
    public override string ToString() => Value;
}

public class SerializedFloat : SerializedData
{
    public float Value { get; set; }
    public override string ToString() => Value.ToString();
}

public class SerializedMember
{
    public string Label { get; set; }
    public SerializedData Data { get; set; }

    public override string ToString() => $"{Label} = {Data}";
}

public class SerializedClass : SerializedData
{
    public SerializedMember[] Children { get; set; }

    public override string ToString()
    {
        return "{" + string.Join(", ", Children.Select(x => x.ToString())) + "}";
    }
}

public class SerializedArray : SerializedData
{
    public SerializedData[] Items { get; set; }

    public override string ToString()
    {
        return "[" + string.Join(", ", Items.Select(x => x.ToString())) + "]";
    }
}
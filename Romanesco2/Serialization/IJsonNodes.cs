using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Romanesco2.DataModel.Serialization;

public abstract class SerializedData
{
    public string Label { get; set; } = "";
}

public class SerializedInt : SerializedData
{
    public int Value { get; set; }
}

public class SerializedBool : SerializedData
{
    public bool Value { get; set; }
}

public class SerializedString : SerializedData
{
    public string Value { get; set; }
}

public class SerializedFloat : SerializedData
{
    public float Value { get; set; }
}

public class SerializedClass : SerializedData
{
    public SerializedData[] Children { get; set; }
}

public class SerializedArray : SerializedData
{
    public SerializedData[] Items { get; set; }
}
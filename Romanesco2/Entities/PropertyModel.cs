﻿namespace Romanesco.DataModel.Entities;

public class PropertyModel
{
    public IEnumerable<ModelAttributeData> Attributes { get; init; } = Array.Empty<ModelAttributeData>();
    public required IDataModel Model { get; init; }

    public PropertyModel Clone()
    {
        return new PropertyModel()
        {
            Attributes = Attributes.ToArray(),
            Model = Model.Clone()
        };
    }
}
using System;

[AttributeUsage(AttributeTargets.Class)]
public class MetadataAttribute : Attribute
{
    public string typeId;

    public MetadataAttribute(string typeId)
    {
        this.typeId = typeId;
    }
}
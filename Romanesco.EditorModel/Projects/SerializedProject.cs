using Romanesco.DataModel.Serialization;

namespace Romanesco.EditorModel.Projects;

public class SerializedProject
{
    public string DllPath { get; set; }
    public SerializedData Data { get; set; }
}
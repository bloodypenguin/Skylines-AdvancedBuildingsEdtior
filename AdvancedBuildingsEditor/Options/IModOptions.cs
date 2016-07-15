using System.Xml.Serialization;

namespace AdvancedBuildingsEditor.Options
{
    public interface IModOptions
    {
        [XmlIgnore]
        string FileName
        {
            get;
        }
    }
}
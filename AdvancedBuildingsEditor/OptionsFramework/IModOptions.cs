using System.Xml.Serialization;

namespace AdvancedBuildingsEditor.OptionsFramework
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
using AdvancedBuildingsEditor.Options;

namespace AdvancedBuildingsEditor
{
    public class ModOptions : IModOptions
    {

        public ModOptions()
        {
            SubBuildingsFormat = (int) SubBuildingsFormats.Native;
        }

        [DropDown("Sub-buildings format", nameof(SubBuildingsFormats))]
        public int SubBuildingsFormat { get; private set; }

        public string FileName => "CSL-AdvancedBuildingsEditor";
    }
}
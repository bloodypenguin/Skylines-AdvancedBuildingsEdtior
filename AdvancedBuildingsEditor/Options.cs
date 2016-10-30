using AdvancedBuildingsEditor.OptionsFramework;

namespace AdvancedBuildingsEditor
{
    public class Options : IModOptions
    {

        public Options()
        {
            SubBuildingsFormat = (int) SubBuildingsFormats.Native;
            PreciseSpecialPointsPostions = false;
        }

        [DropDown("Sub-buildings save format", nameof(SubBuildingsFormats))]
        public int SubBuildingsFormat { get; private set; }

        [Checkbox("Save precise special points positions (use m_editPrefabInfo values instead of marker props)")]
        public bool PreciseSpecialPointsPostions { get; private set; }

        public string FileName => "CSL-AdvancedBuildingsEditor";
    }
}
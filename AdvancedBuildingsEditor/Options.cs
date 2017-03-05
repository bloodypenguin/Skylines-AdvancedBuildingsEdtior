using AdvancedBuildingsEditor.OptionsFramework;

namespace AdvancedBuildingsEditor
{
    public class Options : IModOptions
    {

        public Options()
        {
            PreciseSpecialPointsPostions = false;
            PrecisePathsPostions = false;
        }

        [Checkbox("Save precise special points positions (use m_editPrefabInfo values instead of marker props)")]
        public bool PreciseSpecialPointsPostions { get; private set; }

        [Checkbox("Save precise paths positions (use m_editPrefabInfo values instead of placed networks)")]
        public bool PrecisePathsPostions { get; private set; }

        public string FileName => "CSL-AdvancedBuildingsEditor";
    }
}
using AdvancedBuildingsEditor.OptionsFramework;

namespace AdvancedBuildingsEditor
{
    public class Options : IModOptions
    {

        public Options()
        {
            PreciseSpecialPointsPostions = false;
        }

        [Checkbox("Save precise special points positions (use m_editPrefabInfo values instead of marker props)")]
        public bool PreciseSpecialPointsPostions { get; private set; }

        public string FileName => "CSL-AdvancedBuildingsEditor";
    }
}
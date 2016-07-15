using AdvancedBuildingsEditor.Options;

namespace AdvancedBuildingsEditor
{
    public class ModOptions : IModOptions
    {

        public ModOptions()
        {
            SubBuildingsFormat = (int) SubBuildingsFormats.Native;
            PreciseSpecialPointsPostions = false;
        }

        [DropDown("Sub-buildings save format", nameof(SubBuildingsFormats))]
        public int SubBuildingsFormat { get; private set; }

        [Checkbox("Save precise special points positions")]
        public bool PreciseSpecialPointsPostions { get; private set; }

        public string FileName => "CSL-AdvancedBuildingsEditor";
    }
}
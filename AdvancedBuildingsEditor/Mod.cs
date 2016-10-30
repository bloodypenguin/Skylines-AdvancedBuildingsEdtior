using AdvancedBuildingsEditor.Options;
using ICities;

namespace AdvancedBuildingsEditor
{
    public class Mod : IUserMod
    {
        public string Name => "Advanced Buildings Editor";
        public string Description => "Advanced Buildings Editor";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<ModOptions>();
            helper.AddButton("Refresh Sub-Buildings Editor Definitions",
                SubBuildingsEnablerFormat.InitializeBuildingsWithSubBuildings);
        }
    }
}

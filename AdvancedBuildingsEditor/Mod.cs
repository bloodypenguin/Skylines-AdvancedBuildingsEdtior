using AdvancedBuildingsEditor.OptionsFramework;
using ICities;

namespace AdvancedBuildingsEditor
{
    public class Mod : IUserMod
    {
        public string Name => "Advanced Buildings Editor";
        public string Description => "Advanced Buildings Editor";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
            helper.AddButton("Refresh Sub-Buildings Editor Definitions",
                SubBuildingsEnablerFormat.InitializeBuildingsWithSubBuildings);
        }
    }
}

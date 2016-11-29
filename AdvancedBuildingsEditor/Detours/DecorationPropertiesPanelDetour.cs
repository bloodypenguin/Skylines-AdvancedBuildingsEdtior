using System.Runtime.CompilerServices;
using AdvancedBuildingsEditor.Redirection;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(DecorationPropertiesPanel))]
    public class DecorationPropertiesPanelDetour : DecorationPropertiesPanel
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectReverse]
        public static void CreateSubBuildings(DecorationPropertiesPanel panel, BuildingInfo info)
        {
            UnityEngine.Debug.Log("Lalala");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectReverse]
        public static void ClearSubBuildings(DecorationPropertiesPanel panel)
        {
            UnityEngine.Debug.Log("Lalala");
        }
    }
}
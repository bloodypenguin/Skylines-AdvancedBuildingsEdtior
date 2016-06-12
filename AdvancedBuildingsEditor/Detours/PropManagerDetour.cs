using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(PropManager))]
    public class PropManagerDetour : PropManager
    {
        //TODO(earalov): add similar detour for tree manager
        [RedirectMethod]
        public bool CheckLimits()
        {
            //begin mod
            if (BuildingDecorationDetour.DisableLimits)
            {
                return true;
            }
            var propTool = ToolsModifierControl.GetCurrentTool<PropTool>();
            if (propTool != null)
            {
                if (SpecialPoints.IsSpecialProp(propTool.m_prefab))
                {
                    return true;
                }
            }
            //end mod
            ItemClass.Availability availability = Singleton<ToolManager>.instance.m_properties.m_mode;
            if ((availability & ItemClass.Availability.MapEditor) != ItemClass.Availability.None)
            {
                if (this.m_propCount >= 50000)
                    return false;
            }
            else if ((availability & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                if (this.m_propCount + Singleton<TreeManager>.instance.m_treeCount >= 64)
                    return false;
            }
            else if (this.m_propCount >= 65531)
                return false;
            return true;
        }
    }
}
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(TreeManager))]
    public class TreeManagerDetour : TreeManager
    {
        [RedirectMethod]
        public bool CheckLimits()
        {
            //begin mod
            if (BuildingDecorationDetour.DisableLimits)
            {
                return true;
            }
            //end mod
            ItemClass.Availability availability = Singleton<ToolManager>.instance.m_properties.m_mode;
            if ((availability & ItemClass.Availability.MapEditor) != ItemClass.Availability.None)
            {
                if (this.m_treeCount >= 250000)
                    return false;
            }
            else if ((availability & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                //begin mod
                if (this.m_treeCount + Singleton<PropManager>.instance.m_propCount >= 64 + SpecialPoints.CountSpecialPoints())
                    return false;
                //end mod
            }
            else if (this.m_treeCount >= 262139)
                return false;
            return true;
        }
    }
}
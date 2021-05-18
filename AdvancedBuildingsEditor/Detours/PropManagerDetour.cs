using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(PropManager))]
    public class PropManagerDetour : PropManager
    {
        [RedirectMethod]
        public new bool CheckLimits()
        {
            //begin mod
            if (BuildingDecorationDetour.DisableLimits)
            {
                return true;
            }
            var propTool = ToolsModifierControl.GetCurrentTool<PropTool>();
            if (propTool != null)
            {
                var pointType = SpecialPoints.GetSpecialPointType(propTool.m_prefab);
                if (pointType != SpecialPointType.Unknown)
                {
                    if (!Panel.specialPointTypeCount.TryGetValue(pointType, out var pointTypeCount))
                    {
                        pointTypeCount = 9999;
                    }
                    return pointTypeCount < SpecialPoints.GetMaxNumberOfPoints(ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo, pointType);  
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
                //begin mod
                if (this.m_propCount + Singleton<TreeManager>.instance.m_treeCount >= 64 + SpecialPoints.CountSpecialPoints())
                    return false;
                //end mod
            }
            else if (this.m_propCount >= 65531)
                return false;
            return true;
        }
    }
}
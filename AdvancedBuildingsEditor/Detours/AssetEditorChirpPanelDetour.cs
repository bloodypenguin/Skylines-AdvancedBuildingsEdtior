using System.Reflection;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(AssetEditorChirpPanel))]
    public class AssetEditorChirpPanelDetour : AssetEditorChirpPanel
    {
        [RedirectMethod]
        private void UpdateHeader()
        {
            if ((UnityEngine.Object)ToolsModifierControl.GetCurrentTool<TreeTool>() != (UnityEngine.Object)null)
            {
                if (!this.m_Header.isVisible)
                    this.ShowHeader();
                if (!Singleton<TreeManager>.exists)
                    return;
                //begin mod
                this.m_Header.text = string.Format(Locale.Get("CHIRPHEADER_TREESNPROPS"), (object)(Singleton<TreeManager>.instance.m_treeCount + Singleton<PropManager>.instance.m_propCount - SpecialPoints.CountSpecialPoints()), (object)64.ToString());
                //end mod
            }
            else if ((UnityEngine.Object)ToolsModifierControl.GetCurrentTool<PropTool>() != (UnityEngine.Object)null)
            {
                if (!this.m_Header.isVisible)
                    this.ShowHeader();
                if (!Singleton<PropManager>.exists)
                    return;
                //begin mod
                this.m_Header.text = string.Format(Locale.Get("CHIRPHEADER_TREESNPROPS"), (object)(Singleton<PropManager>.instance.m_propCount + Singleton<TreeManager>.instance.m_treeCount - SpecialPoints.CountSpecialPoints()), (object)64.ToString());
                //end mod    
            }
            else
                this.HideHeader();
        }

        private UILabel m_Header => (UILabel)typeof(AssetEditorChirpPanel).GetField("m_Header", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(this);
    }
}
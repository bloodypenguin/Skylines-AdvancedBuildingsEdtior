using System.Collections.Generic;
using AdvancedBuildingsEditor.Detours;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public class Panel : UIPanel
    {

        private PropInfo cachedPropToolProp = null;
        private int cachedPropCount = -1;
        public static Dictionary<SpecialPointType, int> specialPointTypeCount = new();
        public override void Start()
        {
            base.Start();
            specialPointTypeCount = new Dictionary<SpecialPointType, int>();
            cachedPropCount = -1;
            this.name = "AdvancedBuildingsEditor";
            this.width = 230f;
            this.height = 222f;
            this.backgroundSprite = "MenuPanel2";
            this.canFocus = true;
            this.isInteractive = true;
            this.isVisible = true;
            this.relativePosition = new Vector3(10f, 10f);
            UILabel uiLabel = this.AddUIComponent<UILabel>();
            uiLabel.name = "Title";
            uiLabel.text = "Advanced Buildings Editor";
            uiLabel.textAlignment = UIHorizontalAlignment.Center;
            uiLabel.position = new Vector3((float)((double)this.width / 2.0 - (double)uiLabel.width / 2.0), (float)((double)uiLabel.height / 2.0 - 20.0));
            UIPanel uiPanel1 = this.AddUIComponent<UIPanel>();
            uiPanel1.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            uiPanel1.transform.localPosition = Vector3.zero;
            uiPanel1.width = this.width;
            uiPanel1.height = this.height - 50f;
            uiPanel1.autoLayout = true;
            uiPanel1.autoLayoutDirection = LayoutDirection.Vertical;
            uiPanel1.autoLayoutPadding = new RectOffset(0, 0, 0, 1);
            uiPanel1.autoLayoutStart = LayoutStart.TopLeft;
            uiPanel1.relativePosition = new Vector3(6f, 50f);
            UIPanel uiPanel2 = uiPanel1.AddUIComponent<UIPanel>();
            string str1 = "SelectionPanel";
            uiPanel2.name = str1;
            double num1 = (double)uiPanel1.width;
            uiPanel2.width = (float)num1;
            double num2 = 30.0;
            uiPanel2.height = (float)num2;
            int num3 = 1;
            uiPanel2.autoLayout = num3 != 0;
            int num4 = 0;
            uiPanel2.autoLayoutDirection = (LayoutDirection)num4;
            RectOffset rectOffset1 = new RectOffset(0, 5, 0, 0);
            uiPanel2.autoLayoutPadding = rectOffset1;

            var bcButton = UIUtil.CreateButton(this, "Bulldoze Ped. Connections");
            bcButton.relativePosition = new Vector3(5, 40);
            bcButton.eventClick +=
            (comp, param) =>
            {
                Scripts.BulldozePedestrianConnections();
            };
            var seButton = UIUtil.CreateButton(this, "Make All Segments Editable");
            seButton.relativePosition = new Vector3(5, 66);
            seButton.eventClick += (comp, param) =>
            {
                SimulationManager.instance.AddAction(() =>
                {
                    Scripts.MakeAllSegmentsEditable();
                });
            };

            var clearSpecialPointsButton = UIUtil.CreateButton(this, "Clear spawn points");
            clearSpecialPointsButton.relativePosition = new Vector3(5, 92);
            clearSpecialPointsButton.eventClicked += (component, param) =>
            {
                SimulationManager.instance.AddAction(() =>
                {
                    Scripts.ClearProps(true);
                });
            };

            var clearPropsButton = UIUtil.CreateButton(this, "Clear props");
            clearPropsButton.relativePosition = new Vector3(5, 118);
            clearPropsButton.eventClicked += (component, param) =>
            {
                SimulationManager.instance.AddAction(() =>
                {
                    Scripts.ClearProps();
                });
            };

            var reloadDecorationsButton = UIUtil.CreateButton(this, "Reload Decorations");
            reloadDecorationsButton.relativePosition = new Vector3(5, 144);
            reloadDecorationsButton.eventClicked += (component, param) =>
            {
                SimulationManager.instance.AddAction(() =>
                {
                    TerrainModify.UpdateArea(-500f, -500f, 500f, 500f, true, true, true);
                    BuildingDecoration.ClearDecorations();
                });

                SimulationManager.instance.AddAction(() =>
                {
                    TerrainModify.UpdateArea(-500f, -500f, 500f, 500f, true, true, true);
                    BuildingDecoration.LoadDecorations((BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo);
                });
            };

            var autoPlaceSpecialPointsButton = UIUtil.CreateButton(this, "Auto-place spawn points");
            autoPlaceSpecialPointsButton.relativePosition = new Vector3(5, 170);
            autoPlaceSpecialPointsButton.eventClicked += (component, param) =>
            {
                SimulationManager.instance.AddAction(Scripts.AutoPlaceSpecialPoints);
            };

            var addSubBuildingButton = UIUtil.CreateButton(this, "Reload sub-buildings");
            addSubBuildingButton.relativePosition = new Vector3(5, 196);
            addSubBuildingButton.eventClicked += (component, param) =>
            {
                var panel = GameObject.FindObjectOfType<DecorationPropertiesPanel>();
                SimulationManager.instance.AddAction(() =>
                {
                    TerrainModify.UpdateArea(-500f, -500f, 500f, 500f, true, true, true);
                    DecorationPropertiesPanelDetour.ClearBuildings(panel);
                });
                SimulationManager.instance.AddAction(() =>
                {
                    TerrainModify.UpdateArea(-500f, -500f, 500f, 500f, true, true, true);
                    DecorationPropertiesPanelDetour.CreateSubBuildings(panel, (BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo);
                });
            };
        }

        public override void Update()
        {
            isVisible = ToolsModifierControl.toolController.m_editPrefabInfo is BuildingInfo;
            if (!isVisible)
            {
                cachedPropCount = -1;
                cachedPropToolProp = null;
                return;
            }

            var propTool = ToolsModifierControl.toolController.CurrentTool as PropTool;
            if (propTool == null)
            {
                cachedPropToolProp = null;
            }
            else if (cachedPropToolProp != propTool.m_prefab || cachedPropCount != PropManager.instance.m_propCount)
            {
                var specialPointType = SpecialPoints.GetSpecialPointType(propTool.m_prefab);
                if (specialPointType != SpecialPointType.Unknown)
                {
                    specialPointTypeCount[specialPointType] = SpecialPoints.CountSpecialPoints(specialPointType);
                }
                cachedPropToolProp = propTool.m_prefab;
                cachedPropCount = PropManager.instance.m_propCount;
            }
            
            if (Input.GetKey(KeyCode.Escape))
            {
                if (ToolsModifierControl.GetCurrentTool<BuildingTool>() == null)
                {
                    return;
                }
                ToolsModifierControl.SetTool<DefaultTool>();
                ToolsModifierControl.GetTool<BuildingTool>().m_prefab = null;
            }
        }
    }
}

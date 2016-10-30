using System.Collections.Generic;
using AdvancedBuildingsEditor.Detours;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public static class Scripts
    {

        public static void MakeAllSegmentsEditable(bool makeAllNetworksEditable = true)
        {
            var mgr = NetManager.instance;
            for (var i = 0; i < mgr.m_segments.m_size; i++)
            {
                if (mgr.m_segments.m_buffer[i].m_flags == NetSegment.Flags.None)
                {
                    continue;
                }
                Debug.Log($"Segment {i} -  Type: {mgr.m_segments.m_buffer[i].Info.name}, Length: {mgr.m_segments.m_buffer[i].m_averageLength}");
                if (makeAllNetworksEditable)
                {
                    mgr.m_segments.m_buffer[i].m_flags &= ~NetSegment.Flags.Untouchable;
                }
            }
        }

        public static void BulldozePedestrianConnections()
        {
            var mgr = NetManager.instance;
            for (ushort i = 0; i < mgr.m_segments.m_size; i++)
            {
                var netSegment = mgr.m_segments.m_buffer[i];
                if (netSegment.m_flags == NetSegment.Flags.None) continue;

                var name = netSegment.Info.name;
                if (name.Contains("Pedestrian Connection") /*|| name == "Cargo Connection" ||
                    name == "Ship Dock" || name == "Ship Dockway" || name == "Bus Station Stop" || name == "Bus Station Way"*/)
                {
                    mgr.ReleaseSegment(i, false);
                }
            }
        }

        public static void ClearProps(bool specialPoints = false)
        {
            var instance = PropManager.instance;
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var isSpecialPoint = SpecialPoints.IsSpecialPoint(propInstance.Info);
                if (isSpecialPoint == specialPoints)
                {
                    instance.ReleaseProp(index1);
                }
            }
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
        }

        public static void RecalculateSpecialPoints()
        {
            PropManager instance = PropManager.instance;
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var propInfo = propInstance.Info;
                if (SpecialPoints.IsSpecialPoint(propInfo))
                {
                    BuildingDecorationDetour.SpecialPoints.Add(index1, SpecialPoints.GetSpecialPointType(propInfo));
                }
            }
        }

        public static void AutoPlaceSpecialPoints()
        {
            ClearProps(true);
            var instance = NetManager.instance;
            int counter = 0;
            var canInvert = true;
            if (ToolsModifierControl.toolController.m_editPrefabInfo.GetAI() is CargoStationAI)
            {
                canInvert = ((CargoStationAI)ToolsModifierControl.toolController.m_editPrefabInfo.GetAI()).m_canInvertTarget;
            }
            else if (ToolsModifierControl.toolController.m_editPrefabInfo.GetAI() is DepotAI)
            {
                canInvert = ((DepotAI)ToolsModifierControl.toolController.m_editPrefabInfo.GetAI()).m_canInvertTarget;
            }
            else
            {
                return;
            }
            foreach (var netSegment in instance.m_segments.m_buffer)
            {
                if (netSegment.m_flags == NetSegment.Flags.None || netSegment.Info == null)
                {
                    continue;
                }
                var startNode = instance.m_nodes.m_buffer[netSegment.m_startNode].m_position;
                var endNode = instance.m_nodes.m_buffer[netSegment.m_endNode].m_position;
                var middle = new Vector3((startNode.x + endNode.x) / 2, (startNode.y + endNode.y) / 2, (startNode.z + endNode.z) / 2);
                var name = netSegment.Info.name;
                if ((name == "Bus Station Stop" && name != "Bus Station Way") || name.Contains("Station") || name == "Airplane Stop" || name.Contains("Train Cargo Track"))
                {
                    if (counter == 0 || !name.Contains("Train Cargo Track"))
                    {
                        if (canInvert)
                        {
                            CreateSpecialPoint(PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPointPosition), middle);
                        }
                        CreateSpecialPoint(PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPointTarget), middle);
                    }
                    else if (counter == 1)
                    {
                        if (canInvert)
                        {
                            CreateSpecialPoint(PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPoint2Position), middle);
                        }
                        CreateSpecialPoint(PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPoint2Target), middle);
                    }
                    counter++;
                }
            }
        }


        public static void CreateSpecialPoint(PropInfo info, Vector3 position)
        {
            ushort prop;
            if (PropManager.instance.CreateProp(out prop, ref Singleton<SimulationManager>.instance.m_randomizer, info,
                position, 0, true))
            {
                PropManager.instance.m_props.m_buffer[(int)prop].FixedHeight = true;
            }
        }
    }
}
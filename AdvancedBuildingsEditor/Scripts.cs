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
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var isSpecialPoint = SpecialPoints.GetSpecialPointType(propInstance.Info) != SpecialPointType.Unknown;
                if (isSpecialPoint == specialPoints)
                {
                    instance.ReleaseProp(index1);
                }
            }
        }



        public static void AutoPlaceSpecialPoints()
        {
            ClearProps(true);
            var instance = NetManager.instance;
            var counter = 0;
            bool canInvert;
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.GetAI() is CargoStationAI)
            {
                canInvert = ((CargoStationAI)buildingInfo.GetAI()).m_canInvertTarget;
            }
            else if (buildingInfo?.GetAI() is DepotAI)
            {
                canInvert = ((DepotAI)buildingInfo.GetAI()).m_canInvertTarget;
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
                if (name == "Bus Station Way")
                {
                    continue;
                }
                if (!name.Contains("Station") && name != "Airplane Stop" && !name.Contains("Train Cargo Track"))
                    continue;
                if (name.Contains("Train Cargo Track") && counter > 0)
                {
                    if (counter == 1)
                    {
                        if (canInvert)
                        {
                            CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPoint2Position, middle);
                        }
                        CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPoint2Target, middle);
                    }
                }
                else
                {
                    if (canInvert)
                    {
                        CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointPosition, middle);
                    }
                    CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointTarget, middle);
                }

                counter++;
            }
        }


        public static void CreateSpecialPoint(BuildingInfo buildingInfo, SpecialPointType pointType, Vector3 position)
        {
            var info = SpecialPoints.GetSpecialPointProp(pointType);
            if (info == null || info.m_prefabDataIndex == -1)
            {
                return;
            }
            if (!SpecialPoints.IsAppropriatePointType(buildingInfo, pointType))
            {
                return;
            }
            ushort prop;
            if (PropManager.instance.CreateProp(out prop, ref Singleton<SimulationManager>.instance.m_randomizer, info,
                position, 0, true))
            {
                PropManager.instance.m_props.m_buffer[(int)prop].FixedHeight = true;
            }
        }
    }
}
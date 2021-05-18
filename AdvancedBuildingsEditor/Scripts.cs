using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingsEditor.Detours;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public static class Scripts
    {

        public static void MakeAllSegmentsEditable()
        {
            var mgr = NetManager.instance;
            for (var i = 0; i < mgr.m_segments.m_size; i++)
            {
                if (mgr.m_segments.m_buffer[i].m_flags == NetSegment.Flags.None)
                {
                    continue;
                }
                mgr.m_segments.m_buffer[i].m_flags &= ~NetSegment.Flags.Untouchable;
            }

            MakeConnectionsEditable();
        }

        private static void MakeConnectionsEditable()
        {
            var prefabNames = new string[]
            {
                "Pedestrian Connection",
                "Pedestrian Connection Surface",
                "Pedestrian Connection Inside",
                "Pedestrian Connection Underground",
                "Cargo Connection",
                "Ferry Path",
                "Ferry Dock",
                "Ferry Dockway",
                "Ship Dock",
                "Ship Dockway",
                "Fishing Dockway",
                "Helicopter Path",
                "Helicopter Stop",
                "Blimp Path",
                "Blimp Stop"
            };
            foreach (var prefabName in prefabNames)
            {
                var prefab = PrefabCollection<NetInfo>.FindLoaded(prefabName);
                if (prefab == null)
                {
                    continue;
                }
                if ("Cargo Connection".Equals(prefabName))
                {
                    prefab.m_hasForwardVehicleLanes = true;
                    prefab.m_forwardVehicleLaneCount = 1;
                }
                prefab.m_class.m_layer = ItemClass.Layer.Default;
                prefab.m_netLayers = 512;
                prefab.m_maxPropDistance = 1000f;
                foreach (var segment in prefab.m_segments)
                {
                    segment.m_layer = 9;
                }

                foreach (var node in prefab.m_nodes)
                {
                    node.m_layer = 9;
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
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            TransportInfo primaryTransport;
            TransportInfo secondaryTransport;
            bool canInvertPrimary;
            bool canInvertSecondary;
            if (buildingInfo?.GetAI() is CargoStationAI cargoStationAI)
            {
                primaryTransport = cargoStationAI.m_transportInfo;
                secondaryTransport = cargoStationAI.m_transportInfo2;
                canInvertPrimary = cargoStationAI.m_canInvertTarget;
                canInvertSecondary = cargoStationAI.m_canInvertTarget2;
            } 
            else if (buildingInfo?.GetAI() is DepotAI depotAI)
            {
                primaryTransport = depotAI.m_transportInfo;
                secondaryTransport = depotAI.m_secondaryTransportInfo;
                canInvertPrimary = depotAI.m_canInvertTarget;
                canInvertSecondary = depotAI.m_canInvertTarget2;
            }
            else if (buildingInfo?.GetAI() is FishingHarborAI)
            {
                primaryTransport = null;
                secondaryTransport = null;
                canInvertPrimary = true;
                canInvertSecondary = false;
            }
            else
            {
                return;
            }
            
            int fishingDockwayCounter = 0;
            foreach (var netSegment in instance.m_segments.m_buffer)
            {
                if (netSegment.m_flags == NetSegment.Flags.None || netSegment.Info == null)
                {
                    continue;
                }
                var startNode = instance.m_nodes.m_buffer[netSegment.m_startNode].m_position;
                var endNode = instance.m_nodes.m_buffer[netSegment.m_endNode].m_position;
                var position = new Vector3((startNode.x + endNode.x) / 2, (startNode.y + endNode.y) / 2, (startNode.z + endNode.z) / 2);

                var direction = (endNode - startNode).normalized;
                var target = position + direction * 0.5f;
                var invertedTarget =  position - direction * 0.5f;


                var name = netSegment.Info.name;
                
                if (name == "Fishing Dockway")
                {
                    if (fishingDockwayCounter == 0)
                    {
                        CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointPosition, position);
                        CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointTarget, target);
                        fishingDockwayCounter++;
                    }
                    else
                    {
                        CreateSpecialPoint(buildingInfo, SpecialPointType.DespawnPointPosition, position);  
                        CreateSpecialPoint(buildingInfo, SpecialPointType.DespawnPointTarget, target);
                        fishingDockwayCounter++;
                    }
                    continue;
                }
                
                
                if (name is "Bus Station Way" or "Ferry Dockway" or "Ship Dockway" or "Airplane Taxiway")
                {
                    continue;
                }

                if (!name.Contains("Station")
                    && name != "Airplane Stop" && name != "Blimp Stop" && name != "Helicopter Stop"
                    && !name.Contains("Train Cargo Track") && name != "Airplane Cargo Stop"
                    && !(buildingInfo?.GetAI() is CargoHarborAI && name.Contains("Ferry Path"))
                    && name != "Ferry Dock" && name != "Ship Dock"
                )
                {
                    continue;
                }

                var oneWay = netSegment.Info.m_hasForwardVehicleLanes != netSegment.Info.m_hasBackwardVehicleLanes;
                var airplaneTrack = name is "Airplane Stop" or "Airplane Cargo Stop";
                if (primaryTransport != null && (netSegment.Info.m_vehicleTypes & primaryTransport.m_vehicleType) != VehicleInfo.VehicleType.None)
                {
                    var minimumVerticalOffset = MinimumVerticalOffset(netSegment, primaryTransport);
                    CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointPosition, position + minimumVerticalOffset);
                    CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPointTarget, 
                        ((canInvertPrimary && !oneWay && !airplaneTrack) ? invertedTarget : target) + minimumVerticalOffset);
                }
                if (secondaryTransport != null
                    && (netSegment.Info.m_vehicleTypes & secondaryTransport.m_vehicleType) !=  VehicleInfo.VehicleType.None
                    && (primaryTransport == null || (netSegment.Info.m_vehicleTypes & primaryTransport.m_vehicleType) == VehicleInfo.VehicleType.None))
                {
                    var minimumVerticalOffset = MinimumVerticalOffset(netSegment, secondaryTransport);
                    CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPoint2Position, position + minimumVerticalOffset);
                    CreateSpecialPoint(buildingInfo, SpecialPointType.SpawnPoint2Target, 
                        ((canInvertSecondary && !oneWay && !airplaneTrack) ? invertedTarget : target) + minimumVerticalOffset);
                }
            }
        }

        private static Vector3 MinimumVerticalOffset(NetSegment netSegment, TransportInfo secondaryTransport)
        {
            return new Vector3(0, netSegment.Info.m_lanes
                .Where(l => l.m_vehicleType == secondaryTransport.m_vehicleType)
                .Select(l => l.m_verticalOffset)
                .Min(), 0);
        }


        public static void CreateSpecialPoint(BuildingInfo buildingInfo, SpecialPointType pointType, Vector3 position)
        {
            var info = SpecialPoints.GetSpecialPointProp(pointType);
            if (info == null || info.m_prefabDataIndex == -1)
            {
                return;
            }
            if (SpecialPoints.GetMaxNumberOfPoints(buildingInfo, pointType) < 1)
            {
                return;
            }
            ushort prop;
            BuildingDecorationDetour.DisableLimits = true;
            try
            {
                if (PropManager.instance.CreateProp(out prop, ref Singleton<SimulationManager>.instance.m_randomizer,
                    info,
                    position, 0, true))
                {
                    PropManager.instance.m_props.m_buffer[(int) prop].FixedHeight = true;
                }
            }
            finally
            {
                BuildingDecorationDetour.DisableLimits = false;
            }
        }
    }
}
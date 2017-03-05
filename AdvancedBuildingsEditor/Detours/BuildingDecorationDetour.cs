using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingsEditor.OptionsFramework;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace AdvancedBuildingsEditor.Detours
{

    [TargetType(typeof(BuildingDecoration))]
    public class BuildingDecorationDetour
    {
        public static bool DisableLimits = false;


        public static Dictionary<ushort, SpecialPointType> CollectSpecialPoints()
        {
            var instance = PropManager.instance;
            var specialPoints = new Dictionary<ushort, SpecialPointType>();
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var propInfo = propInstance.Info;
                if (SpecialPoints.GetSpecialPointType(propInfo) != SpecialPointType.Unknown)
                {
                    var specialPointType = SpecialPoints.GetSpecialPointType(propInfo);
                    specialPoints.Add(index1, specialPointType);
                }
            }
            return specialPoints;
        }


        [RedirectMethod]
        public static void LoadDecorations(BuildingInfo source)
        {
            UnityEngine.Debug.Log($"Loading decorations for {source.name}");

            Building data = new Building();
            data.m_position = new Vector3(0.0f, 60f, 0.0f);
            data.Width = source.m_cellWidth;
            data.Length = source.m_cellLength;
            BuildingDecoration.LoadPaths(source, (ushort)0, ref data, 0.0f);
            BuildingDecoration.LoadProps(source, (ushort)0, ref data);
            //begin mod
            LoadSpecialPoints((source), (ushort)0, ref data);
            //end mod
        }

        public static void LoadSpecialPoints(BuildingInfo info, ushort buildingID, ref Building data)
        {
            Scripts.ClearProps(true);
            var pos = data.m_position;
            var q = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            var matrix4x4 = new Matrix4x4();
            matrix4x4.SetTRS(pos, q, Vector3.one);

            var ai = info.m_buildingAI as DepotAI;
            if (ai != null)
            {
                var mSpawnPoints = ai.m_spawnPoints;
                if (mSpawnPoints == null || mSpawnPoints.Length == 0)
                {
                    mSpawnPoints = new[]
                    {
                        new DepotAI.SpawnPoint
                        {
                            m_position = ai.m_spawnPosition,
                            m_target = ai.m_spawnTarget
                        }
                    };
                }
                for (var index = 0; index < mSpawnPoints.Length; ++index)
                {
                    if (ai.m_canInvertTarget)
                    {
                        PlaceSpecialPoint(info, matrix4x4, mSpawnPoints[index].m_position, SpecialPointType.SpawnPointPosition);
                    }
                    PlaceSpecialPoint(info, matrix4x4, mSpawnPoints[index].m_target, SpecialPointType.SpawnPointTarget);
                }
            }
            var ai2 = info.m_buildingAI as CargoStationAI;
            if (ai2 != null)
            {
                PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnPosition, SpecialPointType.SpawnPointPosition);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnTarget, SpecialPointType.SpawnPointTarget);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnPosition2, SpecialPointType.SpawnPoint2Position);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnTarget2, SpecialPointType.SpawnPoint2Target);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_truckSpawnPosition, SpecialPointType.TruckSpawnPosition);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_truckUnspawnPosition, SpecialPointType.TruckDespawnPosition);
            }
        }

        private static void PlaceSpecialPoint(BuildingInfo info, Matrix4x4 matrix4x4, Vector3 pointLocation, SpecialPointType pointType)
        {
            var vector3 = matrix4x4.MultiplyPoint(pointLocation).MirrorZ();
            DisableLimits = true;
            Scripts.CreateSpecialPoint(info, pointType, vector3);
            DisableLimits = false;
        }

        [RedirectMethod]
        public static void SaveProps(BuildingInfo info, ushort buildingID, ref Building data)
        {
            FastList<BuildingInfo.Prop> fastList = new FastList<BuildingInfo.Prop>();
            Vector3 pos = data.m_position;
            Quaternion q = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            Matrix4x4 matrix4x4 = new Matrix4x4();
            matrix4x4.SetTRS(pos, q, Vector3.one);
            matrix4x4 = matrix4x4.inverse;
            //begin mod
            Quaternion q_1 = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            Matrix4x4 matrix4x4_1 = new Matrix4x4();
            matrix4x4_1.SetTRS(pos, q_1, Vector3.one);
            matrix4x4_1 = matrix4x4_1.inverse;
            PropManager instance1 = Singleton<PropManager>.instance;
            var specialPoints = CollectSpecialPoints();
            var depotAI = info.m_buildingAI as DepotAI;
            var cargoStationAI = info.m_buildingAI as CargoStationAI;
            List<DepotAI.SpawnPoint> spawnPoints = new List<DepotAI.SpawnPoint>();

            Vector3 m_truckSpawnPosition = Vector3.zero;
            Vector3 m_truckUnspawnPosition = Vector3.zero;


            for (ushort index = 0; index < ushort.MaxValue; ++index)
            {
                if (((int)instance1.m_props.m_buffer[index].m_flags & 67) == 1)
                {
                    if (specialPoints.ContainsKey(index))
                    {
                        if (depotAI != null || cargoStationAI != null)
                        {
                            var position = instance1.m_props.m_buffer[index].Position;
                            var globalPosition = matrix4x4_1.MultiplyPoint(position);
                            switch (specialPoints[index])
                            {
                                case SpecialPointType.SpawnPointTarget:
                                    {
                                        if (depotAI != null || spawnPoints.Count < 2)
                                        {
                                            var calculatedPositionGlobalPosition = (depotAI != null && depotAI.m_canInvertTarget || cargoStationAI != null && cargoStationAI.m_canInvertTarget) ? FindClosestPositionPoint(specialPoints, SpecialPointType.SpawnPointPosition, instance1, position, globalPosition, matrix4x4_1) : globalPosition;
                                            if (cargoStationAI == null)
                                            {
                                                spawnPoints.Add(new DepotAI.SpawnPoint()
                                                {
                                                    m_position = calculatedPositionGlobalPosition.MirrorZ(),
                                                    m_target = globalPosition.MirrorZ(),
                                                });
                                            }
                                            else
                                            {
                                                spawnPoints.Insert(0, new DepotAI.SpawnPoint()
                                                {
                                                    m_position = calculatedPositionGlobalPosition.MirrorZ(),
                                                    m_target = globalPosition.MirrorZ(),
                                                });
                                            }
                                        }
                                        break;
                                    }
                                case SpecialPointType.SpawnPoint2Target:
                                    {
                                        if (cargoStationAI != null && spawnPoints.Count < 2)
                                        {
                                            var calculatedPositionGlobalPosition = cargoStationAI.m_canInvertTarget ? FindClosestPositionPoint(specialPoints, SpecialPointType.SpawnPoint2Position, instance1, position, globalPosition, matrix4x4_1) : globalPosition;
                                            spawnPoints.Add(new DepotAI.SpawnPoint()
                                            {
                                                m_position = calculatedPositionGlobalPosition.MirrorZ(),
                                                m_target = globalPosition.MirrorZ(),
                                            });
                                        }
                                        break;
                                    }
                                case SpecialPointType.TruckSpawnPosition:
                                    {
                                        if (cargoStationAI != null)
                                        {
                                            m_truckSpawnPosition = globalPosition.MirrorZ();
                                        }
                                        break;
                                    }
                                case SpecialPointType.TruckDespawnPosition:
                                    {
                                        if (cargoStationAI != null)
                                        {
                                            m_truckUnspawnPosition = globalPosition.MirrorZ();
                                        }
                                        break;
                                    }
                                default:
                                    continue; //ignored
                            }
                        }
                        continue;
                    }
                    //end mod
                    BuildingInfo.Prop prop = new BuildingInfo.Prop();
                    prop.m_prop = instance1.m_props.m_buffer[index].Info;
                    prop.m_finalProp = prop.m_prop;
                    prop.m_position = matrix4x4.MultiplyPoint(instance1.m_props.m_buffer[index].Position);
                    prop.m_radAngle = instance1.m_props.m_buffer[index].Angle - data.m_angle;
                    prop.m_angle = 57.29578f * prop.m_radAngle;
                    prop.m_fixedHeight = instance1.m_props.m_buffer[index].FixedHeight;
                    //begin mod
                    var flag = false;
                    foreach (var mProp in info.m_props)
                    {
                        if (mProp.m_prop != prop.m_prop || Vector3.Distance(mProp.m_position, prop.m_position) > 0.01)
                        {
                            continue;
                        }
                        prop.m_probability = mProp.m_probability;
                        flag = true;
                        Debug.Log($"Setting probability of {prop.m_prop.name} to {prop.m_probability}");
                        break;
                    }
                    if (!flag)
                    {
                        prop.m_probability = 100;
                    }
                    //end mod
                    fastList.Add(prop);
                }
            }
            //begin mod
            if (depotAI != null)
            {
                if (OptionsWrapper<Options>.Options.PreciseSpecialPointsPostions)
                {
                    var ai = (DepotAI)((BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo).m_buildingAI;
                    depotAI.m_spawnPoints = ai.m_spawnPoints;
                    depotAI.m_spawnPosition = ai.m_spawnPosition;
                    depotAI.m_spawnTarget = ai.m_spawnTarget;
                }
                else
                {
                    if (spawnPoints.Count == 1)
                    {
                        depotAI.m_spawnPosition = spawnPoints[0].m_position;
                        depotAI.m_spawnTarget = spawnPoints[0].m_target;
                        depotAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    }
                    else if (spawnPoints.Count > 1)
                    {
                        depotAI.m_spawnPosition = Vector3.zero;
                        depotAI.m_spawnTarget = Vector3.zero;
                        depotAI.m_spawnPoints = spawnPoints.ToArray();
                    }
                    else
                    {
                        depotAI.m_spawnPosition = Vector3.zero;
                        depotAI.m_spawnTarget = Vector3.zero;
                        depotAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    }
                }
            }
            if (cargoStationAI != null)
            {
                if (OptionsWrapper<Options>.Options.PreciseSpecialPointsPostions)
                {
                    var ai = (CargoStationAI)((BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo).m_buildingAI;
                    cargoStationAI.m_spawnPosition = ai.m_spawnPosition;
                    cargoStationAI.m_spawnTarget = ai.m_spawnTarget;
                    cargoStationAI.m_spawnPosition2 = ai.m_spawnPosition2;
                    cargoStationAI.m_spawnTarget2 = ai.m_spawnTarget2;
                    cargoStationAI.m_truckSpawnPosition = ai.m_truckSpawnPosition;
                    cargoStationAI.m_truckUnspawnPosition = ai.m_truckUnspawnPosition;
                }
                else
                {
                    if (spawnPoints.Count == 1)
                    {
                        cargoStationAI.m_spawnPosition = spawnPoints[0].m_position;
                        cargoStationAI.m_spawnTarget = spawnPoints[0].m_target;
                        cargoStationAI.m_spawnPosition2 = Vector3.zero;
                        cargoStationAI.m_spawnTarget2 = Vector3.zero;
                    }
                    else if (spawnPoints.Count > 1)
                    {
                        cargoStationAI.m_spawnPosition = spawnPoints[0].m_position;
                        cargoStationAI.m_spawnTarget = spawnPoints[0].m_target;
                        cargoStationAI.m_spawnPosition2 = spawnPoints[1].m_position;
                        cargoStationAI.m_spawnTarget2 = spawnPoints[1].m_target;
                    }
                    else
                    {
                        cargoStationAI.m_spawnPosition = Vector3.zero;
                        cargoStationAI.m_spawnTarget = Vector3.zero;
                        cargoStationAI.m_spawnPosition2 = Vector3.zero;
                        cargoStationAI.m_spawnTarget2 = Vector3.zero;
                    }
                    cargoStationAI.m_truckSpawnPosition = m_truckSpawnPosition;
                    cargoStationAI.m_truckUnspawnPosition = m_truckUnspawnPosition;
                }
            }
            //end mod
            TreeManager instance2 = Singleton<TreeManager>.instance;
            for (int index = 0; index < instance2.m_trees.m_buffer.Length; ++index)
            {
                if (((int)instance2.m_trees.m_buffer[index].m_flags & 3) == 1 && instance2.m_trees.m_buffer[index].GrowState != 0)
                {
                    BuildingInfo.Prop prop = new BuildingInfo.Prop();
                    prop.m_tree = instance2.m_trees.m_buffer[index].Info;
                    prop.m_finalTree = prop.m_tree;
                    prop.m_position = matrix4x4.MultiplyPoint(instance2.m_trees.m_buffer[index].Position);
                    prop.m_fixedHeight = instance2.m_trees.m_buffer[index].FixedHeight;
                    //begin mod
                    var flag = false;
                    foreach (var mProp in info.m_props)
                    {
                        if (mProp.m_tree != prop.m_tree || Vector3.Distance(mProp.m_position, prop.m_position) > 0.01)
                        {
                            continue;
                        }
                        prop.m_probability = mProp.m_probability;
                        flag = true;
                        Debug.Log($"Setting probability of {prop.m_tree.name} to {prop.m_probability}");
                        break;
                    }
                    if (!flag)
                    {
                        prop.m_probability = 100;
                    }
                    //end mod
                    fastList.Add(prop);
                }
            }
            info.m_props = fastList.ToArray();
        }

        private static Vector3 FindClosestPositionPoint(Dictionary<ushort, SpecialPointType> specialPoints, SpecialPointType pointType, PropManager instance1,
            Vector3 position, Vector3 globalPosition, Matrix4x4 matrix4x4_1)
        {
            Vector3 calculatedPositionGlobalPosition;
            var minDistance = float.MaxValue;
            ushort positionIndex = 0;
            foreach (var pi in specialPoints.Where(p => p.Value == pointType).Select(p => p.Key))
            {
                var positionPosition = instance1.m_props.m_buffer[pi].Position;
                var distance = Mathf.Abs(Vector3.Distance(position, positionPosition));
                if (distance > minDistance)
                {
                    continue;
                }
                minDistance = distance;
                positionIndex = pi;
            }
            if (positionIndex == 0)
            {
                Debug.LogWarning("Couldn't find closest position position for target!");
                calculatedPositionGlobalPosition = globalPosition;
            }
            else
            {
                calculatedPositionGlobalPosition = matrix4x4_1.MultiplyPoint(instance1.m_props.m_buffer[positionIndex].Position);
            }
            return calculatedPositionGlobalPosition;
        }

        [RedirectMethod]
        public static void SavePaths(BuildingInfo info, ushort buildingID, ref Building data)
        {
            FastList<BuildingInfo.PathInfo> fastList = new FastList<BuildingInfo.PathInfo>();
            //begin mod
            if (OptionsWrapper<Options>.Options.PrecisePathsPostions)
            {
                var building = (BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo;
                if (building.m_paths != null)
                {
                    foreach (var pathInfo in building.m_paths.Where(p => p != null))
                    {
                        fastList.Add(pathInfo);
                    }
                }
                info.m_paths = fastList.ToArray();
                return;
            }
            //end mod


            List<ushort> ushortList1 = new List<ushort>();
            List<ushort> ushortList2 = new List<ushort>();
            for (ushort index = 1; (int)index < 49152; ++index)
            {
                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)index].m_flags != Building.Flags.None)
                {
                    ushortList1.AddRange((IEnumerable<ushort>)BuildingDecoration.GetBuildingSegments(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)index]));
                    ushortList2.Add(index);
                }
            }
            NetManager instance = Singleton<NetManager>.instance;
            for (int index = 0; index < 36864; ++index)
            {
                if ((instance.m_segments.m_buffer[index].m_flags & (NetSegment.Flags.Created | NetSegment.Flags.Deleted)) == NetSegment.Flags.Created && !ushortList1.Contains((ushort)index))
                {
                    NetInfo info1 = instance.m_segments.m_buffer[index].Info;
                    ushort startNode = instance.m_segments.m_buffer[index].m_startNode;
                    ushort endNode = instance.m_segments.m_buffer[index].m_endNode;
                    Vector3 position1 = instance.m_nodes.m_buffer[(int)startNode].m_position;
                    Vector3 position2 = instance.m_nodes.m_buffer[(int)endNode].m_position;
                    Vector3 startDirection = instance.m_segments.m_buffer[index].m_startDirection;
                    Vector3 endDirection = instance.m_segments.m_buffer[index].m_endDirection;
                    Vector3 vector3;
                    if (NetSegment.IsStraight(position1, startDirection, position2, endDirection))
                    {
                        vector3 = (position1 + position2) * 0.5f;
                    }
                    else
                    {
                        float u;
                        float v;
                        if (Line2.Intersect(VectorUtils.XZ(position1), VectorUtils.XZ(position1 + startDirection), VectorUtils.XZ(position2), VectorUtils.XZ(position2 + endDirection), out u, out v))
                        {
                            float minNodeDistance = info1.GetMinNodeDistance();
                            u = Mathf.Max(minNodeDistance, u);
                            v = Mathf.Max(minNodeDistance, v);
                            vector3 = (position1 + startDirection * u + position2 + endDirection * v) * 0.5f;
                        }
                        else
                            vector3 = (position1 + position2) * 0.5f;
                    }
                    BuildingInfo.PathInfo pathInfo = new BuildingInfo.PathInfo();
                    pathInfo.m_netInfo = info1;
                    pathInfo.m_nodes = new Vector3[2];
                    pathInfo.m_nodes[0] = position1 - data.m_position;
                    pathInfo.m_nodes[0].z = -pathInfo.m_nodes[0].z;
                    pathInfo.m_nodes[1] = position2 - data.m_position;
                    pathInfo.m_nodes[1].z = -pathInfo.m_nodes[1].z;
                    pathInfo.m_curveTargets = new Vector3[1];
                    pathInfo.m_curveTargets[0] = vector3 - data.m_position;
                    pathInfo.m_curveTargets[0].z = -pathInfo.m_curveTargets[0].z;
                    pathInfo.m_forbidLaneConnection = new bool[2];
                    pathInfo.m_forbidLaneConnection[0] = (instance.m_nodes.m_buffer[(int)startNode].m_flags & NetNode.Flags.ForbidLaneConnection) != NetNode.Flags.None;
                    pathInfo.m_forbidLaneConnection[1] = (instance.m_nodes.m_buffer[(int)endNode].m_flags & NetNode.Flags.ForbidLaneConnection) != NetNode.Flags.None;
                    pathInfo.m_maxSnapDistance = info.m_placementMode != BuildingInfo.PlacementMode.Roadside || (double)position1.z <= (double)info.m_cellLength * 4.0 + 8.0 && (double)position2.z <= (double)info.m_cellLength * 4.0 + 8.0 ? 0.1f : 7.5f;
                    pathInfo.m_invertSegments = (instance.m_segments.m_buffer[index].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;
                    fastList.Add(pathInfo);
                }
            }
            for (int index = 0; index < 32768; ++index)
            {
                if ((instance.m_nodes.m_buffer[index].m_flags & (NetNode.Flags.Created | NetNode.Flags.Deleted)) == NetNode.Flags.Created && instance.m_nodes.m_buffer[index].CountSegments() == 0)
                {
                    bool flag = false;
                    using (List<ushort>.Enumerator enumerator = ushortList2.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)enumerator.Current].ContainsNode((ushort)index))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        NetInfo info1 = instance.m_nodes.m_buffer[index].Info;
                        Vector3 position = instance.m_nodes.m_buffer[index].m_position;
                        BuildingInfo.PathInfo pathInfo = new BuildingInfo.PathInfo();
                        pathInfo.m_netInfo = info1;
                        pathInfo.m_nodes = new Vector3[1];
                        pathInfo.m_nodes[0] = position - data.m_position;
                        pathInfo.m_nodes[0].z = -pathInfo.m_nodes[0].z;
                        pathInfo.m_curveTargets = new Vector3[0];
                        pathInfo.m_forbidLaneConnection = new bool[1];
                        pathInfo.m_forbidLaneConnection[0] = (instance.m_nodes.m_buffer[index].m_flags & NetNode.Flags.ForbidLaneConnection) != NetNode.Flags.None;
                        pathInfo.m_maxSnapDistance = 0.1f;
                        pathInfo.m_invertSegments = false;
                        fastList.Add(pathInfo);
                    }
                }
            }
            info.m_paths = fastList.ToArray();
        }
    }
}
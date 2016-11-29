using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingsEditor.OptionsFramework;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
using UnityEngine;

namespace AdvancedBuildingsEditor.Detours
{
    //TODO(earalov): add support for CargoStationAI
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
            var cargoStationAI = info.m_buildingAI as CargoStationAI; //TODO(earalov): save cargo station's special points
            FastList<DepotAI.SpawnPoint> depotSpawnPoints = new FastList<DepotAI.SpawnPoint>();

            for (ushort index = 0; index < ushort.MaxValue; ++index)
            {
                if (((int)instance1.m_props.m_buffer[index].m_flags & 67) == 1)
                {
                    if (depotAI != null)
                    {
                        if (specialPoints.ContainsKey(index))
                        {
                            var targetPosition = instance1.m_props.m_buffer[index].Position;
                            var targetGlobalPosition = matrix4x4_1.MultiplyPoint(targetPosition);

                            switch (specialPoints[index])
                            {
                                case SpecialPointType.SpawnPointTarget:
                                    Vector3 calculatedPositionGlobalPosition;
                                    if (depotAI.m_canInvertTarget)
                                    {
                                        var minDistance = float.MaxValue;
                                        ushort positionIndex = 0;
                                        foreach (var pi in specialPoints.Where(p => p.Value == SpecialPointType.SpawnPointPosition).Select(p => p.Key))
                                        {
                                            var positionPosition = instance1.m_props.m_buffer[pi].Position;
                                            var distance = Mathf.Abs(Vector3.Distance(targetPosition, positionPosition));
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
                                            calculatedPositionGlobalPosition = targetGlobalPosition;
                                        }
                                        else
                                        {
                                            calculatedPositionGlobalPosition = matrix4x4_1.MultiplyPoint(instance1.m_props.m_buffer[positionIndex].Position);
                                        }
                                    }
                                    else
                                    {
                                        calculatedPositionGlobalPosition = targetGlobalPosition;
                                    }
                                    depotSpawnPoints.Add(new DepotAI.SpawnPoint()
                                    {
                                        m_position = calculatedPositionGlobalPosition.MirrorZ(),
                                        m_target = targetGlobalPosition.MirrorZ(),
                                    });
                                    break;
                                case SpecialPointType.SpawnPointPosition:
                                    break;
                                default:
                                    continue; //ignored
                            }
                            continue;
                        }
                    }
                    else if (cargoStationAI != null)
                    {
                        //TODO(earalov): save special points for cargo station
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
                    if (depotSpawnPoints.m_size == 1)
                    {
                        depotAI.m_spawnPosition = depotSpawnPoints[0].m_position;
                        depotAI.m_spawnTarget = depotSpawnPoints[0].m_target;
                        depotAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    }
                    else if (depotSpawnPoints.m_size > 1)
                    {
                        depotAI.m_spawnPosition = Vector3.zero;
                        depotAI.m_spawnTarget = Vector3.zero;
                        depotAI.m_spawnPoints = depotSpawnPoints.ToArray();
                    }
                    else
                    {
                        depotAI.m_spawnPosition = Vector3.zero;
                        depotAI.m_spawnTarget = Vector3.zero;
                        depotAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    }
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

    }
}
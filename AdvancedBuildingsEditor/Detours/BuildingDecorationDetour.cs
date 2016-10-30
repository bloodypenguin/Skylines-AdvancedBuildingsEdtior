using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static Dictionary<ushort, SpecialPointType> SpecialPoints = new Dictionary<ushort, SpecialPointType>();

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
            LoadSubBuildings((source), (ushort)0, ref data);
            LoadSpecialPoints((source), (ushort)0, ref data);
            //end mod
        }

        private static void LoadSubBuildings(BuildingInfo info, ushort buildingID, ref Building data)
        {
            if (info.m_subBuildings == null || info.m_subBuildings.Length == 0)
            {
                return;
            }
            UnityEngine.Debug.Log($"Loading sub buildings for {info.name}");
            Vector3 pos = data.m_position;
            Quaternion q = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            Matrix4x4 matrix4x4 = new Matrix4x4();
            matrix4x4.SetTRS(pos, q, Vector3.one);
            var instance1 = Singleton<BuildingManager>.instance;
            for (var index = 0; index < info.m_subBuildings.Length; index = index + 1)
            {
                BuildingInfo info1 = info.m_subBuildings[index].m_buildingInfo;
                if (info1 == null)
                {
                    continue;
                }
                var propsCopy = info1.m_props;
                info1.m_props = null;
                var pathsCopy = info1.m_paths;
                info1.m_paths = null;
                var subBuildingsCopy = info1.m_subBuildings;
                info1.m_subBuildings = null;
                Vector3 vector3 = matrix4x4.MultiplyPoint(info.m_subBuildings[index].m_position);
                float angle = data.m_angle + (float)(Math.PI / 180.0) * info.m_subBuildings[index].m_angle;

                instance1.CreateBuilding(out buildingID, ref Singleton<SimulationManager>.instance.m_randomizer, info1, vector3, angle, 0, buildingID);

                info1.m_props = propsCopy;
                info1.m_paths = pathsCopy;
                info1.m_subBuildings = subBuildingsCopy;
            }
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
                        PlaceSpecialPoint(matrix4x4, mSpawnPoints[index].m_position, AdvancedBuildingsEditor.SpecialPoints.SpawnPointPosition);
                    }
                    PlaceSpecialPoint(matrix4x4, mSpawnPoints[index].m_target, AdvancedBuildingsEditor.SpecialPoints.SpawnPointTarget);
                }
            }
            var ai2 = info.m_buildingAI as CargoStationAI;
            if (ai2 != null)
            {
                PlaceSpecialPoint(matrix4x4, ai2.m_spawnPosition, AdvancedBuildingsEditor.SpecialPoints.SpawnPointPosition);
                PlaceSpecialPoint(matrix4x4, ai2.m_spawnTarget, AdvancedBuildingsEditor.SpecialPoints.SpawnPointTarget);
                PlaceSpecialPoint(matrix4x4, ai2.m_spawnPosition2, AdvancedBuildingsEditor.SpecialPoints.SpawnPoint2Position);
                PlaceSpecialPoint(matrix4x4, ai2.m_spawnTarget2, AdvancedBuildingsEditor.SpecialPoints.SpawnPoint2Target);
                PlaceSpecialPoint(matrix4x4, ai2.m_truckSpawnPosition, AdvancedBuildingsEditor.SpecialPoints.TruckSpawnPosition);
                PlaceSpecialPoint(matrix4x4, ai2.m_truckUnspawnPosition, AdvancedBuildingsEditor.SpecialPoints.TruckDespawnPosition);
            }
            Scripts.RecalculateSpecialPoints();
        }

        private static void PlaceSpecialPoint(Matrix4x4 matrix4x4, Vector3 pointLocation, string propName)
        {
            var info2 = PrefabCollection<PropInfo>.FindLoaded(propName);
            if (info2 == null || info2.m_prefabDataIndex == -1)
            {
                return;
            }
            Vector3 vector3 = matrix4x4.MultiplyPoint(pointLocation).MirrorZ();
            DisableLimits = true;
            Scripts.CreateSpecialPoint(info2, vector3);
            DisableLimits = false;
        }

        [RedirectMethod]
        public static void SaveDecorations(BuildingInfo target)
        {
            try
            {
                Building data = new Building();
                data.m_position = new Vector3(0.0f, 60f, 0.0f);
                data.Width = target.m_cellWidth;
                data.Length = target.m_cellLength;
                BuildingDecoration.SavePaths(target, (ushort)0, ref data);
                SaveProps(target, (ushort)0, ref data);
                //begin mod
                SaveSubBuildings(target, ref data);
                //end mod
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
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
            Scripts.RecalculateSpecialPoints();
            var depotAI = info.m_buildingAI as DepotAI;
            var cargoStationAI = info.m_buildingAI as CargoStationAI; //TODO(earalov): save cargo station's special points
            FastList<DepotAI.SpawnPoint> depotSpawnPoints = new FastList<DepotAI.SpawnPoint>();

            for (ushort index = 0; index < ushort.MaxValue; ++index)
            {
                if (((int)instance1.m_props.m_buffer[index].m_flags & 67) == 1)
                {
                    if (depotAI != null)
                    {
                        if (SpecialPoints.ContainsKey(index))
                        {
                            var targetPosition = instance1.m_props.m_buffer[index].Position;
                            var targetGlobalPosition = matrix4x4_1.MultiplyPoint(targetPosition);

                            switch (SpecialPoints[index])
                            {
                                case SpecialPointType.SpawnPointTarget:
                                    Vector3 calculatedPositionGlobalPosition;
                                    if (depotAI.m_canInvertTarget)
                                    {
                                        var minDistance = float.MaxValue;
                                        ushort positionIndex = 0;
                                        foreach (var pi in SpecialPoints.Where(p => p.Value == SpecialPointType.SpawnPointPosition).Select(p => p.Key))
                                        {
                                            var positionPosition = instance1.m_props.m_buffer[pi].Position;
                                            var distance = Mathf.Abs(Vector3.Distance(targetPosition, positionPosition));
                                            if (minDistance < distance)
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
                    var ai = ((DepotAI) ((BuildingInfo) ToolsModifierControl.toolController.m_editPrefabInfo).m_buildingAI);
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



        public static void SaveSubBuildings(BuildingInfo info, ref Building data)
        {
            info.m_subBuildings = new BuildingInfo.SubInfo[] { };
            var fastList = CollectSubInfos(info, data);
            foreach (var building in fastList)
            {
                if (building.m_buildingInfo == null)
                {
                    UnityEngine.Debug.LogError("Advanced Buildings Editor: a sub building's info is null! Sub buildings won't be saved");
                    return;
                }
            }
            if (fastList.m_size < 1)
            {
                UnityEngine.Debug.Log("Advanced Buildings Editor: no sub buildings detected");
                return;
            }
            if (OptionsWrapper<Options>.Options.SubBuildingsFormat == (int)SubBuildingsFormats.Native)
            {
                info.m_subBuildings = fastList.ToArray();
            }
            else
            {
                var config = new SubBuildingsDefinition();
                config.Buildings.Add(new SubBuildingsDefinition.Building()
                {
                    Name = info.name
                });
                foreach (var building in fastList.ToArray())
                {
                    config.Buildings[0].SubBuildings.Add(new SubBuildingsDefinition.SubBuilding()
                    {
                        Angle = building.m_angle,
                        Name = building.m_buildingInfo.name,
                        FixedHeight = building.m_fixedHeight,
                        PosX = building.m_position.x,
                        PosY = building.m_position.y,
                        PosZ = building.m_position.z,
                    });
                }
                SubBuildingsDefinition.Save(config);
            }
        }

        private static FastList<BuildingInfo.SubInfo> CollectSubInfos(BuildingInfo info, Building data)
        {
            FastList<BuildingInfo.SubInfo> fastList = new FastList<BuildingInfo.SubInfo>();
            Vector3 pos = data.m_position;
            Quaternion q = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            Matrix4x4 matrix4x4 = new Matrix4x4();
            matrix4x4.SetTRS(pos, q, Vector3.one);
            matrix4x4 = matrix4x4.inverse;
            BuildingManager instance1 = Singleton<BuildingManager>.instance;
            for (int index = 0; index < instance1.m_buildings.m_buffer.Length; ++index)
            {
                var building = instance1.m_buildings.m_buffer[index];
                if (((int)building.m_flags & 67) == 1)
                {
                    BuildingInfo.SubInfo subInfo = new BuildingInfo.SubInfo();
                    subInfo.m_buildingInfo = building.Info;
                    subInfo.m_position = matrix4x4.MultiplyPoint(building.m_position);
                    subInfo.m_angle = 57.29578f * building.m_angle;
                    subInfo.m_fixedHeight = info.m_fixedHeight;
                    fastList.Add(subInfo);
                }
            }
            return fastList;
        }

        [RedirectMethod]
        public static void ClearDecorations()
        {
            NetManager instance1 = Singleton<NetManager>.instance;
            for (int index = 1; index < instance1.m_segments.m_buffer.Length; ++index)
            {
                if (instance1.m_segments.m_buffer[index].m_flags != NetSegment.Flags.None)
                    instance1.ReleaseSegment((ushort)index, true);
            }
            for (int index = 1; index < instance1.m_nodes.m_buffer.Length; ++index)
            {
                if (instance1.m_nodes.m_buffer[index].m_flags != NetNode.Flags.None)
                    instance1.ReleaseNode((ushort)index);
            }
            PropManager instance2 = Singleton<PropManager>.instance;
            for (int index = 1; index < instance2.m_props.m_buffer.Length; ++index)
            {
                if ((int)instance2.m_props.m_buffer[index].m_flags != 0)
                    instance2.ReleaseProp((ushort)index);
            }
            TreeManager instance3 = Singleton<TreeManager>.instance;
            for (int index = 1; index < instance3.m_trees.m_buffer.Length; ++index)
            {
                if ((int)instance3.m_trees.m_buffer[index].m_flags != 0)
                    instance3.ReleaseTree((uint)index);
            }
            //begin mod
            BuildingManager instance4 = Singleton<BuildingManager>.instance;
            for (int index = 1; index < instance4.m_buildings.m_buffer.Length; ++index)
            {
                if ((int)instance4.m_buildings.m_buffer[index].m_flags != 0)
                    instance4.ReleaseBuilding((ushort)index);
            }
            //end mod
        }
    }
}
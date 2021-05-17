using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingsEditor.OptionsFramework;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
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

        [RedirectMethod]
        public static void SaveDecorations(BuildingInfo target)
        {
            Building data = new Building();
            data.m_position = new Vector3(0.0f, 60f, 0.0f);
            data.Width = target.m_cellWidth;
            data.Length = target.m_cellLength;
            //begin mod
            if (OptionsWrapper<Options>.Options.PrecisePathsPostions)
            {
                SavePrecisePaths(target);
            }
            else
            {
                BuildingDecoration.SavePaths(target, (ushort)0, ref data);
            }
            //end mod
            BuildingDecoration.SaveProps(target, (ushort)0, ref data);
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
            List<DepotAI.SpawnPoint> spawnPoints2 = new List<DepotAI.SpawnPoint>();

            Vector3 truckSpawnPosition = Vector3.zero;
            Vector3 truckUnspawnPosition = Vector3.zero;


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
                                        //TODO: add handling of passenger hub
                                        if (cargoStationAI != null && spawnPoints2.Count < 2)
                                        {
                                            var calculatedPositionGlobalPosition = cargoStationAI.m_canInvertTarget2 ? FindClosestPositionPoint(specialPoints, SpecialPointType.SpawnPoint2Position, instance1, position, globalPosition, matrix4x4_1) : globalPosition;
                                            spawnPoints2.Add(new DepotAI.SpawnPoint()
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
                                            truckSpawnPosition = globalPosition.MirrorZ();
                                        }
                                        break;
                                    }
                                case SpecialPointType.TruckDespawnPosition:
                                    {
                                        if (cargoStationAI != null)
                                        {
                                            truckUnspawnPosition = globalPosition.MirrorZ();
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
                    foreach (var mProp in info.m_props ?? new BuildingInfo.Prop[]{})
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
                    depotAI.m_spawnPoints2 = ai.m_spawnPoints2;
                    depotAI.m_spawnPosition2 = ai.m_spawnPosition2;
                    depotAI.m_spawnTarget2 = ai.m_spawnTarget2;
                }
                else
                {
                    if (depotAI.m_transportInfo != null)
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
                    else
                    {
                        depotAI.m_spawnPosition = Vector3.zero;
                        depotAI.m_spawnTarget = Vector3.zero;
                        depotAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    }

                    if (depotAI.m_secondaryTransportInfo != null)
                    {
                        if (spawnPoints2.Count == 1)
                        {
                            depotAI.m_spawnPosition2 = spawnPoints2[0].m_position;
                            depotAI.m_spawnTarget2 = spawnPoints2[0].m_target;
                            depotAI.m_spawnPoints2 = new DepotAI.SpawnPoint[] { };
                        }
                        else if (spawnPoints2.Count > 1)
                        {
                            depotAI.m_spawnPosition2 = Vector3.zero;
                            depotAI.m_spawnTarget2 = Vector3.zero;
                            depotAI.m_spawnPoints2 = spawnPoints2.ToArray();
                        }
                        else
                        {
                            depotAI.m_spawnPosition2 = Vector3.zero;
                            depotAI.m_spawnTarget2 = Vector3.zero;
                            depotAI.m_spawnPoints2 = new DepotAI.SpawnPoint[] { };
                        }                        
                    }
                    else
                    {
                        depotAI.m_spawnPosition2 = Vector3.zero;
                        depotAI.m_spawnTarget2 = Vector3.zero;
                        depotAI.m_spawnPoints2 = new DepotAI.SpawnPoint[] { };   
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
                    if (cargoStationAI.m_transportInfo != null)
                    {
                        if (spawnPoints.Count == 1)
                        {
                            cargoStationAI.m_spawnPosition = spawnPoints[0].m_position;
                            cargoStationAI.m_spawnTarget = spawnPoints[0].m_target;
                        }
                        else if (spawnPoints.Count == 0)
                        {
                            cargoStationAI.m_spawnPosition = Vector3.zero;
                            cargoStationAI.m_spawnTarget = Vector3.zero; 
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Too many spawn points 1!"); //TODO: don't allow to place
                        }     
                    }
                    else
                    {
                        cargoStationAI.m_spawnPosition = Vector3.zero;
                        cargoStationAI.m_spawnTarget = Vector3.zero;     
                    }

                    if (cargoStationAI.m_transportInfo2 != null)
                    {
                        if (spawnPoints2.Count == 1)
                        {
                            cargoStationAI.m_spawnPosition2 = spawnPoints2[0].m_position;
                            cargoStationAI.m_spawnTarget2 = spawnPoints2[0].m_target;
                        }
                        else if (spawnPoints2.Count == 0)
                        {
                            cargoStationAI.m_spawnPosition2 = Vector3.zero;
                            cargoStationAI.m_spawnTarget2 = Vector3.zero;
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Too many spawn points 2!"); //TODO: don't allow to place
                        } 
                    }
                    else
                    {
                        cargoStationAI.m_spawnPosition2 = Vector3.zero;
                        cargoStationAI.m_spawnTarget2 = Vector3.zero;  
                    }
                    cargoStationAI.m_truckSpawnPosition = truckSpawnPosition;
                    cargoStationAI.m_truckUnspawnPosition = truckUnspawnPosition;
                }
            }
            //TODO: add handling of fishing harbor
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
                    foreach (var mProp in info.m_props ?? new BuildingInfo.Prop[]{})
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

        private static void SavePrecisePaths(BuildingInfo info)
        {
            FastList<BuildingInfo.PathInfo> fastList = new FastList<BuildingInfo.PathInfo>();
            var building = (BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo;
            if (building.m_paths != null)
            {
                foreach (var pathInfo in building.m_paths.Where(p => p != null))
                {
                    fastList.Add(pathInfo);
                }
            }
            info.m_paths = fastList.ToArray();
        }

        public static void LoadSpecialPoints(BuildingInfo info, ushort buildingID, ref Building data)
        {
            Scripts.ClearProps(true);
            var pos = data.m_position;
            var q = Quaternion.AngleAxis(data.m_angle * 57.29578f, Vector3.down);
            var matrix4x4 = new Matrix4x4();
            matrix4x4.SetTRS(pos, q, Vector3.one);

            if (info.m_buildingAI is DepotAI ai)
            {
                if (ai.m_transportInfo != null)
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
                        PlaceSpecialPoint(info, matrix4x4, mSpawnPoints[index].m_position, SpecialPointType.SpawnPointPosition);
                        PlaceSpecialPoint(info, matrix4x4, mSpawnPoints[index].m_target, SpecialPointType.SpawnPointTarget);
                    }
                }
                if (ai.m_secondaryTransportInfo != null)
                {
                    var mSpawnPoints2 = ai.m_spawnPoints2;
                    if (mSpawnPoints2 == null || mSpawnPoints2.Length == 0)
                    {
                        mSpawnPoints2 = new[]
                        {
                            new DepotAI.SpawnPoint
                            {
                                m_position = ai.m_spawnPosition2,
                                m_target = ai.m_spawnTarget2
                            }
                        };
                    }
                    for (var index = 0; index < mSpawnPoints2.Length; ++index)
                    {
                        PlaceSpecialPoint(info, matrix4x4, mSpawnPoints2[index].m_position, SpecialPointType.SpawnPoint2Position);
                        PlaceSpecialPoint(info, matrix4x4, mSpawnPoints2[index].m_target, SpecialPointType.SpawnPoint2Target);
                    }   
                }
            }
            if (info.m_buildingAI is CargoStationAI ai2)
            {
                if (ai2.m_transportInfo != null)
                {
                    PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnPosition, SpecialPointType.SpawnPointPosition);
                    PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnTarget, SpecialPointType.SpawnPointTarget);                    
                }
                if (ai2.m_transportInfo2 != null)
                {
                    PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnPosition2, SpecialPointType.SpawnPoint2Position);
                    PlaceSpecialPoint(info, matrix4x4, ai2.m_spawnTarget2, SpecialPointType.SpawnPoint2Target);                    
                }
                PlaceSpecialPoint(info, matrix4x4, ai2.m_truckSpawnPosition, SpecialPointType.TruckSpawnPosition);
                PlaceSpecialPoint(info, matrix4x4, ai2.m_truckUnspawnPosition, SpecialPointType.TruckDespawnPosition);
            }
            if (info.m_buildingAI is FishingHarborAI ai3)
            {
                PlaceSpecialPoint(info, matrix4x4, ai3.m_boatUnspawnPosition, SpecialPointType.DespawnPointPosition);
                PlaceSpecialPoint(info, matrix4x4, ai3.m_boatSpawnPosition, SpecialPointType.SpawnPointPosition);
                PlaceSpecialPoint(info, matrix4x4, ai3.m_boatUnspawnTarget, SpecialPointType.DespawnPointTarget);
                PlaceSpecialPoint(info, matrix4x4, ai3.m_boatSpawnTarget, SpecialPointType.SpawnPointTarget);
            }
        }

        private static void PlaceSpecialPoint(BuildingInfo info, Matrix4x4 matrix4x4, Vector3 pointLocation, SpecialPointType pointType)
        {
            var vector3 = matrix4x4.MultiplyPoint(pointLocation).MirrorZ();
            DisableLimits = true;
            Scripts.CreateSpecialPoint(info, pointType, vector3);
            DisableLimits = false;
        }
    }
}
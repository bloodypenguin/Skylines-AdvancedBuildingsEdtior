using System.Linq;

namespace AdvancedBuildingsEditor
{
    public static class SpecialPoints
    {
        public const string SpawnPointTarget = "Spawn Point Target";
        public const string SpawnPointPosition = "Spawn Point Position";
        public const string DespawnPointTarget = "Despawn Point Target";
        public const string DespawnPointPosition = "Despawn Point Position";


        public const string SpawnPoint2Target = "Spawn Point 2 Target";
        public const string SpawnPoint2Position = "Spawn Point 2 Position";
        public const string TruckSpawnPosition = "Truck Spawn Point";
        public const string TruckDespawnPosition = "Truck Despawn Point";

        public static int CountSpecialPoints()
        {
            return PropManager.instance.m_props.m_buffer.
                Where(propInstance => propInstance.m_flags != 0).
                Count(propInstance => GetSpecialPointType(propInstance.Info) != SpecialPointType.Unknown);
        }

        public static SpecialPointType GetSpecialPointType(PropInfo info)
        {
            var pointType = SpecialPointType.Unknown;
            if (info == null)
            {
                return pointType;
            }
            switch (info.name)
            {
                case SpecialPoints.SpawnPointTarget:
                    pointType = SpecialPointType.SpawnPointTarget;
                    break;
                case SpecialPoints.SpawnPointPosition:
                    pointType = SpecialPointType.SpawnPointPosition;
                    break;
                case SpecialPoints.SpawnPoint2Target:
                    pointType = SpecialPointType.SpawnPoint2Target;
                    break;
                case SpecialPoints.SpawnPoint2Position:
                    pointType = SpecialPointType.SpawnPoint2Position;
                    break;
                case SpecialPoints.TruckDespawnPosition:
                    pointType = SpecialPointType.TruckDespawnPosition;
                    break;
                case SpecialPoints.TruckSpawnPosition:
                    pointType = SpecialPointType.TruckSpawnPosition;
                    break;
                case SpecialPoints.DespawnPointTarget:
                    pointType = SpecialPointType.DespawnPointTarget;
                    break;
                case SpecialPoints.DespawnPointPosition:
                    pointType = SpecialPointType.DespawnPointPosition;
                    break;
                default:
                    break;
            }
            return pointType;
        }

        public static PropInfo GetSpecialPointProp(SpecialPointType pointType)
        {

            switch (pointType)
            {
                case SpecialPointType.Unknown:
                    return null;
                default:
                    return null;
                case SpecialPointType.SpawnPointTarget:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPointTarget);
                case SpecialPointType.SpawnPointPosition:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPointPosition);
                case SpecialPointType.DespawnPointTarget:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.DespawnPointTarget);
                case SpecialPointType.DespawnPointPosition:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.DespawnPointPosition);
                case SpecialPointType.SpawnPoint2Target:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPoint2Target);
                case SpecialPointType.SpawnPoint2Position:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.SpawnPoint2Position);
                case SpecialPointType.TruckDespawnPosition:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.TruckDespawnPosition);
                case SpecialPointType.TruckSpawnPosition:
                    return PrefabCollection<PropInfo>.FindLoaded(SpecialPoints.TruckSpawnPosition);
            }
        }

        public static bool IsAppropriatePointType(BuildingInfo buildingInfo, SpecialPointType pointType)
        {
            if (buildingInfo == null || pointType == SpecialPointType.Unknown)
            {
                return false;
            }
            if (buildingInfo.m_buildingAI is DepotAI)
            {
                if (pointType != SpecialPointType.SpawnPointTarget &&
                    pointType != SpecialPointType.SpawnPointPosition&&
                    pointType != SpecialPointType.SpawnPoint2Position &&
                    pointType != SpecialPointType.SpawnPoint2Target)
                {
                    return false;
                }
            }
            else if (buildingInfo.m_buildingAI is CargoStationAI)
            {
                if (pointType != SpecialPointType.SpawnPointTarget &&
                    pointType != SpecialPointType.SpawnPointPosition &&
                    pointType != SpecialPointType.SpawnPoint2Position &&
                    pointType != SpecialPointType.SpawnPoint2Target &&
                    pointType != SpecialPointType.TruckSpawnPosition &&
                    pointType != SpecialPointType.TruckDespawnPosition)
                {
                    return false;
                }
            }
            else if (buildingInfo.m_buildingAI is FishingHarborAI)
            {
                if (pointType != SpecialPointType.SpawnPointTarget &&
                    pointType != SpecialPointType.SpawnPointPosition &&
                    pointType != SpecialPointType.DespawnPointTarget &&
                    pointType != SpecialPointType.DespawnPointPosition)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
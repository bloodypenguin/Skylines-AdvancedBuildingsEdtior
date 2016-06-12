namespace AdvancedBuildingsEditor
{
    public static class SpecialPoints
    {
        public const string SpawnPointTarget = "Spawn Point Target";
        public const string SpawnPointPosition = "Spawn Point Position";
        public const string TruckSpawnPosition = "Truck Spawn Point";
        public const string TruckDespawnPosition = "Truck Despawn Point";

        public static bool IsSpecialProp(PropInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return info.name == SpawnPointPosition || info.name == SpawnPointTarget || info.name == TruckDespawnPosition || info.name == TruckDespawnPosition;
        }

        public static SpecialPointType GetSpecialPointType(PropInfo info)
        {
            var pointType = SpecialPointType.Unknown;
            if (info.name == SpecialPoints.SpawnPointTarget)
            {
                pointType = SpecialPointType.SpawnPointTarget;
            }
            else if (info.name == SpecialPoints.SpawnPointPosition)
            {
                pointType = SpecialPointType.SpawnPointPosition;
            }
            else if (info.name == SpecialPoints.TruckDespawnPosition)
            {
                pointType = SpecialPointType.TruckDespawnPosition;
            }
            else if (info.name == SpecialPoints.TruckSpawnPosition)
            {
                pointType = SpecialPointType.TruckSpawnPosition;
            }
            return pointType;
        }

    }
}
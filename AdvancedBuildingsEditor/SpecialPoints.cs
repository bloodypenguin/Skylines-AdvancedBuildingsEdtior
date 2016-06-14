namespace AdvancedBuildingsEditor
{
    public static class SpecialPoints
    {
        public const string SpawnPointTarget = "Spawn Point Target";
        public const string SpawnPointPosition = "Spawn Point Position";

        public const string SpawnPoint2Target = "Spawn Point 2 Target";
        public const string SpawnPoint2Position = "Spawn Point 2 Position";
        public const string TruckSpawnPosition = "Truck Spawn Point";
        public const string TruckDespawnPosition = "Truck Despawn Point";

        public static bool IsSpecialPoint(PropInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return info.name == SpawnPointPosition || info.name == SpawnPointTarget || info.name == SpawnPoint2Position || info.name == SpawnPoint2Target  || info.name == TruckDespawnPosition || info.name == TruckDespawnPosition;
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
            else if (info.name == SpecialPoints.SpawnPoint2Target)
            {
                pointType = SpecialPointType.SpawnPoint2Target;
            }
            else if (info.name == SpecialPoints.SpawnPoint2Position)
            {
                pointType = SpecialPointType.SpawnPoint2Position;
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
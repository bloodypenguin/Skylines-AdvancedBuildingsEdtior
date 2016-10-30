using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public class PropInitializer : MonoBehaviour
    {
        private bool _isInitialized;
        public Dictionary<string, PropInfo> _customPrefabs;
        private static readonly Dictionary<string, PropInfo> OriginalPrefabs = new Dictionary<string, PropInfo>();

        public void Awake()
        {
            DontDestroyOnLoad(this);
            _customPrefabs = new Dictionary<string, PropInfo>();
            OriginalPrefabs.Clear();
        }


        public void SetupPropColors()
        {
            _customPrefabs[SpecialPoints.SpawnPointPosition].m_color0 = Color.blue;
            _customPrefabs[SpecialPoints.SpawnPointPosition].m_color1 = Color.blue;
            _customPrefabs[SpecialPoints.SpawnPointPosition].m_color2 = Color.blue;
            _customPrefabs[SpecialPoints.SpawnPointPosition].m_color3 = Color.blue;
            _customPrefabs[SpecialPoints.SpawnPointTarget].m_color0 = Color.cyan;
            _customPrefabs[SpecialPoints.SpawnPointTarget].m_color1 = Color.cyan;
            _customPrefabs[SpecialPoints.SpawnPointTarget].m_color2 = Color.cyan;
            _customPrefabs[SpecialPoints.SpawnPointTarget].m_color3 = Color.cyan;

            _customPrefabs[SpecialPoints.SpawnPoint2Position].m_color0 = Color.magenta;
            _customPrefabs[SpecialPoints.SpawnPoint2Position].m_color1 = Color.magenta;
            _customPrefabs[SpecialPoints.SpawnPoint2Position].m_color2 = Color.magenta;
            _customPrefabs[SpecialPoints.SpawnPoint2Position].m_color3 = Color.magenta;
            _customPrefabs[SpecialPoints.SpawnPoint2Target].m_color0 = Color.yellow;
            _customPrefabs[SpecialPoints.SpawnPoint2Target].m_color1 = Color.yellow;
            _customPrefabs[SpecialPoints.SpawnPoint2Target].m_color2 = Color.yellow;
            _customPrefabs[SpecialPoints.SpawnPoint2Target].m_color3 = Color.yellow;

            _customPrefabs[SpecialPoints.TruckSpawnPosition].m_color0 = Color.green;
            _customPrefabs[SpecialPoints.TruckSpawnPosition].m_color1 = Color.green;
            _customPrefabs[SpecialPoints.TruckSpawnPosition].m_color2 = Color.green;
            _customPrefabs[SpecialPoints.TruckSpawnPosition].m_color3 = Color.green;
            _customPrefabs[SpecialPoints.TruckDespawnPosition].m_color0 = Color.red;
            _customPrefabs[SpecialPoints.TruckDespawnPosition].m_color1 = Color.red;
            _customPrefabs[SpecialPoints.TruckDespawnPosition].m_color2 = Color.red;
            _customPrefabs[SpecialPoints.TruckDespawnPosition].m_color3 = Color.red;
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                _customPrefabs.Clear();
                OriginalPrefabs.Clear();
                _isInitialized = false;
            }
        }

        public void Update()
        {
            if (_isInitialized)
            {
                return;
            }
            try
            {
                if (!SimulationManager.exists || (SimulationManager.instance.m_metaData == null))
                {
                    return;
                }
                if (SimulationManager.instance.m_metaData.m_updateMode != SimulationManager.UpdateMode.LoadAsset &&
                    SimulationManager.instance.m_metaData.m_updateMode != SimulationManager.UpdateMode.NewAsset)
                {
                    _isInitialized = true;
                    Destroy(this);
                }
                var parent = GameObject.Find(SimulationManager.instance.m_metaData.m_environment + " Collections");
                foreach (var t in from Transform t in parent.transform where t.name == "Common" select t)
                {
                    t.gameObject.GetComponent<PropCollection>();
                }
            }
            catch (Exception)
            {
                return;
            }
            Loading.QueueLoadingAction(() =>
            {
                InitializeImpl();
                PrefabCollection<PropInfo>.InitializePrefabs("Advanced Buildings Editor", _customPrefabs.Values.ToArray(), null);
            });
            _isInitialized = true;
        }

        private void InitializeImpl()
        {
            CreatePrefab(SpecialPoints.SpawnPointPosition, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab(SpecialPoints.SpawnPointTarget, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab(SpecialPoints.SpawnPoint2Position, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab(SpecialPoints.SpawnPoint2Target, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab(SpecialPoints.TruckDespawnPosition, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab(SpecialPoints.TruckSpawnPosition, "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
        }

        protected void CreatePrefab(string newPrefabName, string originalPrefabName, Action<PropInfo> setupAction)
        {
            var originalPrefab = FindOriginalPrefab(originalPrefabName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("PropInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalPrefabName, newPrefabName);
                return;
            }
            if (_customPrefabs.ContainsKey(newPrefabName))
            {
                return;
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newPrefabName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("PropInitializer#CreatePrefab - Couldn't make prefab '{0}'", newPrefabName);
                return;
            }
            setupAction.Invoke(newPrefab);
            _customPrefabs.Add(newPrefabName, newPrefab);
            Util.AddLocale("PROPS", newPrefabName, newPrefabName, newPrefabName);

        }

        protected static PropInfo FindOriginalPrefab(string originalPrefabName)
        {
            PropInfo foundPrefab;
            if (OriginalPrefabs.TryGetValue(originalPrefabName, out foundPrefab))
            {
                return foundPrefab;
            }
            foundPrefab = Resources.FindObjectsOfTypeAll<PropInfo>().
            FirstOrDefault(netInfo => netInfo.name == originalPrefabName);
            OriginalPrefabs.Add(originalPrefabName, foundPrefab);
            return foundPrefab;
        }
    }
}
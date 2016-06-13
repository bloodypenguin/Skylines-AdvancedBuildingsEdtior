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
                PrefabCollection<PropInfo>.InitializePrefabs("Sub Buildings Editor", _customPrefabs.Values.ToArray(), null);
            });
            _isInitialized = true;
        }

        private void InitializeImpl()
        {
            CreatePrefab("Spawn Point Position", "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab("Spawn Point Target", "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab("Truck Spawn Point", "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
            CreatePrefab("Truck Despawn Point", "Door Marker", info =>
            {
                info.m_availableIn = ItemClass.Availability.AssetEditor;
            });
        }

        protected void CreatePrefab(string newPrefabName, string originalPrefabName, Action<PropInfo> setupAction)
        {
            var originalPrefab = FindOriginalPrefab(originalPrefabName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalPrefabName, newPrefabName);
                return;
            }
            if (_customPrefabs.ContainsKey(newPrefabName))
            {
                return;
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newPrefabName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Couldn't make prefab '{0}'", newPrefabName);
                return;
            }
            setupAction.Invoke(newPrefab);
            _customPrefabs.Add(newPrefabName, newPrefab);

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
using AdvancedBuildingsEditor.Detours;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static bool initialized;
        public static PropInitializer Container;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (Container == null)
            {
                Container = new GameObject("SubBuildingsEditor").AddComponent<PropInitializer>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            DestroyContainer();
            if (!initialized)
            {
                return;
            }
            Redirector<BuildingDecorationDetour>.Revert();
            Redirector<LoadAssetPanelDetour>.Revert();
            initialized = false;
        }

        private static void DestroyContainer()
        {
            if (Container == null)
            {
                return;
            }
            Object.Destroy(Container.gameObject);
            Container = null;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.LoadAsset && mode != LoadMode.NewAsset)
            {
                Redirector<BuildingDecorationDetour>.Revert();
                Redirector<LoadAssetPanelDetour>.Revert();
                Redirector<PropManagerDetour>.Revert();
                DestroyContainer();
                return;
            }
            if (initialized)
            {
                return;
            }
            Container._customPrefabs[SpecialPoints.SpawnPointPosition].m_color0 = Color.blue;
            Container._customPrefabs[SpecialPoints.SpawnPointPosition].m_color1 = Color.blue;
            Container._customPrefabs[SpecialPoints.SpawnPointPosition].m_color2 = Color.blue;
            Container._customPrefabs[SpecialPoints.SpawnPointPosition].m_color3 = Color.blue;
            Container._customPrefabs[SpecialPoints.SpawnPointTarget].m_color0 = Color.cyan;
            Container._customPrefabs[SpecialPoints.SpawnPointTarget].m_color1 = Color.cyan;
            Container._customPrefabs[SpecialPoints.SpawnPointTarget].m_color2 = Color.cyan;
            Container._customPrefabs[SpecialPoints.SpawnPointTarget].m_color3 = Color.cyan;
            Container._customPrefabs[SpecialPoints.TruckSpawnPosition].m_color0 = Color.green;
            Container._customPrefabs[SpecialPoints.TruckSpawnPosition].m_color1 = Color.green;
            Container._customPrefabs[SpecialPoints.TruckSpawnPosition].m_color2 = Color.green;
            Container._customPrefabs[SpecialPoints.TruckSpawnPosition].m_color3 = Color.green;
            Container._customPrefabs[SpecialPoints.TruckDespawnPosition].m_color0 = Color.red;
            Container._customPrefabs[SpecialPoints.TruckDespawnPosition].m_color1 = Color.red;
            Container._customPrefabs[SpecialPoints.TruckDespawnPosition].m_color2 = Color.red;
            Container._customPrefabs[SpecialPoints.TruckDespawnPosition].m_color3 = Color.red;

            Redirector<BuildingDecorationDetour>.Deploy();
            Redirector<LoadAssetPanelDetour>.Deploy();
            Redirector<PropManagerDetour>.Deploy();
            SubBuildingsEnablerFormat.InitializeBuildingsWithSubBuildings();

            UIView.GetAView().AddUIComponent(typeof(Panel));
            initialized = true;
        }


        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Redirector<PropManagerDetour>.Revert();
        }
    }
}
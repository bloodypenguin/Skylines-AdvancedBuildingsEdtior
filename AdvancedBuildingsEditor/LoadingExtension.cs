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
            if (loading.currentMode != AppMode.AssetEditor)
            {
                return;
            }
            if (Container == null)
            {
                Container = new GameObject("SubBuildingsEditor").AddComponent<PropInitializer>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            Reset();
            initialized = false;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.LoadAsset && mode != LoadMode.NewAsset)
            {
                Reset();
                Redirector<PropManagerDetour>.Revert();
                Redirector<TreeManagerDetour>.Revert();
                return;
            }
            if (initialized)
            {
                return;
            }
            Container.SetupPropColors();

            Redirector<BuildingDecorationDetour>.Deploy();
            Redirector<PropManagerDetour>.Deploy();
            Redirector<TreeManagerDetour>.Deploy();
            Redirector<AssetEditorChirpPanelDetour>.Deploy();
            Redirector<DecorationPropertiesPanelDetour>.Deploy();
            SubBuildingsEnablerFormat.InitializeBuildingsWithSubBuildings();

            UIView.GetAView().AddUIComponent(typeof(Panel));
            initialized = true;
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Redirector<PropManagerDetour>.Revert();
            Redirector<TreeManagerDetour>.Revert();
        }

        private static void Reset()
        {
            DestroyContainer();
            Redirector<BuildingDecorationDetour>.Revert();
            Redirector<AssetEditorChirpPanelDetour>.Revert();
            Redirector<DecorationPropertiesPanelDetour>.Revert();
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
    }
}
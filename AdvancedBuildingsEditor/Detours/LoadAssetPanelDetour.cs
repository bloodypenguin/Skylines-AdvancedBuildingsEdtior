using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.Steamworks;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(LoadAssetPanel))]
    public class LoadAssetPanelDetour : LoadAssetPanel
    {

        [RedirectMethod]
        public void OnLoad()
        {
            this.CloseEverything();
            var saveList = this.Find<UIListBox>("SaveList");
            var m_LastSaveName = typeof(LoadAssetPanel).GetField("m_LastSaveName",
                BindingFlags.NonPublic | BindingFlags.Static);

            m_LastSaveName.SetValue(null, this.GetListingName(saveList.selectedIndex));
            SaveAssetPanel.lastLoadedName = (string)m_LastSaveName.GetValue(null);
            CustomAssetMetaData listingMetaData = this.GetListingMetaData(saveList.selectedIndex);
            this.PrintModsInfo(listingMetaData);
            SaveAssetPanel.lastLoadedAsset = this.SafeGetAssetName(listingMetaData, listingMetaData.assetRef.package);
            string listingPackageName = this.GetListingPackageName(saveList.selectedIndex);
            PublishedFileId publishedFileId = PublishedFileId.invalid;
            ulong result;
            if (ulong.TryParse(listingPackageName, out result))
                publishedFileId = new PublishedFileId(result);
            Singleton<SimulationManager>.instance.m_metaData.m_WorkshopPublishedFileId = publishedFileId;
            Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier = listingMetaData.guid;
            UIView.library.Hide(this.GetType().Name, 1);
            GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("GameController");
            if (!((UnityEngine.Object)gameObjectWithTag != (UnityEngine.Object)null))
                return;
            ToolController component1 = gameObjectWithTag.GetComponent<ToolController>();
            if (listingMetaData.type == CustomAssetMetaData.Type.Vehicle)
            {
                Package package = listingMetaData.assetRef.package;
                Package.AssetType[] assetTypeArray = new Package.AssetType[1] { UserAssetType.CustomAssetMetaData };
                foreach (Package.Asset filterAsset in package.FilterAssets(assetTypeArray))
                {
                    CustomAssetMetaData customAssetMetaData = filterAsset.Instantiate<CustomAssetMetaData>();
                    if (customAssetMetaData.assetRef.checksum != listingMetaData.assetRef.checksum)
                    {
                        GameObject gameObject = customAssetMetaData.assetRef.Instantiate<GameObject>();
                        gameObject.name = customAssetMetaData.assetRef.package.packageName + "." + gameObject.name;
                        gameObject.SetActive(false);
                        VehicleInfo component2 = gameObject.GetComponent<VehicleInfo>();
                        if ((UnityEngine.Object)component2 != (UnityEngine.Object)null &&
                            (UnityEngine.Object)PrefabCollection<VehicleInfo>.FindLoaded(gameObject.name) ==
                            (UnityEngine.Object)null)
                        {
                            PrefabCollection<VehicleInfo>.InitializePrefabs("Custom Assets", component2, (string)null);
                            if ((UnityEngine.Object)component2.m_lodObject != (UnityEngine.Object)null)
                                component2.m_lodObject.SetActive(false);
                            PrefabCollection<VehicleInfo>.BindPrefabs();
                        }
                    }
                }
            }
            GameObject gameObject1 = listingMetaData.assetRef.Instantiate<GameObject>();
            if (!((UnityEngine.Object)gameObject1 != (UnityEngine.Object)null))
                return;
            PrefabInfo component3 = gameObject1.GetComponent<PrefabInfo>();
            if ((UnityEngine.Object)component3.m_Atlas != (UnityEngine.Object)null &&
                component3.m_InfoTooltipThumbnail != null &&
                (component3.m_InfoTooltipThumbnail != string.Empty &&
                 component3.m_Atlas[component3.m_InfoTooltipThumbnail] != (UITextureAtlas.SpriteInfo)null))
                component3.m_InfoTooltipAtlas = component3.m_Atlas;
            if (component3 is CitizenInfo)
            {
                component3.InitializePrefabInstance(component3);
                component3.CheckReferences();
                component3.m_prefabInitialized = true;
            }
            else
            {
                //begin mod
                var buildingInfo = component3 as BuildingInfo;
                if (buildingInfo != null && (buildingInfo.m_subBuildings == null || buildingInfo.m_subBuildings.Length == 0))
                {
                    SubBuildingsEnablerFormat.SetSubBuildings(buildingInfo, listingMetaData.assetRef);
                }
                //end mod
                component3.InitializePrefab();
                component3.CheckReferences();
                component3.m_prefabInitialized = true;
            }
            component1.m_editPrefabInfo = component3;
            SaveAssetPanel.lastAssetDescription = this.SafeGetAssetDesc(listingMetaData,
                listingMetaData.assetRef.package);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectReverse]
        private string SafeGetAssetName(CustomAssetMetaData metaData, Package package)
        {
            UnityEngine.Debug.Log("SafeGetAssetName");
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectReverse]
        private string SafeGetAssetDesc(CustomAssetMetaData metaData, Package package)
        {
            UnityEngine.Debug.Log("SafeGetAssetDesc");
            return null;
        }
    }
}
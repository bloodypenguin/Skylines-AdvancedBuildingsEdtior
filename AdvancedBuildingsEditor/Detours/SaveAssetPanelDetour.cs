using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(SaveAssetPanel))]
    public class SaveAssetPanelDetour : SaveAssetPanel
    {

        private static Task m_PackageSaveTask;


        [RedirectMethod]
        private IEnumerator SaveAsset(string saveName, string assetName, string description, bool ignoreThumbnail)
        {
            SaveAsset_c__Iterator2D assetCIterator2D = new SaveAsset_c__Iterator2D();
            assetCIterator2D.ignoreThumbnail = ignoreThumbnail;
            assetCIterator2D.saveName = saveName;
            assetCIterator2D.assetName = assetName;
            assetCIterator2D.description = description;
            assetCIterator2D.ignoreThumbnail = ignoreThumbnail;
            assetCIterator2D.saveName = saveName;
            assetCIterator2D.assetName = assetName;
            assetCIterator2D.description = description;
            assetCIterator2D._f__this = this;
            return (IEnumerator)assetCIterator2D;
        }


        [RedirectMethod]
        private static List<BuildingInfo.SubInfo> InstantiateSubBuildings(SaveAssetPanel panel, string saveName)
        {
            List<BuildingInfo.SubInfo> subInfoList = new List<BuildingInfo.SubInfo>();
            BuildingInfo buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if ((UnityEngine.Object)buildingInfo != (UnityEngine.Object)null)
            {
                foreach (BuildingInfo.SubInfo mSubBuilding in buildingInfo.m_subBuildings)
                {
                    if ((UnityEngine.Object)mSubBuilding.m_buildingInfo != (UnityEngine.Object)null)
                    {
                        //begin mod
                        if (mSubBuilding.m_buildingInfo.m_isCustomContent)
                        {
                            UnityEngine.Debug.Log($"Building {mSubBuilding.m_buildingInfo.name} is custom");
                            //end mod
                            GameObject gameObject = InstantiateSubBuilding(panel, mSubBuilding.m_buildingInfo.gameObject, saveName);
                            subInfoList.Add(new BuildingInfo.SubInfo()
                            {
                                m_buildingInfo = gameObject.GetComponent<BuildingInfo>(),
                                m_position = mSubBuilding.m_position,
                                m_angle = mSubBuilding.m_angle,
                                m_fixedHeight = mSubBuilding.m_fixedHeight
                            });
                            //begin mod
                        }
                        else
                        {
                            subInfoList.Add(new BuildingInfo.SubInfo()
                            {
                                m_buildingInfo = mSubBuilding.m_buildingInfo,
                                m_position = mSubBuilding.m_position,
                                m_angle = mSubBuilding.m_angle,
                                m_fixedHeight = mSubBuilding.m_fixedHeight
                            });

                        }
                        //end mod
                    }
                }
            }
            return subInfoList;
        }

        [RedirectReverse]
        private static GameObject InstantiateSubBuilding(SaveAssetPanel panel, GameObject template, string saveName)
        {
            UnityEngine.Debug.Log("AAA");
            return null;
        }

        [RedirectReverse]
        private static IEnumerator ThreadSaveDecoration(SaveAssetPanel panel, PrefabInfo newInfo)
        {
            UnityEngine.Debug.Log("AAA");
            return null;
        }

        [RedirectReverse]
        private static GameObject GetLODObject(SaveAssetPanel panel)
        {
            UnityEngine.Debug.Log("AAA");
            return null;
        }

        [RedirectReverse]
        private static void ReloadThumbnail(SaveAssetPanel panel, bool regenerate)
        {
            UnityEngine.Debug.Log("AAA");
        }

        //redirect reverse?
        private static string GetSavePathName(string saveName)
        {
            return Path.Combine(DataLocation.assetsPath, PathUtils.AddExtension(PathEscaper.Escape(saveName), PackageManager.packageExtension));
        }

        [RedirectReverse]
        protected static ModInfo[] EmbedModInfo(SaveAssetPanel panel)
        {
            UnityEngine.Debug.Log("AAA");
            return null;
        }

        [RedirectReverse]
        private static void PerformanceWarning(SaveAssetPanel panel, int triangles, int textureHeight, int textureWidth, int lodTriangles, int lodTextureHeight, int lodTextureWidth)
        {
            UnityEngine.Debug.Log("AAA");
        }


        private sealed class SaveAsset_c__Iterator2D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal AsyncTask task___0;
            internal string snapShot___1;
            internal UITextureAtlas tempThumbAtlas___2;
            internal UITextureAtlas tempInfoAtlas___3;
            internal bool ignoreThumbnail;
            internal bool hasTooltip___4;
            internal Texture2D[] textures___5;
            internal VehicleInfo vi___6;
            internal GameObject trailerObject___7;
            internal string trailerObjectName___8;
            internal int dotIndex___9;
            internal int triangles___10;
            internal int textureHeight___11;
            internal int textureWidth___12;
            internal GameObject assetObject___13;
            internal Texture mainTexture___14;
            internal int lodTriangles___15;
            internal int lodTextureHeight___16;
            internal int lodTextureWidth___17;
            internal GameObject lodObject___18;
            internal Texture lodTexture___19;
            internal List<BuildingInfo.SubInfo> subBuildings___20; //only custom buildings
            internal List<BuildingInfo.SubInfo> allSubBuildings; //vanilla included

            internal List<string> subBuildingNames___21;
            internal List<GameObject> subBuildingObjects___22;
            internal string saveName;
            internal List<BuildingInfo.SubInfo>.Enumerator __s_51___23;
            internal BuildingInfo.SubInfo subInfo___24;
            internal GameObject o___25;
            internal PropInfo pi___26;
            internal List<PropInfo.Variation> propVariations___27;
            internal PropInfo.Variation[] __s_52___28;
            internal int __s_53___29;
            internal PropInfo.Variation vari___30;
            internal int dotIndex___31;
            internal TreeInfo ti___32;
            internal List<TreeInfo.Variation> treeVariations___33;
            internal TreeInfo.Variation[] __s_54___34;
            internal int __s_55___35;
            internal TreeInfo.Variation vari___36;
            internal int dotIndex___37;
            internal string assetName;
            internal string description;
            internal int __PC;
            internal object __current;
            //begin mod
            private int m_CurrentSnapshot
            {
                get
                {
                    return (int)_f__this.GetType().GetField("m_CurrentSnapshot", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_f__this);
                }
            }

            private List<string> m_SnapshotPaths
            {
                get
                {
                    return (List<string>)_f__this.GetType().GetField("m_SnapshotPaths", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_f__this);
                }
            }

            private UIButton m_Thumbnail
            {
                get
                {
                    return (UIButton)_f__this.GetType().GetField("m_Thumbnail", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_f__this);
                }
            }

            private UISprite m_TooltipImage
            {
                get
                {
                    return (UISprite)_f__this.GetType().GetField("m_TooltipImage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_f__this);
                }
            }

            private bool m_IsSaving
            {
                set
                {
                    UnityEngine.Debug.Log(_f__this.GetType());
                    _f__this.GetType().GetField("m_IsSaving", BindingFlags.NonPublic|BindingFlags.Static).SetValue(_f__this, value);
                }
            }

            //end mod

            internal SaveAssetPanel _f__this;
            private static System.Action _f__am__cache31;

            public object Current
            {
                get
                {
                    return this.__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.__current;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint)this.__PC;
                this.__PC = -1;
                switch (num)
                {
                    case 0:
                        m_IsSaving = true;
                        UIView.library.Hide(this._f__this.GetType().Name, 1);
                        this.task___0 = Singleton<SimulationManager>.instance.AddAction(SaveAssetPanelDetour.ThreadSaveDecoration(this._f__this, ToolsModifierControl.toolController.m_editPrefabInfo));
                        goto case 1;
                    case 1:
                        if (!this.task___0.completedOrFailed)
                        {
                            this.__current = (object)0;
                            this.__PC = 1;
                            return true;
                        }
                        if (SaveAssetPanelDetour.m_PackageSaveTask != null)
                            SaveAssetPanelDetour.m_PackageSaveTask.Wait();
                        this.snapShot___1 = (string)null;
                        if (m_CurrentSnapshot >= 0 && m_CurrentSnapshot < m_SnapshotPaths.Count)
                            this.snapShot___1 = m_SnapshotPaths[m_CurrentSnapshot];
                        this.tempThumbAtlas___2 = ToolsModifierControl.toolController.m_editPrefabInfo.m_Atlas;
                        this.tempInfoAtlas___3 = ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipAtlas;
                        if (!this.ignoreThumbnail && AssetImporterThumbnails.NeedThumbnails(ToolsModifierControl.toolController.m_editPrefabInfo) && ((UnityEngine.Object)m_Thumbnail != (UnityEngine.Object)null && (UnityEngine.Object)m_Thumbnail.atlas != (UnityEngine.Object)null) && m_Thumbnail.atlas[m_Thumbnail.normalFgSprite] != (UITextureAtlas.SpriteInfo)null)
                        {
                            this.hasTooltip___4 = (UnityEngine.Object)m_TooltipImage != (UnityEngine.Object)null && (UnityEngine.Object)m_TooltipImage.atlas != (UnityEngine.Object)null && m_TooltipImage.atlas[m_TooltipImage.spriteName] != (UITextureAtlas.SpriteInfo)null;
                            this.textures___5 = new Texture2D[!this.hasTooltip___4 ? 5 : 6];
                            this.textures___5[0] = m_Thumbnail.atlas[m_Thumbnail.normalFgSprite].texture;
                            this.textures___5[1] = m_Thumbnail.atlas[m_Thumbnail.focusedFgSprite].texture;
                            this.textures___5[2] = m_Thumbnail.atlas[m_Thumbnail.hoveredFgSprite].texture;
                            this.textures___5[3] = m_Thumbnail.atlas[m_Thumbnail.pressedFgSprite].texture;
                            this.textures___5[4] = m_Thumbnail.atlas[m_Thumbnail.disabledFgSprite].texture;
                            if (this.hasTooltip___4)
                                this.textures___5[5] = m_TooltipImage.atlas[m_TooltipImage.spriteName].texture;
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_Atlas = AssetImporterThumbnails.CreateThumbnailAtlas(this.textures___5, ToolsModifierControl.toolController.m_editPrefabInfo.gameObject.name);
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_Thumbnail = m_Thumbnail.normalFgSprite;
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipAtlas = (UITextureAtlas)null;
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipThumbnail = m_TooltipImage.spriteName;
                        }
                        else
                        {
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_Atlas = (UITextureAtlas)null;
                            ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipAtlas = (UITextureAtlas)null;
                        }
                        this.vi___6 = ToolsModifierControl.toolController.m_editPrefabInfo as VehicleInfo;
                        this.trailerObject___7 = (GameObject)null;
                        this.trailerObjectName___8 = (string)null;
                        if ((UnityEngine.Object)this.vi___6 != (UnityEngine.Object)null && this.vi___6.m_trailers != null && this.vi___6.m_trailers.Length > 0)
                        {
                            this.trailerObject___7 = this.vi___6.m_trailers[0].m_info.gameObject;
                            this.dotIndex___9 = this.trailerObject___7.name.LastIndexOf(".");
                            this.trailerObject___7.name = this.dotIndex___9 >= 0 ? this.trailerObject___7.name.Remove(0, this.dotIndex___9 + 1) : this.trailerObject___7.name;
                            this.trailerObjectName___8 = this.trailerObject___7.name;
                        }
                        this.triangles___10 = 0;
                        this.textureHeight___11 = 0;
                        this.textureWidth___12 = 0;
                        try
                        {
                            this.assetObject___13 = ToolsModifierControl.toolController.m_editPrefabInfo.gameObject;
                            this.triangles___10 = this.assetObject___13.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
                            this.mainTexture___14 = this.assetObject___13.GetComponent<MeshRenderer>().material.mainTexture;
                            this.textureHeight___11 = this.mainTexture___14.height;
                            this.textureWidth___12 = this.mainTexture___14.width;
                        }
                        catch
                        {
                        }
                        this.lodTriangles___15 = 0;
                        this.lodTextureHeight___16 = 0;
                        this.lodTextureWidth___17 = 0;
                        try
                        {
                            this.lodObject___18 = SaveAssetPanelDetour.GetLODObject(this._f__this);
                            this.lodTriangles___15 = this.lodObject___18.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
                            this.lodTexture___19 = this.lodObject___18.GetComponent<MeshRenderer>().material.mainTexture;
                            this.lodTextureHeight___16 = this.lodTexture___19.height;
                            this.lodTextureWidth___17 = this.lodTexture___19.width;
                        }
                        catch
                        {
                        }
                        SaveAssetPanelDetour.PerformanceWarning(this._f__this, this.triangles___10, this.textureHeight___11, this.textureWidth___12, this.lodTriangles___15, this.lodTextureHeight___16, this.lodTextureWidth___17);
                        //begin mod
                        this.allSubBuildings = new List<BuildingInfo.SubInfo>();
                        this.subBuildings___20 = new List<BuildingInfo.SubInfo>();
                        //end mod
                        this.subBuildingNames___21 = new List<string>();
                        this.subBuildingObjects___22 = new List<GameObject>();
                        if (ToolsModifierControl.toolController.m_editPrefabInfo is BuildingInfo)
                        {
                            //begin mod
                            this.allSubBuildings = SaveAssetPanelDetour.InstantiateSubBuildings(this._f__this, this.saveName);
                            this.subBuildings___20 = allSubBuildings.Where(sb => (sb?.m_buildingInfo?.m_isCustomContent ?? true)).ToList();
                            //end mod
                            this.__s_51___23 = this.subBuildings___20.GetEnumerator();
                            try
                            {
                                while (this.__s_51___23.MoveNext())
                                {
                                    this.subInfo___24 = this.__s_51___23.Current;
                                    this.o___25 = this.subInfo___24.m_buildingInfo.gameObject;
                                    this.subBuildingNames___21.Add(this.o___25.name);
                                    this.subBuildingObjects___22.Add(this.o___25);
                                }
                            }
                            finally
                            {
                                ((IDisposable)this.__s_51___23).Dispose();
                            }
                        }
                        this.pi___26 = ToolsModifierControl.toolController.m_editPrefabInfo as PropInfo;
                        this.propVariations___27 = new List<PropInfo.Variation>();
                        if ((UnityEngine.Object)this.pi___26 != (UnityEngine.Object)null)
                        {
                            this.__s_52___28 = this.pi___26.m_variations;
                            for (this.__s_53___29 = 0; this.__s_53___29 < this.__s_52___28.Length; this.__s_53___29 = this.__s_53___29 + 1)
                            {
                                this.vari___30 = this.__s_52___28[this.__s_53___29];
                                if ((UnityEngine.Object)this.vari___30.m_prop != (UnityEngine.Object)null)
                                {
                                    this.dotIndex___31 = this.vari___30.m_prop.gameObject.name.LastIndexOf(".");
                                    this.vari___30.m_prop.gameObject.name = this.dotIndex___31 >= 0 ? this.vari___30.m_prop.gameObject.name.Remove(0, this.dotIndex___31 + 1) : this.vari___30.m_prop.gameObject.name;
                                    this.propVariations___27.Add(this.vari___30);
                                }
                            }
                            this.pi___26.m_variations = this.propVariations___27.ToArray();
                        }
                        this.ti___32 = ToolsModifierControl.toolController.m_editPrefabInfo as TreeInfo;
                        this.treeVariations___33 = new List<TreeInfo.Variation>();
                        if ((UnityEngine.Object)this.ti___32 != (UnityEngine.Object)null)
                        {
                            this.__s_54___34 = this.ti___32.m_variations;
                            for (this.__s_55___35 = 0; this.__s_55___35 < this.__s_54___34.Length; this.__s_55___35 = this.__s_55___35 + 1)
                            {
                                this.vari___36 = this.__s_54___34[this.__s_55___35];
                                if ((UnityEngine.Object)this.vari___36.m_tree != (UnityEngine.Object)null)
                                {
                                    this.dotIndex___37 = this.vari___36.m_tree.gameObject.name.LastIndexOf(".");
                                    this.vari___36.m_tree.gameObject.name = this.dotIndex___37 >= 0 ? this.vari___36.m_tree.gameObject.name.Remove(0, this.dotIndex___37 + 1) : this.vari___36.m_tree.gameObject.name;
                                    this.treeVariations___33.Add(this.vari___36);
                                }
                            }
                            this.ti___32.m_variations = this.treeVariations___33.ToArray();
                        }
                        // ISSUE: method pointer
                        SaveAssetPanelDetour.m_PackageSaveTask = Task.Create(_m__64).Run();
                        LoadSaveStatus.activeTask = (AsyncTaskBase)new AsyncTaskWrapper("Saving", new Task[1]
                        {
                        SaveAssetPanelDetour.m_PackageSaveTask
                        });
                        this.__PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Dispose()
            {
                this.__PC = -1;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }



            internal void _m__64()
            {
                try
                {
                    SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFC assetCAnonStoreyFc = new SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFC();
                    assetCAnonStoreyFc._f__ref__45 = this;
                    string savePathName = SaveAssetPanelDetour.GetSavePathName(this.saveName);
                    assetCAnonStoreyFc.p = new Package(this.saveName);
                    assetCAnonStoreyFc.p.packageMainAsset = this.saveName;
                    if (Singleton<SimulationManager>.instance.m_metaData.m_WorkshopPublishedFileId != PublishedFileId.invalid)
                        assetCAnonStoreyFc.p.packageName = Singleton<SimulationManager>.instance.m_metaData.m_WorkshopPublishedFileId.ToString();
                    if (!Singleton<SimulationManager>.instance.m_metaData.m_BuiltinAsset && PlatformService.active)
                        assetCAnonStoreyFc.p.packageAuthor = "steamid:" + PlatformService.user.userID.AsUInt64.ToString();
                    if ((UnityEngine.Object)this.trailerObject___7 != (UnityEngine.Object)null)
                    {
                        CustomAssetMetaData customAssetMetaData = new CustomAssetMetaData();
                        customAssetMetaData.mods = SaveAssetPanelDetour.EmbedModInfo(this._f__this);
                        customAssetMetaData.name = this.assetName + "_trailer";
                        customAssetMetaData.timeStamp = DateTime.Now;
                        // ISSUE: method pointer
                        Task<Package.Asset> task = ColossalFramework.Threading.ThreadHelper.dispatcher.Dispatch<Package.Asset>(new Func<Package.Asset>(
                            () => assetCAnonStoreyFc._m__65()));
                        task.Wait();
                        customAssetMetaData.assetRef = task.result;
                        customAssetMetaData.guid = Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier;
                        customAssetMetaData.type = CustomAssetMetaData.Type.Trailer;
                        string str = "VEHICLE_TITLE";
                        assetCAnonStoreyFc.p.AddAsset(this.saveName + "_trailer", (object)customAssetMetaData, UserAssetType.CustomAssetMetaData, false);
                        if (str != null)
                        {
                            ColossalFramework.Globalization.Locale locale1 = new ColossalFramework.Globalization.Locale();
                            ColossalFramework.Globalization.Locale locale2 = locale1;
                            ColossalFramework.Globalization.Locale.Key key = new ColossalFramework.Globalization.Locale.Key();
                            key.m_Identifier = str;
                            key.m_Key = this.assetName + "_trailer";
                            ColossalFramework.Globalization.Locale.Key k = key;
                            string v = this.assetName + "_trailer";
                            locale2.AddLocalizedString(k, v);
                            assetCAnonStoreyFc.p.AddAsset(this.assetName + "_trailer_locale", locale1, false);
                        }
                    }
                    if (ToolsModifierControl.toolController.m_editPrefabInfo is BuildingInfo)
                    {
                        //begin mod
                        ((BuildingInfo)ToolsModifierControl.toolController.m_editPrefabInfo).m_subBuildings = allSubBuildings.ToArray();
                        //end mod
                        int count = this.subBuildings___20.Count;
                        SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFD assetCAnonStoreyFd = new SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFD();
                        assetCAnonStoreyFd._f__ref__45 = this;
                        assetCAnonStoreyFd._f__ref__252 = assetCAnonStoreyFc;
                        for (assetCAnonStoreyFd.i = 0; assetCAnonStoreyFd.i < count; assetCAnonStoreyFd.i = assetCAnonStoreyFd.i + 1)
                        {
                            CustomAssetMetaData customAssetMetaData = new CustomAssetMetaData();
                            customAssetMetaData.mods = SaveAssetPanelDetour.EmbedModInfo(this._f__this);
                            customAssetMetaData.name = this.assetName + "_sub_building_" + assetCAnonStoreyFd.i.ToString();
                            customAssetMetaData.timeStamp = DateTime.Now;
                            // ISSUE: method pointer
                            Task<Package.Asset> task = ColossalFramework.Threading.ThreadHelper.dispatcher.Dispatch<Package.Asset>(new Func<Package.Asset>(
                                () => assetCAnonStoreyFd._m__66()));
                            task.Wait();
                            customAssetMetaData.assetRef = task.result;
                            if (customAssetMetaData.assetRef != (Package.Asset)null)
                            {
                                customAssetMetaData.type = CustomAssetMetaData.Type.SubBuilding;
                                assetCAnonStoreyFc.p.AddAsset(this.saveName + "_sub_building_" + assetCAnonStoreyFd.i.ToString(), (object)customAssetMetaData, UserAssetType.CustomAssetMetaData, false);
                            }
                        }
                    }
                    if (ToolsModifierControl.toolController.m_editPrefabInfo is PropInfo)
                    {
                        PropInfo propInfo = ToolsModifierControl.toolController.m_editPrefabInfo as PropInfo;
                        int num = 0;
                        SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFE assetCAnonStoreyFe = new SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFE();
                        assetCAnonStoreyFe._f__ref__45 = this;
                        assetCAnonStoreyFe._f__ref__252 = assetCAnonStoreyFc;
                        foreach (PropInfo.Variation mVariation in propInfo.m_variations)
                        {
                            assetCAnonStoreyFe.vari = mVariation;
                            CustomAssetMetaData customAssetMetaData = new CustomAssetMetaData();
                            customAssetMetaData.mods = SaveAssetPanelDetour.EmbedModInfo(this._f__this);
                            customAssetMetaData.name = this.assetName + "_variation_" + num.ToString();
                            customAssetMetaData.timeStamp = DateTime.Now;
                            // ISSUE: method pointer
                            Task<Package.Asset> task = ColossalFramework.Threading.ThreadHelper.dispatcher.Dispatch<Package.Asset>(new Func<Package.Asset>(
                                () => assetCAnonStoreyFe._m__67()));
                            task.Wait();
                            customAssetMetaData.assetRef = task.result;
                            customAssetMetaData.type = CustomAssetMetaData.Type.PropVariation;
                            assetCAnonStoreyFc.p.AddAsset(this.saveName + "_variation_" + num.ToString(), (object)customAssetMetaData, UserAssetType.CustomAssetMetaData, false);
                            ++num;
                        }
                    }
                    if (ToolsModifierControl.toolController.m_editPrefabInfo is TreeInfo)
                    {
                        TreeInfo treeInfo = ToolsModifierControl.toolController.m_editPrefabInfo as TreeInfo;
                        int num = 0;
                        SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFF assetCAnonStoreyFf = new SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFF();
                        assetCAnonStoreyFf._f__ref__45 = this;
                        assetCAnonStoreyFf._f__ref__252 = assetCAnonStoreyFc;
                        foreach (TreeInfo.Variation mVariation in treeInfo.m_variations)
                        {
                            assetCAnonStoreyFf.vari = mVariation;
                            CustomAssetMetaData customAssetMetaData = new CustomAssetMetaData();
                            customAssetMetaData.mods = SaveAssetPanelDetour.EmbedModInfo(this._f__this);
                            customAssetMetaData.name = this.assetName + "_variation_" + num.ToString();
                            customAssetMetaData.timeStamp = DateTime.Now;
                            // ISSUE: method pointer
                            Task<Package.Asset> task = ColossalFramework.Threading.ThreadHelper.dispatcher.Dispatch<Package.Asset>(new Func<Package.Asset>(()=>assetCAnonStoreyFf._m__68()));
                            task.Wait();
                            customAssetMetaData.assetRef = task.result;
                            customAssetMetaData.type = CustomAssetMetaData.Type.PropVariation;
                            assetCAnonStoreyFc.p.AddAsset(this.saveName + "_variation_" + num.ToString(), (object)customAssetMetaData, UserAssetType.CustomAssetMetaData, false);
                            ++num;
                        }
                    }
                    CustomAssetMetaData customAssetMetaData1 = new CustomAssetMetaData();
                    customAssetMetaData1.mods = SaveAssetPanelDetour.EmbedModInfo(this._f__this);
                    customAssetMetaData1.name = this.assetName;
                    customAssetMetaData1.timeStamp = DateTime.Now;
                    customAssetMetaData1.dlcMask = ToolsModifierControl.toolController.m_editPrefabInfo.m_dlcRequired;
                    BuildingInfo buildingInfo1 = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
                    if ((UnityEngine.Object)buildingInfo1 != (UnityEngine.Object)null && buildingInfo1.m_props != null)
                    {
                        foreach (BuildingInfo.Prop mProp in buildingInfo1.m_props)
                        {
                            SteamHelper.DLC_BitMask dlcBitMask = (SteamHelper.DLC_BitMask)0;
                            if ((UnityEngine.Object)mProp.m_prop != (UnityEngine.Object)null)
                                dlcBitMask = mProp.m_prop.m_dlcRequired;
                            else if ((UnityEngine.Object)mProp.m_tree != (UnityEngine.Object)null)
                                dlcBitMask = mProp.m_tree.m_dlcRequired;
                            customAssetMetaData1.dlcMask |= dlcBitMask;
                        }
                    }
                    if (this.snapShot___1 != null)
                    {
                        Image img = new Image(this.snapShot___1);
                        customAssetMetaData1.steamPreviewRef = assetCAnonStoreyFc.p.AddAsset(customAssetMetaData1.name + "_SteamPreview", img, false, Image.BufferFileFormat.PNG, false, false);
                        img.Resize(400, 224);
                        customAssetMetaData1.imageRef = assetCAnonStoreyFc.p.AddAsset(customAssetMetaData1.name + "_Snapshot", img, false, Image.BufferFileFormat.PNG, false, false);
                    }
                    // ISSUE: method pointer
                    Task<Package.Asset> task1 = ColossalFramework.Threading.ThreadHelper.dispatcher.Dispatch<Package.Asset>(new Func<Package.Asset>(() => assetCAnonStoreyFc._m__69()));
                    task1.Wait();
                    customAssetMetaData1.assetRef = task1.result;
                    ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipAtlas = ToolsModifierControl.toolController.m_editPrefabInfo.m_Atlas;
                    if (this.ignoreThumbnail)
                    {
                        ToolsModifierControl.toolController.m_editPrefabInfo.m_Atlas = this.tempThumbAtlas___2;
                        ToolsModifierControl.toolController.m_editPrefabInfo.m_InfoTooltipAtlas = this.tempInfoAtlas___3;
                    }
                    customAssetMetaData1.steamTags = ToolsModifierControl.toolController.m_editPrefabInfo.GetSteamTags();
                    customAssetMetaData1.guid = Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier;
                    string str1 = (string)null;
                    string str2 = (string)null;
                    string str3 = (string)null;
                    if (ToolsModifierControl.toolController.m_editPrefabInfo is BuildingInfo)
                    {
                        BuildingInfo buildingInfo2 = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
                        customAssetMetaData1.width = buildingInfo2.GetWidth();
                        customAssetMetaData1.length = buildingInfo2.GetLength();
                        if ((UnityEngine.Object)buildingInfo2.m_class != (UnityEngine.Object)null)
                        {
                            customAssetMetaData1.level = buildingInfo2.m_class.m_level;
                            customAssetMetaData1.service = buildingInfo2.GetService();
                            customAssetMetaData1.subService = buildingInfo2.GetSubService();
                        }
                        VehicleInfo.VehicleType vehicleType = VehicleInfo.VehicleType.None;
                        if (buildingInfo2.m_paths != null)
                        {
                            for (int index1 = 0; index1 < buildingInfo2.m_paths.Length; ++index1)
                            {
                                for (int index2 = 0; index2 < buildingInfo2.m_paths[index1].m_netInfo.m_lanes.Length; ++index2)
                                    vehicleType |= buildingInfo2.m_paths[index1].m_netInfo.m_lanes[index2].m_vehicleType;
                            }
                        }
                        customAssetMetaData1.vehicleType = vehicleType;
                        customAssetMetaData1.type = CustomAssetMetaData.Type.Building;
                        str1 = "BUILDING_TITLE";
                        str2 = "BUILDING_DESC";
                        str3 = "BUILDING_SHORT_DESC";
                    }
                    else if (ToolsModifierControl.toolController.m_editPrefabInfo is PropInfo)
                    {
                        customAssetMetaData1.type = CustomAssetMetaData.Type.Prop;
                        str1 = "PROPS_TITLE";
                        str2 = "PROPS_DESC";
                    }
                    else if (ToolsModifierControl.toolController.m_editPrefabInfo is TreeInfo)
                    {
                        customAssetMetaData1.type = CustomAssetMetaData.Type.Tree;
                        str1 = "TREE_TITLE";
                        str2 = "TREE_DESC";
                    }
                    else if (ToolsModifierControl.toolController.m_editPrefabInfo is VehicleInfo)
                    {
                        VehicleInfo vehicleInfo = (VehicleInfo)ToolsModifierControl.toolController.m_editPrefabInfo;
                        customAssetMetaData1.vehicleType = vehicleInfo.m_vehicleType;
                        customAssetMetaData1.type = CustomAssetMetaData.Type.Vehicle;
                        str1 = "VEHICLE_TITLE";
                    }
                    else
                        customAssetMetaData1.type = CustomAssetMetaData.Type.Unknown;
                    customAssetMetaData1.triangles = this.triangles___10;
                    customAssetMetaData1.textureHeight = this.textureHeight___11;
                    customAssetMetaData1.textureWidth = this.textureWidth___12;
                    customAssetMetaData1.lodTriangles = this.lodTriangles___15;
                    customAssetMetaData1.lodTextureHeight = this.lodTextureHeight___16;
                    customAssetMetaData1.lodTextureWidth = this.lodTextureWidth___17;
                    assetCAnonStoreyFc.p.AddAsset(this.saveName, (object)customAssetMetaData1, UserAssetType.CustomAssetMetaData, false);
                    if (str1 != null)
                    {
                        ColossalFramework.Globalization.Locale locale1 = new ColossalFramework.Globalization.Locale();
                        ColossalFramework.Globalization.Locale locale2 = locale1;
                        ColossalFramework.Globalization.Locale.Key key1 = new ColossalFramework.Globalization.Locale.Key();
                        key1.m_Identifier = str1;
                        key1.m_Key = this.assetName + "_Data";
                        ColossalFramework.Globalization.Locale.Key k1 = key1;
                        string v1 = this.assetName;
                        locale2.AddLocalizedString(k1, v1);
                        if (str2 != null)
                        {
                            ColossalFramework.Globalization.Locale locale3 = locale1;
                            ColossalFramework.Globalization.Locale.Key key2 = new ColossalFramework.Globalization.Locale.Key();
                            key2.m_Identifier = str2;
                            key2.m_Key = this.assetName + "_Data";
                            ColossalFramework.Globalization.Locale.Key k2 = key2;
                            string v2 = this.description;
                            locale3.AddLocalizedString(k2, v2);
                        }
                        if (str3 != null)
                        {
                            ColossalFramework.Globalization.Locale locale3 = locale1;
                            ColossalFramework.Globalization.Locale.Key key2 = new ColossalFramework.Globalization.Locale.Key();
                            key2.m_Identifier = str3;
                            key2.m_Key = this.assetName + "_Data";
                            ColossalFramework.Globalization.Locale.Key k2 = key2;
                            string v2 = this.description;
                            locale3.AddLocalizedString(k2, v2);
                        }
                        assetCAnonStoreyFc.p.AddAsset(this.assetName + "_Locale", locale1, false);
                    }
                    assetCAnonStoreyFc.p.Save(savePathName);
                    Dispatcher dispatcher = ColossalFramework.Threading.ThreadHelper.dispatcher;
                    if (SaveAssetPanelDetour.SaveAsset_c__Iterator2D._f__am__cache31 == null)
                    {
                        // ISSUE: method pointer
                        SaveAssetPanelDetour.SaveAsset_c__Iterator2D._f__am__cache31 = new System.Action(_m__6A);
                    }
                    System.Action action = SaveAssetPanelDetour.SaveAsset_c__Iterator2D._f__am__cache31;
                    dispatcher.Dispatch(action);
                }
                catch (Exception ex)
                {
                    string message;
                    if (DiskUtils.IsFull(ex))
                    {
                        message = "The disk is full." + System.Environment.NewLine + "Please free some space before saving.";
                        CODebugBase<LogChannel>.Error(LogChannel.Core, message + "\n" + (object)ex.GetType() + " " + ex.Message + " " + ex.StackTrace);
                    }
                    else
                    {
                        message = "An error occurred while packaging asset.";
                        CODebugBase<LogChannel>.Error(LogChannel.Core, message + "\n" + (object)ex.GetType() + " " + ex.Message + " " + ex.StackTrace);
                    }
                    UIView.ForwardException((Exception)new IOException(message, ex));
                }
                finally
                {
                    Thread.Sleep(1000);
                    m_IsSaving = false;
                    //begin mod
                    m_PackageSaveTask = null;
                    //end mod

                }
            }

            private static void _m__6A()
            {
                if (PlatformService.achievements["Decorator"].achieved)
                    return;
                PlatformService.achievements["Decorator"].Unlock();
            }

            private sealed class SaveAsset_c__AnonStoreyFC
            {
                internal Package p;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D _f__ref__45;

                internal Package.Asset _m__65()
                {
                    return this.p.AddAsset(this._f__ref__45.trailerObjectName___8, this._f__ref__45.trailerObject___7, false);
                }

                internal Package.Asset _m__69()
                {
                    return this.p.AddAsset(this._f__ref__45.assetName + "_Data", ToolsModifierControl.toolController.m_editPrefabInfo.gameObject, false);
                }
            }

            private sealed class SaveAsset_c__AnonStoreyFD
            {
                internal int i;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D _f__ref__45;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFC _f__ref__252;

                internal Package.Asset _m__66()
                {
                    return this._f__ref__252.p.AddAsset(this._f__ref__45.subBuildingNames___21[this.i], this._f__ref__45.subBuildingObjects___22[this.i], false);
                }
            }

            private sealed class SaveAsset_c__AnonStoreyFE
            {
                internal PropInfo.Variation vari;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D _f__ref__45;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFC _f__ref__252;

                internal Package.Asset _m__67()
                {
                    return this._f__ref__252.p.AddAsset(this.vari.m_prop.gameObject.name, this.vari.m_prop.gameObject, false);
                }
            }

            private sealed class SaveAsset_c__AnonStoreyFF
            {
                internal TreeInfo.Variation vari;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D _f__ref__45;
                internal SaveAssetPanelDetour.SaveAsset_c__Iterator2D.SaveAsset_c__AnonStoreyFC _f__ref__252;

                internal Package.Asset _m__68()
                {
                    return this._f__ref__252.p.AddAsset(this.vari.m_tree.gameObject.name, this.vari.m_tree.gameObject, false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class RefreshStaging_c__AnonStorey100
        {
            internal string type;
            internal SaveAssetPanel _f__this;

            internal void _m__62()
            {
                SaveAssetPanelDetour.ReloadThumbnail(this._f__this, this.type.Equals(string.Empty));
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.IO;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(AssetImporterAssetImport))]
    public class AssetImporterAssetImportDetour : AssetImporterAssetImport
    {
//        [RedirectMethod]
//        public void OnFileSelectionChanged(UIComponent comp, int index)
//        {
//            if (index >= 0 && index < this.m_FileList.items.Length)
//            {
//                this.m_ContinueButton.isEnabled = false;
//                if ((UnityEngine.Object)this.selectedTemplate != (UnityEngine.Object)null && this.hasPreview && index == 0)
//                    this.m_CurrentAsset.ImportDefault();
//                else
//                    this.m_CurrentAsset.Import(AssetImporterAssetImport.m_AssetsPath, this.m_FileList.items[index]);
//            }
//            else
//                this.Reset();
//        }
//
//        [RedirectMethod]
//        private void RefreshAssetList(string[] extensions)
//        {
//            this.component.Focus();
//            List<string> stringList = new List<string>();
//            if ((UnityEngine.Object)this.selectedTemplate != (UnityEngine.Object)null && this.hasPreview)
//                stringList.Add(Locale.Get("ASSETIMPORTER_TEMPLATE"));
//            DirectoryInfo directoryInfo = new DirectoryInfo(AssetImporterAssetImport.assetImportPath);
//            if (directoryInfo.Exists)
//            {
//                System.IO.FileInfo[] fileInfoArray = (System.IO.FileInfo[])null;
//                try
//                {
//                    fileInfoArray = directoryInfo.GetFiles();
//                }
//                catch (Exception ex)
//                {
//                    Debug.LogError((object)("An exception occured " + (object)ex));
//                    UIView.ForwardException(ex);
//                }
//                if (fileInfoArray != null)
//                {
//                    foreach (System.IO.FileInfo fileInfo in fileInfoArray)
//                    {
//                        if (!Path.GetFileNameWithoutExtension(fileInfo.Name).EndsWith(AssetImporterAssetImport.sLODModelSignature, StringComparison.OrdinalIgnoreCase))
//                        {
//                            for (int index = 0; index < extensions.Length; ++index)
//                            {
//                                if (string.Compare(Path.GetExtension(fileInfo.Name), extensions[index]) == 0)
//                                    stringList.Add(fileInfo.Name);
//                            }
//                        }
//                    }
//                }
//                this.m_FileList.items = stringList.ToArray();
//            }
//            if (!((UnityEngine.Object)this.selectedTemplate != (UnityEngine.Object)null) || !this.hasPreview)
//                return;
//            this.m_FileList.selectedIndex = 0;
//        }
    }
}
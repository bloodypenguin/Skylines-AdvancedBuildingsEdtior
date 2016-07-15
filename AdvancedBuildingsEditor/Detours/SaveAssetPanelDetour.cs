using System.IO;
using System.Runtime.InteropServices;
using AdvancedBuildingsEditor.Redirection;
using ColossalFramework.IO;
using ColossalFramework.Packaging;

namespace AdvancedBuildingsEditor.Detours
{
    [TargetType(typeof(SaveAssetPanel))]
    public class SaveAssetPanelDetour
    {
//TODO(earalov): implement later
//        [RedirectMethod]
//        private static string GetSavePathName(string saveName)
//        {
//            var escapedSaveName = PathEscaper.Escape(saveName);
//            var nameWithExtension = PathUtils.AddExtension(escapedSaveName, PackageManager.packageExtension);
//            if (LoadAssetPanelDetour.lastFilePath != null)
//            {
//                var lastFileName = Path.GetFileName(LoadAssetPanelDetour.lastFilePath);
//                var lastFileDirectory = Path.GetDirectoryName(LoadAssetPanelDetour.lastFilePath);
//                UnityEngine.Debug.Log($"lastFileName={lastFileName}, lastFileDirectory={lastFileDirectory}, nameWithExtension={nameWithExtension}, assetsPath={DataLocation.assetsPath}");
//                if (nameWithExtension.Equals(lastFileName) && lastFileDirectory != null && lastFileDirectory.Contains(DataLocation.assetsPath))
//                {
//                    return lastFileName;
//                }
//            }
//            return Path.Combine(DataLocation.assetsPath, nameWithExtension);
//        }
    }
}
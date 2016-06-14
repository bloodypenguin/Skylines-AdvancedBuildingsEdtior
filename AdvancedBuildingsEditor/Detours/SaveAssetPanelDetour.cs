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
        [RedirectMethod]
        private static string GetSavePathName(string saveName)
        {
            var escapedSaveName = PathEscaper.Escape(saveName);
            var nameWithExtension = PathUtils.AddExtension(escapedSaveName, PackageManager.packageExtension);
            if (LoadAssetPanelDetour.lastFilePath != null)
            {
                var lastFileName = Path.GetFileName(LoadAssetPanelDetour.lastFilePath);
                var lastFileDirectory = Path.GetDirectoryName(LoadAssetPanelDetour.lastFilePath);
                UnityEngine.Debug.Log($"lastFileName={lastFileName}, lastFileDirectory={lastFileDirectory}, nameWithExtension={nameWithExtension}, assetsPath={DataLocation.assetsPath}");
                if (nameWithExtension.Equals(lastFileName) && DataLocation.assetsPath.Equals(lastFileDirectory))
                {
                    return lastFileName;
                }
            }
            return Path.Combine(DataLocation.assetsPath, escapedSaveName);
        }
    }
}
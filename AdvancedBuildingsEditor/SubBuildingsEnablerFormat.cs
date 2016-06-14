using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public class SubBuildingsEnablerFormat
    {

        public static void InitializeBuildingsWithSubBuildings()
        {
            var subBuildingsDefParseErrors = new HashSet<string>();
            var checkedPaths = new List<string>();

            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetLoaded(i);

                if (prefab == null) continue;

                // search for SubBuildingsDefinition.xml
                var prefabName = prefab.name;

                string packageName;
                var subBuildingsDefPath = GetDefinitionPathForPrefab(prefabName, out packageName);
                if (subBuildingsDefPath == null)
                {
                    continue;
                }

                // skip files which were already parsed
                if (checkedPaths.Contains(subBuildingsDefPath)) continue;
                checkedPaths.Add(subBuildingsDefPath);

                InitializeDefinition(subBuildingsDefPath, subBuildingsDefParseErrors, packageName);
            }

            if (subBuildingsDefParseErrors.Count > 0)
            {
                var errorMessage = "Error while parsing sub-building definition file(s). Contact the author of the assets. \n"
                                   + "List of errors:\n";
                foreach (var error in subBuildingsDefParseErrors) errorMessage += error + '\n';

                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Sub-Buildings Enabler", errorMessage, true);
            }
        }

        public static void InitializeDefinition(string subBuildingsDefPath, HashSet<string> subBuildingsDefParseErrors, string packageName, BuildingInfo prefab = null)
        {
            SubBuildingsDefinition subBuildingsDef;
            try
            {
                subBuildingsDef = ReadDefinition(subBuildingsDefPath);
                if (subBuildingsDef == null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                subBuildingsDefParseErrors.Add(packageName + " - " + e.Message);
                return;
            }

            if (subBuildingsDef == null || subBuildingsDef.Buildings == null || subBuildingsDef.Buildings.Count == 0)
            {
                subBuildingsDefParseErrors.Add(packageName + " - subBuildingsDef is null or empty.");
                return;
            }

            foreach (var parentBuildingDef in subBuildingsDef.Buildings)
            {
                if (parentBuildingDef == null || parentBuildingDef.Name == null)
                {
                    subBuildingsDefParseErrors.Add(packageName + " - Building name missing.");
                    continue;
                }
                var parentBuildingPrefab = FindPrefab(parentBuildingDef.Name, packageName);

                if (parentBuildingPrefab == null)
                {
                    subBuildingsDefParseErrors.Add(packageName + " - Building with name " + parentBuildingDef.Name +
                                                   " not loaded.");
                    continue;
                }
                if (prefab != null &&  parentBuildingPrefab.name != $"{packageName}.{prefab.name}")
                {
                    UnityEngine.Debug.Log($"{parentBuildingPrefab.name}!={prefab.name} (package {packageName})");
                    continue;
                }

                if (parentBuildingDef.SubBuildings == null || parentBuildingDef.SubBuildings.Count == 0)
                {
                    subBuildingsDefParseErrors.Add(packageName + " - No sub buildings specified for " + parentBuildingDef.Name +
                                                   ".");
                    continue;
                }

                var subBuildings = new List<BuildingInfo.SubInfo>();

                foreach (var subBuildingDef in parentBuildingDef.SubBuildings)
                {
                    if (subBuildingDef == null || subBuildingDef.Name == null)
                    {
                        subBuildingsDefParseErrors.Add(parentBuildingDef.Name + " - Sub-building name missing.");
                        continue;
                    }

                    var subBuildingPrefab = FindPrefab(subBuildingDef.Name, packageName);

                    if (subBuildingPrefab == null)
                    {
                        subBuildingsDefParseErrors.Add(parentBuildingDef.Name + " - Sub-building with name " +
                                                       subBuildingDef.Name + " not loaded.");
                        continue;
                    }

                    var subBuilding = new BuildingInfo.SubInfo
                    {
                        m_buildingInfo = subBuildingPrefab,
                        m_position = new Vector3(subBuildingDef.PosX, subBuildingDef.PosY, subBuildingDef.PosZ),
                        m_angle = subBuildingDef.Angle,
                        m_fixedHeight = subBuildingDef.FixedHeight
                    };

                    subBuildings.Add(subBuilding);

                    // this is usually done in the InitializePrefab method
                    if (subBuildingDef.FixedHeight && !parentBuildingPrefab.m_fixedHeight)
                        parentBuildingPrefab.m_fixedHeight = true;
                }

                if (subBuildings.Count == 0)
                {
                    subBuildingsDefParseErrors.Add("No sub buildings specified for " + parentBuildingDef.Name + ".");
                    continue;
                }
                if (prefab == null)
                {
                    parentBuildingPrefab.m_subBuildings = subBuildings.ToArray();
                }
                else
                {
                    prefab.m_subBuildings = subBuildings.ToArray();
                }
            }
        }

        private static SubBuildingsDefinition ReadDefinition(string subBuildingsDefPath)
        {
            if (!File.Exists(subBuildingsDefPath))
            {
                return null;
            }
            SubBuildingsDefinition subBuildingsDef = null;
            var xmlSerializer = new XmlSerializer(typeof(SubBuildingsDefinition));
            using (StreamReader streamReader = new System.IO.StreamReader(subBuildingsDefPath))
            {
                subBuildingsDef = xmlSerializer.Deserialize(streamReader) as SubBuildingsDefinition;
            }
            return subBuildingsDef;
        }

        private static string GetDefinitionPathForPrefab(string prefabName, out string packageName)
        {
            var asset = PackageManager.FindAssetByName(prefabName);
            return DefinitionPathForAssetPackage(asset, out packageName);
        }

        private static string DefinitionPathForAssetPackage(Package.Asset asset, out string packageName)
        {
            var assetPath = Util.GetAssetPath(asset, out packageName);
            if (assetPath == null)
            {
                return null;
            }
            var subBuildingsDefPath = Path.Combine(Path.GetDirectoryName(assetPath), "SubBuildingsDefinition.xml");
            return subBuildingsDefPath;
        }

        private static BuildingInfo FindPrefab(string prefabName, string packageName)
        {
            var prefab = PrefabCollection<BuildingInfo>.FindLoaded(prefabName);
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(prefabName + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(PathEscaper.Escape(prefabName) + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(packageName + "." + prefabName + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(packageName + "." + PathEscaper.Escape(prefabName) + "_Data");

            return prefab;
        }

        public static void SetSubBuildings(BuildingInfo prefab, Package.Asset assetRef)
        {
            string packageName;
            var subBuildingsDefPath = DefinitionPathForAssetPackage(assetRef, out packageName);
            if (subBuildingsDefPath == null)
            {
                UnityEngine.Debug.Log("Path is empty");
                return;
            }
            InitializeDefinition(subBuildingsDefPath, new HashSet<string>(), packageName, prefab);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public class SubBuildingsDefinition
    {
        public List<Building> Buildings { get; set; }

        public SubBuildingsDefinition()
        {
            Buildings = new List<Building>();
        }

        public class Building
        {
            [XmlAttribute("name"), System.ComponentModel.DefaultValue(null)]
            public string Name { get; set; }

            public List<SubBuilding> SubBuildings { get; set; }

            public Building()
            {
                SubBuildings = new List<SubBuilding>();
            }
        }

        public class SubBuilding
        {
            [XmlAttribute("name"), System.ComponentModel.DefaultValue(null)]
            public string Name { get; set; }

            [XmlAttribute("pos-x"), System.ComponentModel.DefaultValue(0f)]
            public float PosX { get; set; }

            [XmlAttribute("pos-y"), System.ComponentModel.DefaultValue(0f)]
            public float PosY { get; set; }

            [XmlAttribute("pos-z"), System.ComponentModel.DefaultValue(0f)]
            public float PosZ { get; set; }

            [XmlAttribute("angle"), System.ComponentModel.DefaultValue(0f)]
            public float Angle { get; set; }

            [XmlAttribute("fixed-height"), System.ComponentModel.DefaultValue(true)]
            public bool FixedHeight { get; set; }

            public SubBuilding()
            {
                FixedHeight = true;
            }
        }

        public static void Save(SubBuildingsDefinition instance)
        {
            if (instance != null)
            {
                var configPath = "SubBuildingsDefinition.xml";
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SubBuildingsDefinition));
                XmlSerializerNamespaces noNamespaces = new XmlSerializerNamespaces();
                noNamespaces.Add("", "");
                try
                {
                    using (StreamWriter streamWriter = new System.IO.StreamWriter(configPath))
                    {
                        xmlSerializer.Serialize(streamWriter, instance, noNamespaces);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

    }
}
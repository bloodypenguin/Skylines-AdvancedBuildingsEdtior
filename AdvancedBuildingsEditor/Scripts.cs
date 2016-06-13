using System.Collections.Generic;
using AdvancedBuildingsEditor.Detours;
using UnityEngine;

namespace AdvancedBuildingsEditor
{
    public static class Scripts
    {

        public static void MakeAllSegmentsEditable(bool makeAllNetworksEditable = true)
        {
            var mgr = NetManager.instance;
            for (var i = 0; i < mgr.m_segments.m_size; i++)
            {
                if (mgr.m_segments.m_buffer[i].m_flags == NetSegment.Flags.None)
                {
                    continue;
                }
                Debug.Log($"Segment {i} -  Type: {mgr.m_segments.m_buffer[i].Info.name}, Length: {mgr.m_segments.m_buffer[i].m_averageLength}");
                if (makeAllNetworksEditable)
                {
                    mgr.m_segments.m_buffer[i].m_flags &= ~NetSegment.Flags.Untouchable;
                }
            }
        }

        public static void BulldozePedestrianConnections()
        {
            var mgr = NetManager.instance;
            for (ushort i = 0; i < mgr.m_segments.m_size; i++)
            {
                var netSegment = mgr.m_segments.m_buffer[i];
                if (netSegment.m_flags == NetSegment.Flags.None) continue;

                var name = netSegment.Info.name;
                if (name.Contains("Pedestrian Connection") /*|| name == "Cargo Connection" ||
                    name == "Ship Dock" || name == "Ship Dockway" || name == "Bus Station Stop" || name == "Bus Station Way"*/)
                {
                    mgr.ReleaseSegment(i, false);
                }
            }
        }

        public static void ClearProps(bool specialPoints = false)
        {
            var instance = PropManager.instance;
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var isSpecialPoint = SpecialPoints.IsSpecialPoint(propInstance.Info);
                if (isSpecialPoint == specialPoints)
                {
                    instance.ReleaseProp(index1);
                }
            }
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
        }

        public static void RecalculateSpecialPoints()
        {
            PropManager instance = PropManager.instance;
            BuildingDecorationDetour.SpecialPoints = new Dictionary<ushort, SpecialPointType>();
            for (ushort index1 = 0; index1 < ushort.MaxValue; ++index1)
            {
                var propInstance = instance.m_props.m_buffer[index1];
                if (propInstance.m_flags == 0)
                {
                    continue;
                }
                var propInfo = propInstance.Info;
                if (SpecialPoints.IsSpecialPoint(propInfo))
                {
                    BuildingDecorationDetour.SpecialPoints.Add(index1, SpecialPoints.GetSpecialPointType(propInfo));
                }
            }
        }
    }
}
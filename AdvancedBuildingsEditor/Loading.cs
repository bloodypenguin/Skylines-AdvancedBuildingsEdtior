﻿using System;
using System.Collections;
using ColossalFramework;

namespace AdvancedBuildingsEditor
{
    public class Loading
    {
        public static void QueueLoadingAction(Action action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(action));
        }

        public static void QueueLoadingAction(IEnumerator action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(action);
        }

        private static IEnumerator ActionWrapper(Action a)
        {
            a.Invoke();
            yield break;
        }
    }
}
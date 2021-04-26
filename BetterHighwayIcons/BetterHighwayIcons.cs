using System;
using ColossalFramework.UI;
using UnityEngine;
using System.Reflection;
using ColossalFramework.Plugins;
using System.Collections.Generic;
using ICities;
using System.Threading;
using ColossalFramework;

namespace BetterHighwayIcons
{
    // Content Manager Information for mod
    public class ModInfo : IUserMod
    {
        public string Name => "Better Highway Icons";
        public string Description => "Makes the default highway icons consistent with the Mass Transit highways.";
    }

    // Loading Hook
    public class BetterHighwayIconsLoader : LoadingExtensionBase
    {
        private static BetterHighwayIcons instance;

        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                // Don't run the mod inside editors
                if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                    return;

                BetterHighwayIcons.isGameLoaded = true;

                if (instance != null)
                    GameObject.DestroyImmediate(instance.gameObject);

                instance = new GameObject("BetterHighwayIcons").AddComponent<BetterHighwayIcons>();
            }
            catch (Exception ex)
            {
                if (instance != null)
                    GameObject.Destroy(instance.gameObject);
                Debug.Log(ex);
            }
        }

        public class BetterHighwayIcons : MonoBehaviour
        {
            public static bool isGameLoaded = false;

            public void Start()
            {
                try
                {
                    // Find highway prefab info
                    NetInfo highway3LanePrefab = PrefabCollection<NetInfo>.FindLoaded("Highway");
                    NetInfo highway3LaneBarrierPrefab = PrefabCollection<NetInfo>.FindLoaded("Highway Barrier");
                    NetInfo highway4LaneBarrierPrefab = PrefabCollection<NetInfo>.FindLoaded("Four Lane Highway Barrier");
                    NetInfo highwayRampPrefab = PrefabCollection<NetInfo>.FindLoaded("HighwayRamp");

                    // Create and replace texture atlas
                    highway3LanePrefab.m_Atlas = ImageLoader.GenerateAtlas("Highway3Lane", ImageLoader.Highway3Lane);
                    highway3LaneBarrierPrefab.m_Atlas = ImageLoader.GenerateAtlas("Highway3LaneBarrier", ImageLoader.Highway3LaneBarrier);
                    highway4LaneBarrierPrefab.m_Atlas = ImageLoader.GenerateAtlas("Highway4LaneBarrier", ImageLoader.Highway4LaneBarrier);
                    highwayRampPrefab.m_Atlas = ImageLoader.GenerateAtlas("HighwayRamp", ImageLoader.HighwayRamp);

                    // Set thumbnails
                    highway3LanePrefab.m_Thumbnail = "Highway3Lane";
                    highway3LaneBarrierPrefab.m_Thumbnail = "Highway3LaneBarrier";
                    highway4LaneBarrierPrefab.m_Thumbnail = "Highway4LaneBarrier";
                    highwayRampPrefab.m_Thumbnail = "HighwayRamp";

                    // Refresh the UI (Is this a Hack?)
                    UnlockManager unlockManager = GameObject.FindObjectOfType<UnlockManager>();
                    unlockManager.MilestonesUpdated();
                }
                catch (Exception ex)
                {
                    Debug.Log("BetterHighwayIcons - Start() - " + ex.Message);
                }
            }
        }

    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_RhythmMarker.Component;

namespace ChroMapper_RhythmMarker
{
    [Plugin("Rhythm Marker")]
    public class Plugin
    {
        public static RhythmMarkerController rhythmMarkerController;
        [Init]
        private void Init()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            Debug.Log("Rhythm Marker Plugin has loaded!");
        }
        [Exit]
        private void Exit()
        {
            Debug.Log("Rhythm Marker:Application has closed!");
        }
        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex != 3) // Mapper scene
                return;
            if (rhythmMarkerController != null && rhythmMarkerController.isActiveAndEnabled)
                return;
            rhythmMarkerController = new GameObject("RhythmMarker").AddComponent<RhythmMarkerController>();
        }
    }
}

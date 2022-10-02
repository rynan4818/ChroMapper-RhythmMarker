using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_RhythmMarker.Component;
using ChroMapper_RhythmMarker.UserInterface;

namespace ChroMapper_RhythmMarker
{
    [Plugin("Rhythm Marker")]
    public class Plugin
    {
        public static RhythmMarkerController rhythmMarkerController;
        public static UI _ui;
        [Init]
        private void Init()
        {
            Debug.Log("Rhythm Marker Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            _ui = new UI();
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
            var mapEditorUI = Object.FindObjectOfType<MapEditorUI>();
            _ui.AddMenu(mapEditorUI);
        }
    }
}

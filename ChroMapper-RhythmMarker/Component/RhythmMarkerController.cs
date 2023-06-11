using Beatmap.Base;
using ChroMapper_RhythmMarker.Configuration;
using ChroMapper_RhythmMarker.UserInterface;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ChroMapper_RhythmMarker.Component
{
    public class RhythmMark
    {
        public float Time;
        public Color Color;
        public TextMeshProUGUI Text;
        public RhythmMark(float time, Color color, TextMeshProUGUI text)
        {
            Color = color;
            Time = time;
            Text = text;
        }
        public JSONNode ConvertToJson()
        {
            JSONNode node = new JSONObject();
            node["_time"] = Math.Round(Time, Settings.Instance.TimeValueDecimalPrecision);
            node["_color"] = Color;
            return node;
        }
    }
    public class RhythmMarkerController : MonoBehaviour
    {
        public List<RhythmMark> rhythmMarks = new List<RhythmMark>();
        private Transform rhythmMarksParent;
        private BaseDifficulty map;
        private AudioTimeSyncController atsc;
        public InputAction _shiftAction;
        public InputAction _MarkJumpAction;
        public InputAction _scrollAction;
        public bool _shiftKeyEnable;
        public bool _markJumpEnable;
        public List<Type> queuedToDisable = new List<Type>();
        public List<Type> queuedToEnable = new List<Type>();
        public CustomStandaloneInputModule customStandaloneInputModule;
        private readonly Type[] editActionMapsDisabled =
        {
            typeof(CMInput.ITimelineActions)
        };
        private readonly Type[] actionMapsEnabledWhenNodeEditing =
        {
            typeof(CMInput.ICameraActions), typeof(CMInput.IBeatmapObjectsActions),
            typeof(CMInput.ISavingActions), typeof(CMInput.ITimelineActions)
        };
        private Type[] actionMapsDisabled => typeof(CMInput).GetNestedTypes()
            .Where(x => x.IsInterface && !actionMapsEnabledWhenNodeEditing.Contains(x)).ToArray();
        public void Start()
        {
            var measureLinesCanvas = GameObject.Find("Moveable Grid/Measure Lines/Measure Lines Canvas");
            rhythmMarksParent = measureLinesCanvas.transform;
            map = BeatSaberSongContainer.Instance.Map;
            atsc = FindObjectOfType<AudioTimeSyncController>();
            EditorScaleController.EditorScaleChangedEvent += OnEditorScaleChange;
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
            this._shiftAction = new InputAction("ShiftKey", binding: Options.Instance.shiftBinding);
            this._shiftAction.started += OnShiftKey;
            this._shiftAction.performed += OnShiftKey;
            this._shiftAction.canceled += OnShiftKey;
            this._shiftAction.Enable();
            this._MarkJumpAction = new InputAction("MarkJump", binding: Options.Instance.markJumpBinding);
            this._MarkJumpAction.started += OnMarkJump;
            this._MarkJumpAction.performed += OnMarkJump;
            this._MarkJumpAction.canceled += OnMarkJump;
            this._MarkJumpAction.Enable();
            this._scrollAction = new InputAction("MouseScroll");
            this._scrollAction.AddBinding("<Mouse>/scroll/y");
            this._scrollAction.started += OnScrollActive;
            this._scrollAction.performed += OnScrollActive;
            this._scrollAction.canceled += OnScrollActive;
            if (map.CustomData.HasKey("_rhythmMarks"))
            {
                var dataNode = map.CustomData["_rhythmMarks"];
                foreach (JSONNode node in dataNode)
                {
                    var time = RetrieveRequiredNode(node, "_time").AsFloat;
                    Color color;
                    if (node.HasKey("_color"))
                        color = RetrieveRequiredNode(node, "_color");
                    else
                        color = Color.cyan;
                    var text = CreateRhythmMark(time, color);
                    rhythmMarks.Add(new RhythmMark(time, color, text));
                }
            }
        }
        private void Update()
        {
            QueuedActionMaps();
        }
        public void OnDestroy()
        {
            this._shiftAction.Dispose();
            this._MarkJumpAction.Dispose();
            this._scrollAction.Dispose();
            EditorScaleController.EditorScaleChangedEvent -= OnEditorScaleChange;
        }
        public void OnMarkJump(InputAction.CallbackContext context)
        {
            this._markJumpEnable = context.ReadValueAsButton();
            if (this._markJumpEnable)
            {
                DisableAction(editActionMapsDisabled);
                this._scrollAction.Enable();
            }
            else
            {
                this._scrollAction.Disable();
                EnableAction(editActionMapsDisabled);
            }
        }
        public void OnShiftKey(InputAction.CallbackContext context)
        {
            this._shiftKeyEnable = context.ReadValueAsButton();
        }
        public void SaveCustomData()
        {
            if (!map.MainNode.HasKey("_customData") || map.MainNode["_customData"] is null ||
                !map.MainNode["_customData"].Children.Any())
            {
                map.MainNode["_customData"] = map.CustomData;
            }
            var rhythmMarksJSON = new JSONArray();
            foreach (var rhythmMark in rhythmMarks)
                rhythmMarksJSON.Add(rhythmMark.ConvertToJson());
            if (rhythmMarks.Any())
                map.MainNode["_customData"]["_rhythmMarks"] = CleanupArray(rhythmMarksJSON);
            else
                map.MainNode["_customData"].Remove("_rhythmMarks");
        }
        public void SetViewRhythmMark(bool view)
        {
            foreach (var rhythmMark in rhythmMarks)
                rhythmMark.Text.gameObject.SetActive(view);
            if (view)
                this._MarkJumpAction.Enable();
            else
                this._MarkJumpAction.Disable();
        }
        public void GenerateRhythmMark(float startBeat, float endBeat, float interval, Color color)
        {
            if (startBeat < 0)
                startBeat = 0;
            if (interval < 0)
                interval = 1;
            float lastBeat = atsc.GetBeatFromSeconds(atsc.SongAudioSource.clip.length);
            if (endBeat < 0 || endBeat <= startBeat)
                endBeat = lastBeat;
            var currentBeat = startBeat;
            while(currentBeat <= lastBeat && currentBeat <= endBeat)
            {
                var checkMark = rhythmMarks.Find(n => n.Time == currentBeat);
                if (checkMark == null)
                {
                    var text = CreateRhythmMark(currentBeat, color);
                    rhythmMarks.Add(new RhythmMark(currentBeat, color, text));
                }
                else
                    TextSetRhythmMark(checkMark.Text, color);
                currentBeat += interval;
            }
            SaveCustomData();
            Debug.Log($"RhythmMarks Size={rhythmMarks.Count()}");
        }
        public void DeleteRhythmMark(float startBeat, float endBeat)
        {
            if (startBeat < 0)
                startBeat = 0;
            if (endBeat < 0 || endBeat <= startBeat)
                endBeat = -1f;
            for (var i = rhythmMarks.Count - 1; i >= 0; i--)
            {
                RhythmMark rhythmMark = rhythmMarks[i];
                if (rhythmMark.Time >= startBeat && (endBeat == -1f || rhythmMark.Time <= endBeat))
                {
                    Destroy(rhythmMark.Text.gameObject);
                    rhythmMarks.Remove(rhythmMark);
                }
            }
            SaveCustomData();
        }
        public void OnEditorScaleChange(float newScale)
        {
            foreach (RhythmMark rhythmMark in rhythmMarks)
                SetRhythmMarkPos(rhythmMark.Text.rectTransform, rhythmMark.Time);
        }
        private void SetRhythmMarkPos(RectTransform rect, float time)
        {
            rect.anchoredPosition3D = new Vector3(-10.3f, time * EditorScaleController.EditorScale, 0);
        }

        private TextMeshProUGUI CreateRhythmMark(float time, Color color)
        {
            var obj = new GameObject("RhythmMark", typeof(TextMeshProUGUI));
            var rect = (RectTransform)obj.transform;
            rect.SetParent(rhythmMarksParent);
            SetRhythmMarkPos(rect, time);
            rect.sizeDelta = Vector2.one;
            rect.localRotation = Quaternion.identity;

            var text = obj.GetComponent<TextMeshProUGUI>();
            text.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            text.alignment = TextAlignmentOptions.Left;
            text.fontSize = 0.4f;
            text.enableWordWrapping = false;
            text.raycastTarget = false;
            TextSetRhythmMark(text, color);
            return text;
        }
        private void TextSetRhythmMark(TextMeshProUGUI text, Color color)
        {
            text.text = $"<mark=#{ColorUtility.ToHtmlStringRGB(color)}50><voffset=0.06> <indent=9.5> </voffset> <color=#00000000>.</color>";
            text.ForceMeshUpdate();
        }
        protected JSONNode RetrieveRequiredNode(JSONNode node, string key)
        {
            if (!node.HasKey(key)) throw new ArgumentException($"{GetType().Name} missing required node \"{key}\".");
            return node[key];
        }
        private JSONArray CleanupArray(JSONArray original)
        {
            var array = original.Clone().AsArray;
            foreach (JSONNode node in original)
                if (node is null || node["_time"].IsNull || float.IsNaN(node["_time"]))
                    array.Remove(node);
            return array;
        }
        public void DisableAction(Type[] actionMaps)
        {
            foreach (Type actionMap in actionMaps)
            {
                queuedToEnable.Remove(actionMap);
                if (!queuedToDisable.Contains(actionMap))
                    queuedToDisable.Add(actionMap);
            }
        }
        public void EnableAction(Type[] actionMaps)
        {
            foreach (Type actionMap in actionMaps)
            {
                queuedToDisable.Remove(actionMap);
                if (!queuedToEnable.Contains(actionMap))
                    queuedToEnable.Add(actionMap);
            }
        }
        public void QueuedActionMaps()
        {
            if (queuedToDisable.Any())
                CMInputCallbackInstaller.DisableActionMaps(typeof(UI), queuedToDisable.ToArray());
            queuedToDisable.Clear();
            if (queuedToEnable.Any())
                CMInputCallbackInstaller.ClearDisabledActionMaps(typeof(UI), queuedToEnable.ToArray());
            queuedToEnable.Clear();
        }
        public void OnScrollActive(InputAction.CallbackContext context)
        {
            if (atsc.IsPlaying) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            var value = context.ReadValue<float>();
            if (context.performed)
            {
                if (value > 0)
                {
                    var rhythmMarksOrder = rhythmMarks.OrderBy(x => x.Time);
                    foreach (var mark in rhythmMarksOrder)
                    {
                        if (mark.Time > atsc.CurrentSongBpmTime)
                        {
                            atsc.MoveToSongBpmTime(mark.Time);
                            break;
                        }
                    }
                }
                else
                {
                    var rhythmMarksOrder = rhythmMarks.OrderByDescending(x => x.Time);
                    foreach (var mark in rhythmMarksOrder)
                    {
                        if (mark.Time < atsc.CurrentSongBpmTime)
                        {
                            atsc.MoveToSongBpmTime(mark.Time);
                            break;
                        }
                    }
                }
            }
        }
        public void InputDisable()
        {
            DisableAction(actionMapsDisabled);
        }
        public void InputEnable()
        {
            EnableAction(actionMapsDisabled);
        }
    }
}

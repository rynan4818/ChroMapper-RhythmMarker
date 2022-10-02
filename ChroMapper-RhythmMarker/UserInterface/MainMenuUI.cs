using UnityEngine;
using UnityEngine.UI;
using ChroMapper_RhythmMarker.Configuration;
using ChroMapper_RhythmMarker.Component;
using TMPro;
using System.Text.RegularExpressions;

namespace ChroMapper_RhythmMarker.UserInterface
{
    public class MainMenuUI
    {
        public GameObject _mainMenu;
        public float _startBeat = 0;
        public float _endBeat = -1;
        public float _interval = 4;
        public Color _color = Color.cyan;
        public bool _view = true;
        public TextMeshProUGUI _colorLabel;
        public UIButton _colorButton;

        public void AnchoredPosSave()
        {
            Options.Instance.mainMenuUIAnchoredPosX = _mainMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.mainMenuUIAnchoredPosY = _mainMenu.GetComponent<RectTransform>().anchoredPosition.y;
            Options.Instance.SettingSave();
        }
        public void AddMenu(MapEditorUI mapEditorUI)
        {
            var parent = mapEditorUI.MainUIGroup[5];
            _mainMenu = new GameObject("RhythmMarker Main Menu");
            _mainMenu.transform.parent = parent.transform;
            _mainMenu.AddComponent<DragWindowController>();
            _mainMenu.GetComponent<DragWindowController>().canvas = parent.GetComponent<Canvas>();
            _mainMenu.GetComponent<DragWindowController>().OnDragWindow += AnchoredPosSave;

            //Main Menu
            UI.AttachTransform(_mainMenu, 170, 220, 1, 1, Options.Instance.mainMenuUIAnchoredPosX, Options.Instance.mainMenuUIAnchoredPosY, 1, 1);

            Image imageMain = _mainMenu.AddComponent<Image>();
            imageMain.sprite = PersistentUI.Instance.Sprites.Background;
            imageMain.type = Image.Type.Sliced;
            imageMain.color = new Color(0.24f, 0.24f, 0.24f);

            UI.MoveTransform(UI.AddLabel(_mainMenu.transform, "Rhythm Marker", "Rhythm Marker").Item1, 150, 24, 0.5f, 1, 0, -18);

            var textInput = UI.AddTextInput(_mainMenu.transform, "Start Beat", "Start Beat", this._startBeat.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    this._startBeat = res;
                if (value == "")
                    this._startBeat = 0;
            });
            UI.MoveTransform(textInput.Item1, 60, 16, 0, 1, 45, -40.3f);
            UI.MoveTransform(textInput.Item3.transform, 50, 20, 0.1f, 1, 86.3f, -37.6f);

            textInput = UI.AddTextInput(_mainMenu.transform, "End Beat", "End Beat", "", (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    this._endBeat = res;
                if (value == "")
                    this._endBeat = -1;
            });
            UI.MoveTransform(textInput.Item1, 60, 16, 0, 1, 45, -64.2f);
            UI.MoveTransform(textInput.Item3.transform, 50, 20, 0.1f, 1, 86.3f, -62.8f);

            textInput = UI.AddTextInput(_mainMenu.transform, "Interval", "Interval", this._interval.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    this._interval = res;
                if (value == "")
                    this._interval = 1;
            });
            UI.MoveTransform(textInput.Item1, 60, 16, 0, 1, 45, -91.1f);
            UI.MoveTransform(textInput.Item3.transform, 50, 20, 0.1f, 1, 86.3f, -88.7f);

            var label = UI.AddLabel(_mainMenu.transform, "Color", "Color", 12);
            label.Item2.alignment = TMPro.TextAlignmentOptions.Right;
            label.Item2.color = this._color;
            UI.MoveTransform(label.Item1, 60, 16, 0, 1, 45, -116.5f);
            this._colorLabel = label.Item2;

            this._colorButton = UI.AddButton(_mainMenu.transform, "Pick", "Pick", () =>
            {
                PersistentUI.Instance.ShowColorInputBox("Mapper", "bookmark.update.color", HandleNewBookmarkColor, this._colorLabel.color);
            });
            UI.MoveTransform(this._colorButton.transform, 50, 18, 0, 1, 102.9f, -113.8f);
            this._colorButton.Text.color = this._color;

            var button = UI.AddButton(_mainMenu.transform, "Create Marker", "Create Marker", () =>
            {
                Plugin.rhythmMarkerController.GenerateRhythmMark(this._startBeat, this._endBeat, this._interval, this._color);
            });
            UI.MoveTransform(button.transform, 70, 25, 0, 1, 44.5f, -141.4f);

            button = UI.AddButton(_mainMenu.transform, "Delete Marker", "Delete Marker", () =>
            {
                Plugin.rhythmMarkerController.DeleteRhythmMark(this._startBeat, this._endBeat);
            });
            UI.MoveTransform(button.transform, 70, 25, 0, 1, 128.7f, -141.4f);

            var checkBox = UI.AddCheckbox(_mainMenu.transform, "Marker View", "Marker View", this._view, (check) =>
            {
                this._view = check;
                Plugin.rhythmMarkerController.SetViewRhythmMark(check);
            });
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 28.1f, -179.3f);
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 111, -176.2f);

            button = UI.AddButton(_mainMenu.transform, "Close", "Close", () =>
            {
                _mainMenu.SetActive(false);
            });
            UI.MoveTransform(button.transform, 70, 25, 0, 1, 128.7f, -172.4f);

            var regexKey = new Regex(@"<\w+>/");
            label = UI.AddLabel(_mainMenu.transform, "Jump Marker", $"{regexKey.Replace(Options.Instance.markJumpBinding, "").ToUpper()} + mouse scroll to move between marks", 10);
            label.Item2.alignment = TMPro.TextAlignmentOptions.Center;
            UI.MoveTransform(label.Item1, 160, 16, 0, 1, 86.4f, -206.3f);

            _mainMenu.SetActive(false);
            UI._extensionBtn.Click = () =>
            {
                _mainMenu.SetActive(!_mainMenu.activeSelf);
            };
        }
        private void HandleNewBookmarkColor(Color? res)
        {
            if (res == null) return;
            this._color = (Color)res;
            this._colorLabel.color = (Color)res;
            this._colorButton.Text.color = (Color)res;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChroMapper_RhythmMarker.Component
{
    public class DragWindowController : MonoBehaviour, IDragHandler
    {
        public Canvas canvas { set; get; }
        public event Action OnDragWindow;

        public void OnDrag(PointerEventData eventData)
        {
            if (Plugin.rhythmMarkerController._shiftKeyEnable)
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvas.scaleFactor;
                OnDragWindow?.Invoke();
            }
        }
    }
}

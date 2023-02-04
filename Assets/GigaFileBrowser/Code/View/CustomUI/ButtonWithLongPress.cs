using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ButtonWithLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        float requiredClickTime = 0.15f;

        [SerializeField]
        float requiredHoldTime = 0.5f;

        float pointerDownTime;

        bool pointerDown;
        float pointerDownTimer;
        bool isLongPressFired;

        public event Action OnClick = delegate { };
        public event Action OnLongPress = delegate { };

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownTime = Time.time;

            pointerDownTimer = 0;
            pointerDown = true;

            isLongPressFired = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDown = false;
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if ((Time.time - pointerDownTime) < requiredClickTime)
                {
                    OnClick();
                }
            }

            if (pointerDown && !isLongPressFired)
            {
                pointerDownTimer += Time.deltaTime;

                if (pointerDownTimer >= requiredHoldTime)
                {
                    OnLongPress();

                    isLongPressFired = true;
                }
            }
        }
    }
}
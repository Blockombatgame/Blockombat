using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

//Handles joystick controls
public class AnalogInput : MonoBehaviour, IEndDragHandler, IDragHandler
{
    public static AnalogInput Instance;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public float originOffset;
    public bool resetPos;
    public bool canReadCancler;

    public UnityAction OnInputDown;
    public UnityAction<bool> OnInputUp;

    public Action<Vector2> dragEvent;

    bool dragStarted;

    private void Awake()
    {
        Instance = this;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!dragStarted)
        {
            OnInputDown?.Invoke();
        }
        dragStarted = true;
        Vector3 dis = (new Vector3(eventData.position.x, eventData.position.y) - transform.parent.position);
        transform.localPosition = Vector3.ClampMagnitude(dis, 100);
        direction = dis.normalized;
        originOffset = transform.localPosition.magnitude;

        dragEvent?.Invoke(direction);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragStarted = false;
        if (canReadCancler)
        {
            //OnInputUp?.Invoke(!dragCancler.cancleGrag);
            //dragCancler.Activate(true);
        }
        else
        {
            OnInputUp?.Invoke(true);
        }

        if (!resetPos) return;
        direction = Vector3.zero;
        transform.localPosition = Vector3.zero;
        //OnInputDown?.Invoke();

        dragEvent?.Invoke(direction);

    }
}

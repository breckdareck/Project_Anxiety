using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _windowTransform;
    [SerializeField] private Canvas _canvas;

    private void Awake()
    {
        if (_windowTransform == null)
        {
            _windowTransform = transform.parent.GetComponent<RectTransform>();
        }

        if (_canvas == null)
        {
            Transform tempTransform = transform.parent;
            while (tempTransform != null)
            {
                _canvas = tempTransform.GetComponent<Canvas>();
                if(_canvas != null) break;
                tempTransform = tempTransform.parent;

            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        _windowTransform.anchoredPosition += data.delta / _canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _windowTransform.SetAsLastSibling();
    }

}
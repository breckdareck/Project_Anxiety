using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AbilityDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [FormerlySerializedAs("_skillTransform")] [SerializeField] private RectTransform _abilityTransform;
    [SerializeField] private Canvas _canvas;
    public Image IconImage;
    private Transform _originalParent;
    public AbilityTemplate _template;
    private CanvasGroup _group;

    private void Awake()
    {
        if (_template == null)
        {
            _template = GetComponentInParent<AbilityTemplate>();
        }

        if (_group == null)
        {
            _group = GetComponent<CanvasGroup>();
        }
        
        if (_abilityTransform == null)
        {
            _abilityTransform = transform.parent.GetComponent<RectTransform>();
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
    public void OnPointerDown(PointerEventData eventData)
    {
        if(_template.abilityDataAttached is not ActiveAbilityData) return;
        _originalParent = transform.parent;
        _group.blocksRaycasts = false;
        transform.SetParent(_canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_template.abilityDataAttached is not ActiveAbilityData) return;
        _abilityTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_template.abilityDataAttached is not ActiveAbilityData) return;
        transform.SetParent(_originalParent);
        _group.blocksRaycasts = true;
        _abilityTransform.anchoredPosition = Vector2.zero;
    }
}

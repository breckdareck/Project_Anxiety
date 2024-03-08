using System;
using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HotBarSlot : MonoBehaviour, IDropHandler
{
    private AbilityDragHandler _abilityDragHandler;
    private Transform _originalParent;
    private RectTransform hotBarSlotRectTransform;
    private RectTransform imageTransform;
    
    [FormerlySerializedAs("activeAbilityDataData")] [FormerlySerializedAs("activeAbility")] public ActiveAbilityData activeAbilityData;
    public Image abilityIconImage;
    [ShowInInspector] public string HotBarKey => GetComponentInChildren<TMP_Text>().text;
    
    private Canvas _canvas;

    private void Awake()
    {
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

        if (hotBarSlotRectTransform == null)
        {
            hotBarSlotRectTransform = transform.GetComponent<RectTransform>();
        }

        if (imageTransform == null)
        {
            imageTransform = abilityIconImage.transform.GetComponent<RectTransform>();
        }
    }
    
    // TODO - Use this for the image so we can remove it from the HotBar or move it to another
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if(activeAbility == null) return;
    //     
    //     _originalParent = transform;
    //     abilityIconImage.transform.SetParent(_canvas.transform);
    // }
    //
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if(activeAbility == null) return;
    //
    //     abilityIconImage.transform.SetParent(_originalParent);
    //     imageTransform.anchoredPosition = Vector2.zero;
    //     imageTransform.SetSiblingIndex(0);
    // }
    //
    // public void OnDrag(PointerEventData eventData)
    // {
    //     if(activeAbility == null) return;
    //     
    //     imageTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    // }
    
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("On Drop");
        if (eventData.pointerPress.TryGetComponent(out AbilityDragHandler handler))
        {
            _abilityDragHandler = handler;
            activeAbilityData = (ActiveAbilityData)_abilityDragHandler._template.abilityDataAttached;
            abilityIconImage.sprite = _abilityDragHandler.IconImage.sprite;
            abilityIconImage.color =  Color.white;
        }
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerUpHandler
{
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        eventData.selectedObject = null;
        GameManager.Instance.player.stateHandler.isJumping = false;
    }
}

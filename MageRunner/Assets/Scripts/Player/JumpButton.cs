using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerUpHandler
{
    public PlayerController player;

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        eventData.selectedObject = null;
        player.isJumping = false;
    }
}

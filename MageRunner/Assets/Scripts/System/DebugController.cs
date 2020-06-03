using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public void TogglePlayerMovement()
    {    
        GameManager.Instance.player.moving = !GameManager.Instance.player.moving;
    }
}

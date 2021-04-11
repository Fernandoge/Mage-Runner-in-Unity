using System;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public void ToggleLevelMovement()
    {
        if (GameManager.Instance.level.isMoving)
            GameManager.Instance.level.DisableMovement();
        else
            GameManager.Instance.level.EnableMovement();
    }

    public void ResetLevel() => GameManager.Instance.level.ResetLevel();
}

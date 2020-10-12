using UnityEngine;

public class DebugController : MonoBehaviour
{
    public void ToggleLevelMovement()
    {    
        GameManager.Instance.level.isMoving = !GameManager.Instance.level.isMoving;
    }

    public void ResetLevel()
    {
        GameManager.Instance.level.ResetLevel();
    }
}

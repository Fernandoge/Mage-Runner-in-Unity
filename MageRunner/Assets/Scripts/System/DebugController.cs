using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public LevelController level;

    public void EnableLevel()
    {
        level.enabled = level.isActiveAndEnabled ? false : true;
    }
}

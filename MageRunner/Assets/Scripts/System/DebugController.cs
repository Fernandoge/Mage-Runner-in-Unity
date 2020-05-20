using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public void EnableLevel() => GameManager.Instance.level.enabled = !GameManager.Instance.level.isActiveAndEnabled;
}

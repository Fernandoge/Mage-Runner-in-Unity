using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TimeFrame
{
    public GameObject movingGO;
    public GameObject staticGO;
}

public class PortalController : MonoBehaviour
{
    public TimeFrame currentTimeFrame;
    public TimeFrame nextTimeFrame;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            ChangeTimeFrame();
    }

    private void ChangeTimeFrame()
    {
        currentTimeFrame.movingGO.SetActive(false);
        currentTimeFrame.staticGO.SetActive(false);
        GameManager.Instance.level.movingObjects.position = Vector3.zero;
        nextTimeFrame.movingGO.SetActive(true);
        nextTimeFrame.staticGO.SetActive(true);
    }
}

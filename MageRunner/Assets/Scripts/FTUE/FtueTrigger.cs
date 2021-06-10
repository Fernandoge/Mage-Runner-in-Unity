using System.Collections;
using System.Collections.Generic;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;

public class FtueTrigger : MonoBehaviour
{
    [SerializeField] private int _ftueNumber;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")) 
            return;
        
        FtueManager.Instance.StartFtue(_ftueNumber);
    }
}

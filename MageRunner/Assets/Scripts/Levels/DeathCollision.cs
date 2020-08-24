using System;
using UnityEngine;

public class DeathCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.level.ResetLevel();
        }
    }
}

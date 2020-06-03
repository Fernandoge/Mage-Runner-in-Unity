using System;
using UnityEngine;

public class DeathCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.level.ResetLevel();
        }
    }
}

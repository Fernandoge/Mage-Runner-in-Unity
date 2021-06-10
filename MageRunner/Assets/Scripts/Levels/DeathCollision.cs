﻿using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class DeathCollision : MonoBehaviour
    {
        private void Start() => transform.parent = Camera.main.transform;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GameManager.Instance.level.ResetLevel();
            }
        }
    }
}

﻿using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Obstacles
{
    public class ObstacleController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
                GameManager.Instance.level.ResetLevel();
        }
    }
}

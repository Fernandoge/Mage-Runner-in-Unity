using System.Collections.Generic;
using MageRunner.Enemies;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Scenes
{
    public class LevelSceneController : MonoBehaviour
    {
        public FlyingEnemyAreasColumn[] flyingEnemyAreas;

        [SerializeField] private GameObject _level;

        private void Start()
        {
            GameManager.Instance.levelScene = this;
            Instantiate(_level);
        }
    }
}

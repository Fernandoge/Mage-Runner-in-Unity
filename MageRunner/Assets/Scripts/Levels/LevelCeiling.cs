using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class LevelCeiling : MonoBehaviour
    {
        private void Start() => transform.parent = GameManager.Instance.mainCamera.transform;
    }
}

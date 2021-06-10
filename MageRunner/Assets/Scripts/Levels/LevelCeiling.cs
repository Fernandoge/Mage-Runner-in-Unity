using UnityEngine;

namespace MageRunner.Levels
{
    public class LevelCeiling : MonoBehaviour
    {
        private void Start() => transform.parent = Camera.main.transform;
    }
}

using UnityEngine;

namespace MageRunner.Scenes
{
    public class LevelSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject _level;

        private void Start()
        {
            Instantiate(_level);
        }
    }
}

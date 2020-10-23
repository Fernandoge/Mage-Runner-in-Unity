using UnityEngine;
using UnityEngine.UI;

public class LevelSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _level;

    private void Start()
    {
        Instantiate(_level);
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [System.NonSerialized] public PlayerController player;
    [System.NonSerialized] public LevelController level;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

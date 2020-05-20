using UnityEngine;

public class DeathCollision : MonoBehaviour
{
    public LevelController currentLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            currentLevel.ResetLevel();
    }
}

using UnityEngine;

public class MovingBG : MonoBehaviour
{
    public float speed;
    
    private void Update()
    {
        if (GameManager.Instance.player.isMoving == false)
            return;

        float totalSpeed = GameManager.Instance.level.movingSpeed * speed;
        transform.Translate(Vector2.left * totalSpeed * Time.deltaTime);
    }
}
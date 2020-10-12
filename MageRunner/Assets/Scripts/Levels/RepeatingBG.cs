using UnityEngine;

public class RepeatingBG : MonoBehaviour
{
    public float speed;
    public float startX;
    public float endX;

    private void Update()
    {
        if (GameManager.Instance.level.isMoving == false)
            return;

        float totalSpeed = GameManager.Instance.level.movingSpeed * speed;
        transform.Translate(Vector2.left * totalSpeed * Time.deltaTime);
        
        if (transform.localPosition.x <= endX)
        {
            transform.localPosition = new Vector2(startX, transform.localPosition.y);
        }
    }
}

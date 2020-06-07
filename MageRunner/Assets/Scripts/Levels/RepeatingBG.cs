using UnityEngine;

public class RepeatingBG : MonoBehaviour
{
    public float speed;
    public float startX;
    public float endX;

    private void Update()
    {
        if (GameManager.Instance.player.moving == false)
            return;
        
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        
        if (transform.localPosition.x <= endX)
        {
            transform.localPosition = new Vector2(startX, transform.localPosition.y);
        }
    }
}

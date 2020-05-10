using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Rigidbody2D rigBody;
    public float speed;
    public LayerMask noDestroyLayers;

    [System.NonSerialized]
    public bool preparingReflect;

    private void Update()
    {
        if (preparingReflect)
        {
            transform.Translate(Vector2.right * Time.deltaTime / 4);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & noDestroyLayers) == 0)
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ReflectAura"))
        {
            rigBody.simulated = false;
            preparingReflect = true;
            transform.SetParent(collision.transform.parent);
            PlayerController player = collision.gameObject.transform.parent.GetComponent<PlayerController>();
            player.reflectedAttacks.Add(this);
        }
        
        if (collision.CompareTag("Player"))
        {
            //TODO: Change this when player health is implemented
            LevelController currentLevel = FindObjectOfType<LevelController>();
            currentLevel.ResetLevel();
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

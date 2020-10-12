using UnityEngine;

public class PortalController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            GameManager.Instance.level.ChangeTimeFrame();
    }
}

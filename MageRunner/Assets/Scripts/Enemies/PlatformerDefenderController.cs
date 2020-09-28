using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerDefenderController : PlatformerEnemyController
{

    [Header("Platformer Defender Fields")]
    [SerializeField] private float _dashSpeed;
        
    private bool _isDashing;
    
    public override void PlatformInteract()
    {
        if (_isDashing)
            return;

        StartCoroutine(DashToPlayer());
    }

    private IEnumerator DashToPlayer()
    {
        _isDashing = true;
        LookPlayer();
        Vector2 playerCollisionPosition = new Vector2(GameManager.Instance.player.transform.position.x, transform.position.y);
        yield return new WaitForSeconds(0.5f);
        while (Vector2.Distance(transform.position, playerCollisionPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerCollisionPosition, _dashSpeed * Time.deltaTime);
            yield return null;    
        }
    }
}

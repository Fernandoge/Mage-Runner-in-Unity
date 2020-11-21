using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStats : Stats
{
    [SerializeField] private Attack _requiredAttackToHit; 
    
    public override void HandleAttack(int attackDamage, EElement attackElement)
    {
        if (attackElement == _requiredAttackToHit.element && attackDamage == _requiredAttackToHit.damage)
        {
            base.HandleAttack(attackDamage, attackElement);
        }
        else
        {
            GameManager.Instance.player.companionChatBubble.StartChat(new Message("HEY! Don't break the tutorial", 2f, false, false, false));
            GameManager.Instance.player.companionChatBubble.StartChat(new Message("Try using the " + _requiredAttackToHit.name + " spell", 2f, true, true, false));
        }
    }

    public override void DestroyGameObject()
    {
        base.DestroyGameObject();
        GameManager.Instance.ftueController.NextFtueStep();
    }
}

using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MageRunner.Player
{
    public class JumpButton : MonoBehaviour, IPointerUpHandler
    {
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            eventData.selectedObject = null;
            GameManager.Instance.player.jumpStillPressed = false;
            GameManager.Instance.player.stateHandler.isJumping = false;
        }
    }
}

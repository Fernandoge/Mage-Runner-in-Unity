using MageRunner.Managers.FtueManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class FtueTrigger : MonoBehaviour
    {
        [SerializeField] private int _ftueNumber;
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player")) 
                return;
        
            FtueManager.Instance.StartFtue(_ftueNumber);
        }
    }
}

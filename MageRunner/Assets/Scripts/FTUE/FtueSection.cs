using GestureRecognizer;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public abstract class FtueSection : MonoBehaviour
    {
        [SerializeField] protected int FtueID;

        protected bool _ftueStarted;
    
        protected void Start() => FtueManager.Instance.ftueDict.Add(FtueID, this);

        protected void OnTriggerEnter2D(Collider2D col)
        {
            if (_ftueStarted || col.gameObject.layer != LayerMask.NameToLayer("Player")) 
                return;

            FtueManager.Instance.currentFtueNumber = FtueID;
            _ftueStarted = true;
            FirstStep();
        }

        protected void UnlockBasicSpell(GesturePattern ftuePattern)
        {
            GameManager.Instance.basicSpellsUnlocked++;
            GameManager.Instance.recognizer.patterns.Add(ftuePattern);
        }

        protected abstract void FirstStep();

        public virtual void StepAfterDestroyed() {}
    }
}

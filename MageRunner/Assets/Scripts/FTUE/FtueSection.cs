using MageRunner.Managers.FtueManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public abstract class FtueSection : MonoBehaviour
    {
        [SerializeField] private int FtueID;
    
        protected void Start() => FtueManager.Instance.ftueDict.Add(FtueID, this);

        public abstract void FirstStep();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageRunner.Managers.PopUpsManager
{
    public class PopUpsManager : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> activePopUps = new List<GameObject>();
    
        public void ActivatePopUp(GameObject popUp)
        {
            popUp.SetActive(true);
            Time.timeScale = 0;
            if (activePopUps.Contains(popUp) == false)
                activePopUps.Add(popUp);
        }

        public void DeactivatePopUp(GameObject popUp)
        {
            popUp.SetActive(false);
            if (activePopUps.Contains(popUp))
                activePopUps.Remove(popUp);
        }

        public void CloseActivePopUps()
        {
            foreach (GameObject popUp in activePopUps)
            {
                popUp.SetActive(false);
            }
        
            activePopUps.Clear();
            Time.timeScale = 1;
        }
    }
}

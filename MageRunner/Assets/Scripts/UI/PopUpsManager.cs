using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpsManager : MonoBehaviour
{
    [NonSerialized] public List<GameObject> activePopUps = new List<GameObject>();
    
    public void ActivatePopUp(GameObject popUp)
    {
        popUp.SetActive(true);
        activePopUps.Add(popUp);
        Time.timeScale = 0;
    }

    public void DeactivatePopUp(GameObject popUp)
    {
        popUp.SetActive(false);
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

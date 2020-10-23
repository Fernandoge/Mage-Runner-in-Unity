using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpsManager : MonoBehaviour
{
    [SerializeField] private GameObject _popUp;
    
    public void ActivatePopUp()
    {
        _popUp.SetActive(! _popUp.activeSelf);
        Time.timeScale = _popUp.activeSelf ? 0 : 1;
    }
}

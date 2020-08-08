using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScaleWidthCamera : MonoBehaviour
{
    [SerializeField] private int targetWidth;
    [SerializeField] private float pixelsToUnits;
    [SerializeField] private RectTransform bottomPanel;

    private int _height;

    private void Awake()
    {
        _height = Mathf.RoundToInt(targetWidth / (float) Screen.width * Screen.height);
        print (Screen.width + "   " + Screen.height + "    " + _height);

        if (_height > 1080)
            Camera.main.orthographicSize = _height / pixelsToUnits / 2;
    }

    public void ScaleLevelCamera(Transform level)
    {
        switch (_height)
        {
            // 4:3
            case 1440: 
                level.localScale = new Vector3(1, 1.2f, 1);
                level.position = new Vector3(0, 1.02f, 0);
                transform.position = new Vector3(0, 0, -10);
                bottomPanel.anchoredPosition = new Vector3(0, 139.5f, 0);
                bottomPanel.sizeDelta = new Vector2(bottomPanel.sizeDelta.x, 278);
                break;
            
            // 5:4
            case 1536:  
                level.localScale = new Vector3(1, 1.25f, 1);
                level.position = new Vector3(0, 1.3f, 0);
                transform.position = new Vector3(0, 0, -10);
                bottomPanel.anchoredPosition = new Vector3(0, 148.4f, 0);
                bottomPanel.sizeDelta = new Vector2(bottomPanel.sizeDelta.x, 295.7f);
                break;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using MageRunner.FTUE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.Managers.FtueManager
{
    public class FtueManager : MonoBehaviour
    {
        public static FtueManager Instance;

        public readonly Dictionary<int, FtueSection> ftueDict = new Dictionary<int, FtueSection>();
        public GameObject ftuePanel;
        
        [SerializeField] private GameObject _bottomChatPanel;
        [SerializeField] private TMP_Text _bottomChatText;

        [NonSerialized] public int currentFtueNumber;
        
        private Transform _originalButtonParent;
        private Image _ftuePanelImage;
        private Image _originalFtuePanelImage;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        
        private void Start()
        {
            _ftuePanelImage = ftuePanel.GetComponent<Image>();
            _originalFtuePanelImage = _ftuePanelImage;
        }

        public void CurrentFtueDestroyedStep() => ftueDict[currentFtueNumber].StepAfterDestroyed();
    }
}
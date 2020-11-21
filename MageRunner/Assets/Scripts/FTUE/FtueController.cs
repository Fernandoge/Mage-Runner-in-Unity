using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FtueController : MonoBehaviour
{
    [NonSerialized] public int ftueStepIndex;
    
    [SerializeField] private GameObject _ftuePanel;
    [SerializeField] private GameObject _bottomChatPanel;
    [SerializeField] private TMP_Text _bottomChatText;
    [SerializeField] private FtueElement[] _ftueElements;

    private Transform _originalButtonParent;
    private Image _ftuePanelImage;
    private Image _originalFtuePanelImage;

    private void Start()
    {
        GameManager.Instance.ftueController = this;
        _ftuePanelImage = _ftuePanel.GetComponent<Image>();
        _originalFtuePanelImage = _ftuePanelImage;
    }

    // This is called in the event OnTextShowed of the FTUE Bottom chat TextAnimator too
    public void NextFtueStep()
    {
        if (ftueStepIndex > 0)
        {
            FtueElement previousFtueStep = _ftueElements[ftueStepIndex - 1];
            if (previousFtueStep.elements.Length > 0)
            {
                Elements previousFtueStepElements = previousFtueStep.elements[0];
                previousFtueStepElements.hand.SetActive(false);
                previousFtueStepElements.button.onClick.RemoveListener(NextFtueStep);
                previousFtueStepElements.button.transform.SetParent(_originalButtonParent);
                if (previousFtueStepElements.deactivatedObject != null)
                    previousFtueStepElements.deactivatedObject.SetActive(true);
            }
        }

        if (ftueStepIndex < _ftueElements.Length)
        {
            Debug.Log("playing Ftue index " + ftueStepIndex);
            FtueElement currentFtueStep = _ftueElements[ftueStepIndex];
            
            if (currentFtueStep.elements.Length > 0)
            {
                Elements currentFtueStepElements = currentFtueStep.elements[0];
                currentFtueStepElements.hand.SetActive(true);
                currentFtueStepElements.button.onClick.AddListener(NextFtueStep);
                _originalButtonParent = currentFtueStepElements.button.transform.parent;
                currentFtueStepElements.button.transform.SetParent(_ftuePanel.transform);
            }
            else if (currentFtueStep.text.Length > 0)
            {
                FtueText currentFtueStepText = currentFtueStep.text[0];
                _bottomChatText.text = currentFtueStepText.stepText;
                _bottomChatPanel.SetActive(true);
                
                if (currentFtueStepText.makeFtuePanelTransparent)
                {
                    Color tempColor = _ftuePanelImage.color;
                    tempColor.a = 0;
                    _ftuePanelImage.color = tempColor;
                }

                if (currentFtueStepText.returnFtuePanelAlpha)
                    _ftuePanelImage = _originalFtuePanelImage;
            }
            else if (currentFtueStep.dialogue.Length > 0)
            {
                DialogueController currentFtueStepDialogue = currentFtueStep.dialogue[0];
                currentFtueStepDialogue.StartDialogue();
                _ftuePanel.SetActive(false);
                _bottomChatPanel.SetActive(false);
            }
            else
                Debug.LogError("Some FTUE Element isn't correctly configured!");

            Time.timeScale = currentFtueStep.ftueStepTimeScale;
            
            if (currentFtueStep.objectToDeactivate != null)
                currentFtueStep.objectToDeactivate.SetActive(false);
            
            if (currentFtueStep.objectToActivate != null)
                currentFtueStep.objectToActivate.SetActive(true);
        }
        else
        {
            _ftuePanel.SetActive(false);
            _bottomChatPanel.SetActive(false);
        }

        ftueStepIndex += 1;
    }
}

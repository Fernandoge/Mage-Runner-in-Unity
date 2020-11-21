using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct FtueElement
{
     public FtueText[] text;
     public Elements[] elements;
     public DialogueController[] dialogue;
     public float ftueStepTimeScale;
     public GameObject objectToDeactivate;
     public GameObject objectToActivate;
}

[System.Serializable]
public struct FtueText
{
     [TextArea] public string stepText;
     public bool makeFtuePanelTransparent;
     public bool returnFtuePanelAlpha;
}

[System.Serializable]
public struct Elements
{
     public Button button;
     public GameObject hand;
     public GameObject deactivatedObject;
     // public DialogueController dialogue;
     // public float secondsToWait;
     // public bool enableChatBubble;
}

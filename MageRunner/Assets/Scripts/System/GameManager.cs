using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ElementsData elementsData;
    public Dictionary<(EElement, EElement), int> elementsMultipliersDict = new Dictionary<(EElement, EElement), int>();

    [NonSerialized] public PlayerController player;
    [NonSerialized] public LevelController level;
    [NonSerialized] public DialogueController dialoguePlaying;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadElementsMultipliers();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadElementsMultipliers()
    {
        foreach (EElement originElement in Enum.GetValues(typeof(EElement)))
        {
            ElementsData.ElementMultipliers elementMultipliers = new ElementsData.ElementMultipliers();
            switch (originElement)
            {
                case EElement.Neutral: elementMultipliers = elementsData.neutralMultipliers; break;
                case EElement.Fire: elementMultipliers = elementsData.fireMultipliers; break;
                case EElement.Ice: elementMultipliers = elementsData.iceMultipliers; break;
                case EElement.Water: elementMultipliers = elementsData.waterMultipliers; break;
                case EElement.Wind: elementMultipliers = elementsData.windMultipliers; break;
                case EElement.Earth: elementMultipliers = elementsData.earthMultipliers; break;
                case EElement.Lightning: elementMultipliers = elementsData.lightningMultipliers; break;
                case EElement.Nature: elementMultipliers = elementsData.natureMultipliers; break;
                default: Debug.LogError("Element " + originElement + " isn't being loaded in elements multipliers dictionary"); break;
            }
            
            foreach (EElement targetElement in Enum.GetValues(typeof(EElement)))
            {
                int finalMultiplier = 0;
                switch (targetElement)
                {
                    case EElement.Neutral: finalMultiplier = elementMultipliers.neutral; break;
                    case EElement.Fire: finalMultiplier = elementMultipliers.fire; break;
                    case EElement.Ice: finalMultiplier = elementMultipliers.ice; break;
                    case EElement.Water: finalMultiplier = elementMultipliers.water; break;
                    case EElement.Wind: finalMultiplier = elementMultipliers.wind; break;
                    case EElement.Earth: finalMultiplier = elementMultipliers.earth; break;
                    case EElement.Lightning: finalMultiplier = elementMultipliers.lightning; break;
                    case EElement.Nature: finalMultiplier = elementMultipliers.nature; break;
                    default: Debug.LogError("Element " + targetElement + " isn't being loaded in elements multipliers dictionary"); break;
                }
                elementsMultipliersDict.Add((originElement, targetElement), finalMultiplier);
            }
        }
    }
}

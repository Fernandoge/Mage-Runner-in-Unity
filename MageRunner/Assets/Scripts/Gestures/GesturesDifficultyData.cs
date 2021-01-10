using GestureRecognizer;
using UnityEngine;

[CreateAssetMenu(menuName = "GesturesDifficultyData")]
public class GesturesDifficultyData : ScriptableObject
{
    public GesturePattern[] easy;
    public GesturePattern[] medium;
}

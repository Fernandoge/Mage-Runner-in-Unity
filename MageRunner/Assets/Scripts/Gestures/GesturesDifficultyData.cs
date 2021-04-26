using GestureRecognizer;
using UnityEngine;

namespace MageRunner.Gestures
{
    [CreateAssetMenu(menuName = "GesturesDifficultyData")]
    public class GesturesDifficultyData : ScriptableObject
    {
        public GesturePattern[] easy;
        public GesturePattern[] medium;
    }
}

using System.Collections.Generic;
using GestureRecognizer;
using UnityEngine;

namespace MageRunner.Gestures
{
    [CreateAssetMenu(menuName = "GesturesDifficultyData")]
    public class GesturesDifficultyData : ScriptableObject
    {
        public List<GesturePattern> easy = new List<GesturePattern>();
        public List<GesturePattern> medium = new List<GesturePattern>();
    }
}

using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ElementsData")]
public class ElementsData : ScriptableObject
{
    [System.Serializable]
    public struct ElementMultipliers
    {
        public int neutral;
        public int fire;
        public int ice;
        public int water;
        public int wind;
        public int earth;
        public int lightning;
        public int nature;
    }

    public ElementMultipliers neutralMultipliers;
    public ElementMultipliers fireMultipliers;
    public ElementMultipliers iceMultipliers;
    public ElementMultipliers waterMultipliers;
    public ElementMultipliers windMultipliers;
    public ElementMultipliers earthMultipliers;
    public ElementMultipliers lightningMultipliers;
    public ElementMultipliers natureMultipliers;
}

using System;
using UnityEngine;

namespace MageRunner.Enemies
{
    [Serializable]
    public class FlyingEnemyArea
    {
        public Transform area;
        public bool isPositionOccupied;
    }
}

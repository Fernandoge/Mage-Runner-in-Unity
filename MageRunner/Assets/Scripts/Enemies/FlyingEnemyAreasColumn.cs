using System;

namespace MageRunner.Enemies
{
    [Serializable]
    public class FlyingEnemyAreasColumn
    {
        public FlyingEnemyArea[] enemyAreas;
        public bool isColumnOccupied;
    }
}
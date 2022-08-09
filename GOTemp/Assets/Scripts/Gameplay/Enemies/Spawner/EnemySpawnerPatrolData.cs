using System;
using System.Collections.Generic;

[Serializable]
public class EnemySpawnerPatrolData : EnemySpawnerBaseData
{
    public List<Field> MoveTargets = new List<Field>();
}

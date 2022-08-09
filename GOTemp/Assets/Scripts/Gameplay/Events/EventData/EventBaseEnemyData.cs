public class EventBaseEnemyData : EventBaseData
{
    public Enemy Enemy 
    {
        get;
        private set;
    }

    public EventBaseEnemyData(Field field, Enemy enemy) : base(field)
    {
        this.Enemy = enemy;
    }
}

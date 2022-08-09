using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Field : FieldBase
{
    public ActionObject ActionObject
    { 
        get;
        set;
    }

    public List<Enemy> Enemies = new List<Enemy>();
    public List<EventBaseData> Events = new List<EventBaseData>();

    //Serialize to editor
    [field: SerializeField] 
    public bool IsStartField 
    {
        get; 
        set;
    } = false;
    [field: SerializeField]
    public bool IsWonField
    {
        get;
        set;
    } = false;

    [field: SerializeField]
    public Transform StandTransform 
    {
        get;
        private set; 
    } = null;

    public bool HasEnemy()
    {
        if (Enemies.Count <= 0) return false;

        return true;
    }

    public bool HasLookEvents()
    {
        if(Events.Count <=0)
        {
            return false;
        }

        foreach (var singleEvent in Events)
        {
            EventEnemyLookData enemyLook = singleEvent as EventEnemyLookData;
            EventObjectLookData objectLook = singleEvent as EventObjectLookData;

            if(enemyLook != null || objectLook != null)
            {
                return true;
            }
        }

        return false;
    }

    public EventBaseEnemyData GetEnemyEvent(Enemy enemy)
    {
        foreach (var singleEvent in Events)
        {
            EventBaseEnemyData lookEvent = singleEvent as EventBaseEnemyData;
            if (lookEvent != null && lookEvent.Enemy == enemy)
            {
                return lookEvent;
            }
        }

        return null;
    }

    public EventBaseObjectData GetObjectEvent(ActionObject actionObject)
    {
        foreach (var singleEvent in Events)
        {
            EventBaseObjectData lookEvent = singleEvent as EventBaseObjectData;
            if (lookEvent != null && lookEvent.ActionObject == actionObject)
            {
                return lookEvent;
            }
        }

        return null;
    }

    public List<Field> GetNeighborsFields()
    {
        List<Field> neighborsFierlds = new List<Field>();

        foreach (var path in paths)
        {
            neighborsFierlds.Add(path.Value.GetNextField(this) as Field);
        }

        return neighborsFierlds;
    }
}

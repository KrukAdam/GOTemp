using UnityEngine;

public class TurnBasedSystem 
{
    public int TurnNumber 
    {
        get; 
        private set;
    } = 0;

    public void NextTurn()
    {
        TurnNumber++;
        Debug.Log($"Turn: {TurnNumber}");
    }
}

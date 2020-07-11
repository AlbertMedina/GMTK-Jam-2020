using UnityEngine;

public class Match
{
    public int bulletRules;
    public int movementRules;
    public int winConRules;
    
    public Match()
    {
        
    }

    public void Generate()
    {
        bulletRules = Random.Range(0, 3);
        movementRules = Random.Range(0, 3);
        winConRules = Random.Range(0, 3);
    }
}

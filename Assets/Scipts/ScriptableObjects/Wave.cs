using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/New Wave")]
public class Wave : ScriptableObject
{
    
    public int enemiesInWave;

    public int GetEnemiesInWave()
    {
        return enemiesInWave;
    }
}

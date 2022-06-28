using System.Collections;
using System.Collections.Generic;

public class WaveInformation 
{
    public readonly int NumberofEnemies;
    public readonly int LotsofEnemies;
    public readonly float TimeBetweenEnemiesSpawning;
    public WaveInformation(int numberOfEnemies, int lotsOfEnemies, float timeBetweenEnemiesSpawning)
    {
        this.NumberofEnemies = numberOfEnemies;
        LotsofEnemies = lotsOfEnemies;
        TimeBetweenEnemiesSpawning = timeBetweenEnemiesSpawning;
    }
}
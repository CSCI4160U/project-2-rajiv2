using System.Collections.Generic;

[System.Serializable]
public class GameStatisticsData
{
    // stores Enemy names
    public List<string> bossesDefeatedNames = new List<string>();

    // store Scene names where Enemy was defeated
    public List<string> bossesDefeatedScenes = new List<string>();

    // stores names of puzzles that the player has completed
    public List<string> puzzlesCompleted = new List<string>();
}

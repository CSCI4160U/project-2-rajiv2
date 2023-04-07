using System.Collections.Generic;

[System.Serializable]
public class BossDefeatsData
{
    // stores Enemy names
    public List<string> bossesDefeatedNames = new List<string>();

    // store Scene names where Enemy was defeated
    public List<string> bossesDefeatedScenes = new List<string>();
}

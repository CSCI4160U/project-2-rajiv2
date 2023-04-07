public class Level
{
    public int scoreRequired;
    public string levelName;
    public int attackBoost;
    public int defenseBoost;

    public Level(string levelName, int scoreRequired, int attackBoost, int defenseBoost)
    {
        this.scoreRequired = scoreRequired;
        this.levelName = levelName;
        this.attackBoost = attackBoost;
        this.defenseBoost = defenseBoost;
    }
}

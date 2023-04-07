using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    private Level[] levels = new Level[] { 
        new Level("LVL 1",0,0,0),
        new Level("LVL 2",1000,2,2),
        new Level("LVL 3",5000,4,2),
        new Level("LVL 4",10000,6,3),
        new Level("LVL 5",15000,8,4),
        new Level("LVL 6",20000,10,5),
        new Level("LVL 7",25000,12,6),
        new Level("LVL 8",30000,14,7),
        new Level("LVL 9",40000,16,8),
        new Level("LVL 10",50000,20,10),
    };

    [SerializeField] private Player player = null;
    private string previousLvl = string.Empty;

    private void Update()
    {
        if (!player.isDead)
        {
            UpdateLevel();
        }
    }

    public void UpdateLevel()
    {
        GameObject levelSlider = GameObject.Find("LevelSlider");
        GameObject currentLvlInHUD = GameObject.Find("CurrentLevel");
        GameObject nextLvlInHUD = GameObject.Find("NextLevel");

        if (player != null)
        {
            int nextLevelIndex = GetNextLevelIndex(player.playerScore);

            Level currentLvl = levels[nextLevelIndex - 1];
            Level nextLvl = levels[nextLevelIndex];

            // Updating Slider
            if(levelSlider != null)
            {
                levelSlider.GetComponent<Slider>().maxValue = nextLvl.scoreRequired - currentLvl.scoreRequired;
                levelSlider.GetComponent<Slider>().value = player.playerScore - currentLvl.scoreRequired;
            }

            
            if(currentLvlInHUD != null && nextLvlInHUD != null)
            {
                // if new level acquired
                if(previousLvl != string.Empty && previousLvl != currentLvl.levelName)
                {
                    // Updating Player Attack
                    player.SetAttackPower(currentLvl.attackBoost + player.defaultAttack);

                    // Updating Player Defense
                    player.SetDefense(currentLvl.defenseBoost + player.defaultDefense);

                    // Heal player as a reward for upgrading levels
                    player.Heal(player.maxHealth);

                    // show message in console for 10 seconds
                    HUDConsole._instance.Log(player.userName + " has reached " +currentLvl.levelName+"!\n"+
                    "Enjoy full health, +" + currentLvl.attackBoost + " Attack and "+
                    "+" + currentLvl.defenseBoost + " Defense!" , 10f);
                }

                // Updating Text in HUD
                currentLvlInHUD.GetComponent<TMP_Text>().text = currentLvl.levelName;
                nextLvlInHUD.GetComponent<TMP_Text>().text = nextLvl.levelName;

                previousLvl = currentLvl.levelName;
            }

            
        }

    }

    /*
     * Gets the index of the next level in the array of Levels
     * based on the current playerScore
     */
    private int GetNextLevelIndex(int playerScore)
    {
        int result = 0;
        for(int i = 0; i < levels.Length; i++)
        {
            if(playerScore < levels[i].scoreRequired) {
                result = i;
                break;
            }
        }

        return result;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public PlayerData playerData = null;
    public SceneData sceneData = null;
    public GameStatisticsData gameStatisticsData = null;
    public Player player = null;
    public static SaveManager _instance;
    private string savePath;

    private void Awake()
    {

        _instance = this;

        // set player userName
        this.player.userName = CreateNewGame.newUserName;
    }

    private void Start()
    {

        savePath = Application.persistentDataPath + "/saveData/";
        Debug.Log("Saving to path: " + savePath);
        this.playerData = new PlayerData();
        this.sceneData = new SceneData();
        this.gameStatisticsData = new GameStatisticsData();

        // if coming from load game button or entering a door way
        if (!CreateNewGame.pressedCreateGame && !MenuControls.pressedRestartLevel)
        {
            // load game save files overwriting default player
            LoadGame();

            if (MenuControls.pressedLoadGame)
            {
                StartCoroutine(LoadScene());
                MenuControls.pressedLoadGame = false;
            }
            else if(Application.isEditor)
            {
                // if in Edit mode, save game so everything doesn't go all over the place
                SaveGame();
            }
            if (DoorPortal.enteredDoorPortal)
            {
                // put player in starting position for the active Scene
                player.transform.position = player.startingTransform.initialPosition;
                player.transform.rotation = player.startingTransform.initialRotation;
                player.transform.localScale = player.startingTransform.initialSize;
                DoorPortal.enteredDoorPortal = false;
            }
        }

        // if creating a new game or restarting level
        if(CreateNewGame.pressedCreateGame || MenuControls.pressedRestartLevel)
        {

            // create new game save files with default player
            SaveGame();

            LoadGame();

            // reset Menu Controls variables
            CreateNewGame.pressedCreateGame = false;
            MenuControls.pressedRestartLevel = false;
            CreateNewGame.newUserName = string.Empty;
        }
    }

    /*
     * Saves entire game, updates save files
     */
    public void SaveGame()
    {
        SavePlayer();
        SaveGameStatistics();
        SaveScene();

        if(HUDConsole._instance != null)
        {
            HUDConsole._instance.Log("Saved Game successfully.", 2f);
        }
        
    }

    /*
     * Loads entire game from last saved files
     */
    public void LoadGame()
    {
        LoadPlayer();
        LoadGameStatistics();
    }

    [ContextMenu("Save Player")]
    public void SavePlayer()
    {
        this.playerData.userName = this.player.userName;
        this.playerData.attack = this.player.GetAttackPower();
        this.playerData.defense = this.player.GetDefense();
        this.playerData.maxHealth = this.player.maxHealth;
        this.playerData.health = this.player.GetCurrentHealth();
        this.playerData.playerScore = this.player.playerScore;
        this.playerData.isDead = this.player.isDead;

        // if player has not entered a door portal
        if (!DoorPortal.enteredDoorPortal)
        {
            this.playerData.playerPosition = this.player.transform.position;
            this.playerData.playerRotation = this.player.transform.rotation;
            this.playerData.playerScale = this.player.transform.localScale;
        }
        

        JSONLoaderSaver.SavePlayerDataAsJSON(savePath, "playerData.json",this.playerData);
        //BinaryLoaderSaver.SavePlayerAsBinary(savePath, "player.bin",this.playerData);
    }
    [ContextMenu("Load Player")]
    public void LoadPlayer()
    {
        this.playerData = JSONLoaderSaver.LoadPlayerDataFromJSON(savePath, "playerData.json");

        //this.playerData = BinaryLoaderSaver.LoadPlayerFromBinary(savePath,"player.bin");

        if(playerData != null)
        {
            this.player.userName = playerData.userName;
            this.player.SetAttackPower(playerData.attack);
            this.player.SetDefense(playerData.defense);
            this.player.maxHealth = playerData.maxHealth;
            this.player.SetHealth(playerData.health);
            this.player.playerScore = playerData.playerScore;
            this.player.isDead = playerData.isDead;

            if (!DoorPortal.enteredDoorPortal)
            {
                Debug.Log("Loaded Player position");
                this.player.transform.position = playerData.playerPosition;
                this.player.transform.rotation = playerData.playerRotation;
                this.player.transform.localScale = playerData.playerScale;
            }
        }
    }

    [ContextMenu("Save Scene")]
    void SaveScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        this.sceneData.sceneFullName = activeSceneName;

        Debug.Log("Saved Scene named: " + sceneData.sceneFullName);

        JSONLoaderSaver.SaveSceneDataAsJSON(savePath, "sceneData.json", this.sceneData);
    }
    [ContextMenu("Load Scene")]
    IEnumerator LoadScene()
    {
        this.sceneData = JSONLoaderSaver.LoadSceneDataFromJSON(savePath, "sceneData.json");
        
        yield return new WaitForSeconds(5);

        if (sceneData != null)
        {
            if(SceneManager.GetActiveScene().name != sceneData.sceneFullName)
            {
                // switch to correct scene
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneData.sceneFullName);

                while (!asyncOperation.isDone)
                {
                    yield return null;
                }
            } 
        }
        else
        {
            Debug.Log("ALERT: No save file exists. Create a New Game!");
        }
    }

    [ContextMenu("Save Game Statistics")]
    void SaveGameStatistics()
    {
        this.gameStatisticsData.bossesDefeatedNames = player.bossesDefeatedNames;
        this.gameStatisticsData.bossesDefeatedScenes = player.bossesDefeatedScenes;

        JSONLoaderSaver.SaveGameStatisticsDataAsJSON(savePath, "gameStatisticsData.json", this.gameStatisticsData);
    }
    [ContextMenu("Load Game Statistics")]
    void LoadGameStatistics()
    {
        this.gameStatisticsData = JSONLoaderSaver.LoadGameStatisticsDataFromJSON(savePath, "gameStatisticsData.json");

        if (gameStatisticsData != null)
        {
            player.bossesDefeatedNames = gameStatisticsData.bossesDefeatedNames;
            player.bossesDefeatedScenes = gameStatisticsData.bossesDefeatedScenes;
        }

        // removing already defeated bosses from the scene (TODO: only enable if player has completed the puzzle)
        for(int i = 0; i < player.bossesDefeatedNames.Count; i++)
        {
            GameObject bossDefeated = GameObject.Find(player.bossesDefeatedNames[i]);

            // if currentBoss is an Enemy present in the scene
            if (bossDefeated != null && bossDefeated.GetComponent<Enemy>() != null 
                && SceneManager.GetActiveScene().name == player.bossesDefeatedScenes[i])
            {
                // hide boss that was already defeated
                bossDefeated.SetActive(false);
            }
        }
    }

}

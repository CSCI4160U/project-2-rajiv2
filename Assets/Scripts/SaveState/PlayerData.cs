using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string userName;
    public int attack;
    public int defense;
    public int maxHealth = 100;
    public int health;
    public int playerScore;

    public bool isDead;

    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public Vector3 playerScale;
}

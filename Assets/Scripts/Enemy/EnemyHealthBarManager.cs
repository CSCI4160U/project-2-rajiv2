using TMPro;
using UnityEngine;
using UnityEngine.UI;

// RESOURCE: "How to Make A Simple Tooltip in Unity" https://www.youtube.com/watch?v=y2N_J391ptg
public class EnemyHealthBarManager : MonoBehaviour
{
    public static EnemyHealthBarManager _instance;

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    /*
     * Function updates the value of the Health Bar slider
     * and adds the enemy name above it with its textComponent
     */
    public void SetAndShowEnemyHealthBar(Enemy enemy)
    {
        gameObject.SetActive(true);

        if (textComponent != null)
        {
            // set health bar name to enemy name
            textComponent.text = enemy.enemyName;
        }

        if (healthSlider != null)
        {
            // set health slider values
            healthSlider.maxValue = enemy.maxHealth;
            healthSlider.value = enemy.health;
        }
    }

    public void HideEnemyHeathBar()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}

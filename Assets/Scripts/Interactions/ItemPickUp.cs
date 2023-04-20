using UnityEngine;
using TMPro;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Canvas hintCanvas = null;
    [SerializeField] private GameObject item = null;
    private Player player = null;
    private TextMeshProUGUI hintText;

    private void Awake()
    {
        if (hintCanvas != null)
        {
            hintText = hintCanvas.GetComponentInChildren<TextMeshProUGUI>();
            hintText.text = "Press Q to use "+ item.name + ".";
            hintCanvas.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && item != null)
        {
            player = other.GetComponent<Player>();
            if (hintCanvas != null)
            {
                hintCanvas.gameObject.SetActive(true);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // hide UI
        if (hintCanvas != null)
        {
            hintCanvas.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && player != null && item != null)
        {
            Gun gun = item.GetComponent<Gun>();
            HealingItem healingItem = item.GetComponent<HealingItem>();

            if (gun != null)
            {
                gun.transform.localScale = new Vector3(1, 1, 1);
                player.gun = gun;
                Destroy(gun.gameObject);
            }

            if (healingItem != null)
            {
                player.Heal(healingItem.healingPower);
                HUDConsole._instance.Log(player.name+" got healed using " + healingItem.itemName,5f);
                Destroy(healingItem.gameObject);
            }

        }
    }
}

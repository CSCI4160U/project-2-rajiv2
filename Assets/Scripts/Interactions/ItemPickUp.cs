using UnityEngine;
using TMPro;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Canvas hintCanvas = null;
    [SerializeField] private GameObject gameObject = null;
    private Player player = null;
    private TextMeshProUGUI hintText;

    private void Awake()
    {
        if (hintCanvas != null)
        {
            hintText = hintCanvas.GetComponentInChildren<TextMeshProUGUI>();
            hintText.text = "Press Q to pick up "+ gameObject.name + ".";
            hintCanvas.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && gameObject != null)
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
        if (Input.GetKeyDown(KeyCode.Q) && player != null && gameObject != null)
        {
            Gun gun = gameObject.GetComponent<Gun>();

            if(gun != null)
            {
                gun.transform.localScale = new Vector3(1, 1, 1);
                player.gun = gun;
                Destroy(gun.gameObject);
            }
            
        }
    }
}

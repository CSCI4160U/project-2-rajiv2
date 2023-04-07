using UnityEngine;
using System.Collections;
using TMPro;

public class HUDConsole : MonoBehaviour
{
    private GameObject console = null;
    [SerializeField] private TextMeshProUGUI textComponent;
    public static HUDConsole _instance;

    private void Awake()
    {
        _instance = this;

        console = _instance.gameObject;
    }
    private void Start()
    {
        console.SetActive(false);
    }

    public void Log(string message, float readTime)
    {
        console.SetActive(true);
        _instance.StartCoroutine(ShowMessage(message, readTime));
    }

    IEnumerator ShowMessage(string message, float readTime)
    {
        Debug.Log("Showing message on console...");
        textComponent.text = message;

        yield return new WaitForSeconds(readTime);

        console.SetActive(false);
        textComponent.text = string.Empty;

    }
}

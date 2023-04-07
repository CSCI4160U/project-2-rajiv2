using UnityEngine;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    private TMP_Text loadingText;

    private void Awake()
    {
        loadingText = this.GetComponentInChildren<TMP_Text>();
    }

    /*
     * Show Text Animation for Loading Screen
     */
    IEnumerator TextAnimation()
    {
        
        if(loadingText != null)
        {
            loadingText.text = "LOADING";
            yield return new WaitForSeconds(0.4f);
            loadingText.text = "LOADING.";
            yield return new WaitForSeconds(0.4f);
            loadingText.text = "LOADING..";
            yield return new WaitForSeconds(0.4f);
            loadingText.text = "LOADING...";
            yield return new WaitForSeconds(0.4f);

            Start();
        }
    }

    private void Start()
    {
        StartCoroutine(TextAnimation());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneEnter : MonoBehaviour
{
    public GameObject player;
    public GameObject cutSceneCamera;

    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        cutSceneCamera.SetActive(true);
        player.SetActive(false);
    }

    IEnumerator FinishCutScene()
    {
        yield return new WaitForSeconds(10);
        player.SetActive(true);
        cutSceneCamera.SetActive(false);
    }
}

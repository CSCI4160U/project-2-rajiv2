using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PatternBlock : MonoBehaviour
{
    public Material defaultMaterial;
    public Material activeMaterial;
    public Material correctMaterial;
    public Material incorrectMaterial;

    public int sequenceNumber;
    public bool wasActivated;

    private float activeTime = 1f;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockActive()
    {
        // set material to active
        rend.sharedMaterial = activeMaterial;
        yield return new WaitForSeconds(activeTime);
        // set material to default
        rend.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockCorrect()
    {
        // set material to active
        rend.sharedMaterial = correctMaterial;
        yield return new WaitForSeconds(2);
        // set material to default
        rend.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockIncorrect()
    {
        // set material to active
        rend.sharedMaterial = incorrectMaterial;
        yield return new WaitForSeconds(0.5f);
        // set material to default
        rend.sharedMaterial = defaultMaterial;
    }

    public void SetActiveTime(float time)
    {
        activeTime = time;
    }

    public float GetActiveTime()
    {
        return activeTime;
    }
}

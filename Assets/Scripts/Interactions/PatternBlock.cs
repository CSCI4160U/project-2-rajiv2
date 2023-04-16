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
    private Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockActive()
    {
        // set material to active
        renderer.sharedMaterial = activeMaterial;
        yield return new WaitForSeconds(activeTime);
        // set material to default
        renderer.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockCorrect()
    {
        // set material to active
        renderer.sharedMaterial = correctMaterial;
        yield return new WaitForSeconds(2);
        // set material to default
        renderer.sharedMaterial = defaultMaterial;
    }

    public IEnumerator FlashBlockIncorrect()
    {
        // set material to active
        renderer.sharedMaterial = incorrectMaterial;
        yield return new WaitForSeconds(0.5f);
        // set material to default
        renderer.sharedMaterial = defaultMaterial;
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

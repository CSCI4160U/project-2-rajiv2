using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] alarmLights = null;
    
    
    IEnumerator AlarmSequence()
    {
        yield return new WaitForSeconds(0.8f);

        for(int i = 0; i < alarmLights.Length; i++)
        {
            alarmLights[i].SetActive(!alarmLights[i].activeInHierarchy);
        }

        Activate();
    }

    private void Awake()
    {
        for (int i = 0; i < alarmLights.Length; i++)
        {
            alarmLights[i].SetActive(false);
        }
    }

    public void Activate()
    {
        StartCoroutine(AlarmSequence());
    }

    public void Deactivate()
    {
        StopCoroutine(AlarmSequence());
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutScene : MonoBehaviour
{
    [SerializeField] private GameObject timeLine = null;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(timeLine != null)
            {
                timeLine.SetActive(false);
            }
        }
    }
}

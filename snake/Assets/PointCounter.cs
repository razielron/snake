using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField]
    PointHUD pointHud;
    
    private void Start()
    {
        StartCoroutine(CountPoints());
    }

    private IEnumerator CountPoints()
    {
        while (true)
        {
            pointHud.Points += 5;
            yield return new WaitForSeconds(1);
        }
    }
}

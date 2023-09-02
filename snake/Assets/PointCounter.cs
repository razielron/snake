using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField]
    private PointHUD pointHud;
    public bool GameContinues { get; set; } = true;
    public double Factor { get; set; } = 1;
    public double Points
    {
        get
        {
            return pointHud.Points;
        }
    }
    
    public void Start()
    {
        StartCoroutine(CountPoints());
    }

    private IEnumerator CountPoints()
    {
        while (GameContinues)
        {
            pointHud.Points += Factor;
            yield return new WaitForSeconds(1);
        }
    }
}

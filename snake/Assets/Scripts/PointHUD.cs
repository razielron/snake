using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointHUD : MonoBehaviour
{
    [SerializeField]
    Text pointText;
    private double points = 0;
    
    private void Awake()
    {
        UpdateHUD();
    }

    public double Points
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        pointText.text = points.ToString();
    }
}
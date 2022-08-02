using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Color originColor; // 원래 색깔
    public Color hazardColor; // 빨간색
    
    // 경고를 위한 색 변경
    public void ChangeColor(Color color)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Color newColor = mesh.material.color;
        newColor = color;
        mesh.material.color = newColor;
    }
}

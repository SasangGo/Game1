using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Color originColor;
    public Color hazardColor;

    public void ChangeColor(Color color)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Color newColor = mesh.material.color;
        newColor = color;
        mesh.material.color = newColor;
    }
}

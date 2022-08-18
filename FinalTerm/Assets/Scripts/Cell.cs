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

    // Wall 스킬 때 벽으로 막기 위함
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            AObstacle obj = other.GetComponent<AObstacle>();
            StartCoroutine(obj.ReturnObstacle(0, obj.Index));
        }
    }
}

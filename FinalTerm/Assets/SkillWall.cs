using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AObstacle obs = other.gameObject.GetComponent<AObstacle>();
        if (obs != null)
        {
            StartCoroutine(obs.ReturnObstacle(0, obs.Index));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBomb : MonoBehaviour
{

    void OnParticleCollision(GameObject other)
    {

        SkillManager.Instance.debugText.text = "s"; 
        if (other.gameObject.tag == "Bullet")
        {
            SkillManager.Instance.debugText.text = "ss";
            AObstacle obj = other.GetComponent<AObstacle>();
            StartCoroutine(obj.ReturnObstacle(0, obj.Index));
        }
    }
}

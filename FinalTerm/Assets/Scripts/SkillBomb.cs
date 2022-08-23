using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBomb : MonoBehaviour
{
    public float speed = 5f;
    public GameObject effect;

    private void Update()
    {
        if (transform.localScale.x < 40f)
            transform.localScale += new Vector3(speed, speed, speed) * Time.deltaTime;
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            effect.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SkillManager.Instance.debugText.text = other.gameObject.tag;
        if(other.gameObject.tag == "Bullet")
        {
            AObstacle obstacle = other.GetComponent<AObstacle>();
            StartCoroutine(obstacle.ReturnObstacle(0, obstacle.Index));
        }
    }
}

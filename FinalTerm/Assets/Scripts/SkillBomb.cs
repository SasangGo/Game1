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
        AObstacle obstacle = other.GetComponent<AObstacle>();
        if (obstacle != null)
        {
            StartCoroutine(obstacle.ReturnObstacle(0, obstacle.Index));
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        //layer == 12 -> Enemy
        if (obj.layer == 12)
        {
            ABoss enemy = obj.GetComponent<ABoss>();
            if (collision.collider.CompareTag("Damagable"))
            {
                enemy.OnDamaged(Vector3.zero);
            }
        }
    }

}

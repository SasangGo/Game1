using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    public int maxHp = 5;
    public int hp;

    private void OnEnable()
    {
        maxHp = 5;
        hp = maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            hp--;

            if (hp <= 0)
            {
                RemoveClone();
                return;
            }

            gameObject.layer = 9;
            Invoke("Invincibility", 1.5f);

            AObstacle obj = other.GetComponent<AObstacle>();
            StartCoroutine(obj.ReturnObstacle(0, obj.Index));
        }
    }

    private void Invincibility()
    {
        gameObject.layer = 0;
    }

    private void RemoveClone()
    {
        gameObject.SetActive(false);
    }
}

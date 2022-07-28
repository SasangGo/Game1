using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AObstacle : MonoBehaviour
{
    protected virtual void OnCollisionEnter(Collision collision)
    {
        PlayerControl player = collision.collider.GetComponent<PlayerControl>();
        if(player != null)
        {
            StartCoroutine(player.DieOperate());
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();
        if(player != null)
        {
            StartCoroutine(player.DieOperate());
        }
    }
    protected IEnumerator ReturnObstacle(float time, int index)
    {
        float cnt = 0;
        while (cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        ObjectPool.Instance.ReturnObject(gameObject, index);
    }
}

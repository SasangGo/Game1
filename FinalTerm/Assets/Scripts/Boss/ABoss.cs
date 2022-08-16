using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ABoss : MonoBehaviour
{
    protected int health;
    protected int speed;
    protected Transform target;

    protected Rigidbody rigid;

    public enum BossState
    {
        idle,trace,attack,dead
    }
    BossState bossState = BossState.idle;
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        InvokeRepeating("UpdateTarget",1,1);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (target == null) return;
        Rotate(target.position);
    }
    protected virtual void UpdateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, LayerMask.GetMask("Player"));
        for (int i = 0; i < cols.Length; i++)
        {
            PlayerControl newTarget = cols[i].GetComponent<PlayerControl>();
            if (newTarget != null)
            {
                target = newTarget.transform;
                Trace(target.position);
                return;
            }
        }
        target = null;
    }
    protected virtual void Trace(Vector3 pos)
    {
        bossState = BossState.trace;
    }
    protected virtual void Rotate(Vector3 pos)
    {
        if (pos == Vector3.zero) return;
        Vector3 dir = new Vector3(pos.x, 0, pos.z);
        Quaternion newRot = Quaternion.LookRotation(dir);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRot, Time.deltaTime * speed);
    }
    /*//Vector3Int GetMovePosition()
    //{
        float dir = rigid.rotation.y;
        if(90f > dir && dir > -90f)
    //}*/
}

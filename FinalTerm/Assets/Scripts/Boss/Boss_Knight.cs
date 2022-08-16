using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Knight : ABoss
{
    const int OFFSET = -135;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        speed = 5;
        health = 4;
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }
    protected override void Rotate(Vector3 pos)
    {
        base.Rotate(pos);
        Vector3 newPos = rigid.rotation.eulerAngles + new Vector3(0, OFFSET, 0);
        Quaternion newRot = Quaternion.Euler(newPos);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRot, Time.deltaTime * speed);
    }
}

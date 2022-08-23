using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInteract : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("이펙트 충돌");
    }
}

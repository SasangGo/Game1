using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();
        if(player != null && player.gameObject.layer == 10)
        {
            player.OnDamaged();
        }
    }
}

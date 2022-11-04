using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBomb : MonoBehaviour
{
    [SerializeField] BombBlockPattern pattern;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            Cell cell = other.GetComponent<Cell>();
            other.gameObject.SetActive(false);
            pattern.cells.Add(cell);
        }
        if(other.gameObject.layer == 10)
        {
            other.GetComponent<PlayerControl>().OnDamaged();
        }
    }
}

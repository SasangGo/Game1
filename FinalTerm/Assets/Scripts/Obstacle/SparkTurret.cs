using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkTurret : MonoBehaviour
{
    private Vector3 beforePos;
    private Vector3 affterPos;
    public float speed = 10f;

    protected void OnEnable()
    {
        beforePos = transform.position;
        affterPos = new Vector3(transform.position.x, -6.7f, transform.position.z);
        Invoke("ChangeColliderEnable", 0.5f);
        Invoke("ChangePos", 3f);
        Invoke("ChangeActiveFalse", 4.5f);
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell == null)
        {
            if (other.gameObject.layer == 10)
                other.GetComponent<PlayerControl>().OnDamaged();
        }
        else
        {
            cell.ChangeColor(Color.red);
            StartCoroutine("ChangeOriginColor", cell);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, affterPos, Time.deltaTime * speed);
    }

    private void ChangePos()
    {
        affterPos = beforePos;
    }

    private IEnumerator ChangeOriginColor(Cell cell)
    {
        yield return new WaitForSeconds(1f);
        cell.ChangeColor(cell.originColor);
    }

    private void ChangeColliderEnable()
    {
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private void ChangeActiveFalse()
    {
        gameObject.SetActive(false);
    }

}

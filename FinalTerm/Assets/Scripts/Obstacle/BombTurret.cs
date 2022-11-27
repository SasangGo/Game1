using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTurret : MonoBehaviour
{
    [SerializeField] Transform centerPos;
    [SerializeField] GameObject bigBomb;

    private Vector3 beforePos;
    private Vector3 affterPos;

    public float speed = 10f;
    public bool isStart;

    protected void OnEnable()
    {
        Vector3 temp = centerPos.position;
        temp.y = transform.position.y;
        transform.LookAt(temp);

        beforePos = transform.position;
        affterPos = new Vector3(transform.position.x, -6.7f, transform.position.z);
        GetComponent<Collider>().enabled = true;
        Invoke("ShootBomb", 1.5f);
        Invoke("ChangePos", 3f);
        Invoke("ChangeActiveFalse", 4f);
    }

    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
        isStart = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell != null)
        {
            cell.ChangeColor(Color.red);
            StartCoroutine("ChangeOriginColor", cell);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
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
        isStart = true;
    }

    private void ChangeActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void ShootBomb()
    {
        bigBomb.transform.position = transform.position + new Vector3(0,7f,0);
        Vector3 direction = (centerPos.position - bigBomb.transform.position) * 0.3f + new Vector3(0, 10f, 0);
        bigBomb.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bigBomb.SetActive(true);
        bigBomb.GetComponent<Rigidbody>().AddForce(direction * 30f, ForceMode.Impulse);
    }
}

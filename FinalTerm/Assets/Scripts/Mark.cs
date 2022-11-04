using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : AObstacle
{
    private float speed = 15f;

    public Vector3 direction;
    public Vector3 start;

    private List<Cell> cells = new List<Cell>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, direction, Time.deltaTime * speed);
    }
    private void OnDisable()
    {
        foreach(Cell cell in cells){
            cell.gameObject.SetActive(true);
        }
        cells = new List<Cell>();
    }

    private void FixedUpdate()
    {
        // 아래 쪽으로 레이캐스트를 쏴서 땅을 빨간색으로 만듬
        Ray ray = new Ray(transform.position, -transform.up);
        hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if (hits != null)
        {
            foreach (var hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(Color.red);
                StartCoroutine(RemoveGround(cell));
            }
        }
    }

    IEnumerator RemoveGround(Cell cell)
    {
        // 땅과 부딪힐 때는 땅을 원래 색으로 바꿈

        yield return new WaitForSeconds(1f);
        cell.ChangeColor(cell.originColor);
        cell.gameObject.SetActive(false);
        cells.Add(cell);
    }
}

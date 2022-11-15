using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : AObstacle
{
    [SerializeField] float speed; // 속도
    private bool timerEnd;

    protected override void OnEnable()
    {
        base.OnEnable();

        speed = 200f;
        timerEnd = false;
        StartCoroutine(Timer(2f)); // 2초 뒤에 날아가게 하기 위해
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Cell cell = other.GetComponent<Cell>();
        if(cell != null)
        {
            cell.ChangeColor(cell.originColor); // 셀과 충돌시 색을 원래대로
        }
    }
    // 생성 후 정지시간(날아가기 전 멈춰있는 시간)
    private IEnumerator Timer(float time)
    {
        float cnt = 0;
        while(cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        timerEnd = true;
        StartCoroutine(ReturnObstacle(3f, 2)); // 생성 3초후 사라짐
        SoundManager.Instance.PlaySound(SoundManager.Instance.objectAudioSource, SoundManager.Instance.missileSound);

    }
    void Update()
    {
        if (!timerEnd) return; // timerEnd시 이동
        transform.position += -transform.forward * Time.deltaTime * speed;
    }
    private void FixedUpdate()
    {
        if (timerEnd) return;

        // 미사일 이동 방향으로 있는 모든 셀을 빨간색으로 변경
        Ray ray = new Ray(transform.position, -transform.forward);
        hits = Physics.SphereCastAll(ray, 5f,Mathf.Infinity,LayerMask.GetMask("Ground"));
        if (hits != null)
        {
            foreach (RaycastHit hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(Color.red);
            }
        }
    }
}

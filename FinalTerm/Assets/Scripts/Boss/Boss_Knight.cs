using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Knight : ABoss
{
    [SerializeField] ParticleSystem rushEffect;
    [SerializeField] ParticleSystem diveEffect;
    [SerializeField] GameObject pawnPrefab;
    [SerializeField] float rushDelay = 3f;
    [SerializeField] int amountOfPawn = 10;

    private float rushTime = 0;
    private const int spawnOffsetX = -15;
    private const float divePower = 100f;
    private int diveY = 20;
    private const float MIN_DIVE_RANGE = 0.5f;
    private const float MAX_DIVE_RANGE = 500f;
    // 기본적인 정보
    protected override void OnEnable()
    {
        base.OnEnable();
        InvokeRepeating("UpdateTarget", 0.5f, 1);
        Invoke("OnAction", 3);
        speed = 50;
        health = 4;
        cntHealth = 2;
    }
    protected override void Update()
    {
        rushTime += Time.deltaTime;
        if (target == null) return;
        Rotate(target.position);

        if (bossState != BossState.idle) return;


        Vector3Int targetPos = ConvertCellPos(target.position);
        if (ConvertCellPos(transform.position).x == targetPos.x
            || ConvertCellPos(transform.position).z == targetPos.z)
        {
            // rushTime 마다 한번씩만 발동하도록 설정
            if (rushTime > rushDelay)
            {
                rushTime = 0;
                Vector3 dir = (targetPos - ConvertCellPos(transform.position));
                StartCoroutine(Rush((dir.normalized)));
            }
        }
    }
    protected override void Trace(Vector3 pos)
    {
        base.Trace(pos);
        // 체스에서 말의 움직임 구현
        moveList.Add(GetMovePosition(2, 1));
        moveList.Add(GetMovePosition(1, 2));

        // 두 방향 중 랜덤 방향으로 움직임
        while(moveList.Count > 0)
        {
            int idx = Random.Range(0, moveList.Count);
            // 배열 중 이동 가능한 장소가 있으면 MovingAction을 실행하여 이동
            if (CheckCanMove(moveList[idx]))
            {
                Debug.Log("움직임");

                StartCoroutine(MovingAction(moveList[idx]));
                break;
            }
            //그 장소가 이동불가능하다면 삭제
            moveList.Remove(moveList[idx]);
        }
        //리스트가 다 삭제될때까지 이동가능한 곳이 없다면 이동안하고 종료
        //새로운 리스트를 받기위해 리스트 초기화
        moveList.Clear();
    }
    protected override void OnAction()
    {
        base.OnAction();
        action = Random.Range(0, 2);
        switch (action)
        {
            case 0:
                {
                    if (target != null) Trace(target.position);
                }
                break;
            case 1:
                SpawnPawn();
                break;
            case 2:
                if (target != null) StartCoroutine(Dive(target.position));
                break;
        }
        Invoke("OnAction", 3);
    }
    private IEnumerator Rush(Vector3 dir)
    {
        bossState = BossState.attack;
        CancelInvoke();
        rushEffect.gameObject.SetActive(true);
        while (CheckCanMove(ConvertCellPos(transform.position + dir)))
        {
            transform.Translate(dir * Time.deltaTime * speed);
            yield return null;
        }
        transform.position = ConvertCellPos(transform.position);
        bossState = BossState.idle;
        rushEffect.gameObject.SetActive(false);
        Invoke("OnAction", 3);
        InvokeRepeating("UpdateTarget", 0.5f, 1);
    }
    private void SpawnPawn()
    {
        int index = Random.Range(0, 4);
        int sPosX = GameManager.Instance.MAP_START_X;
        int sPosZ = GameManager.Instance.MAP_START_Z;
        int ePosX = GameManager.Instance.MAP_END_X;
        int ePosZ = GameManager.Instance.MAP_END_Z;
        int offsetY = GameManager.Instance.CELL_OFFSET_Y;
        int temp = 0;

        for (int i = 0; i < amountOfPawn; i++)
        {
            GameObject pawn = Instantiate(pawnPrefab);
            if (index == 0)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX+ spawnOffsetX + temp, offsetY, sPosZ));
            else if (index == 1)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX + spawnOffsetX, offsetY, sPosZ+temp));
            else if (index == 2)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX + spawnOffsetX + temp, offsetY, ePosZ));
            else
                pawn.transform.position = ConvertCellPos(new Vector3(ePosX + spawnOffsetX, offsetY, sPosZ + temp));

            pawn.transform.Rotate(Vector3.up, 90 * index);
            temp += 5;
        }
        bossState = BossState.idle;
    }
    private IEnumerator Dive(Vector3 location)
    {
        rigid.AddForce(Vector3.up * divePower, ForceMode.VelocityChange);
        location = ConvertCellPos(location) + Vector3.up * diveY;
        rigid.velocity = Vector3.zero;
        Ray ray = new Ray(location, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell != null)
            {
                cell.ChangeColor(cell.hazardColor);
                yield return new WaitForSeconds(2f);
                cell.ChangeColor(cell.originColor);
                transform.position = location;
                rigid.AddForce(Vector3.down * divePower * 2f, ForceMode.VelocityChange);
                yield return new WaitWhile(() => Vector3.Distance(transform.position, cell.transform.position) > 3.3f);
                yield return StartCoroutine(DiveEffect(1.5f));
            }
        }
    }
    private IEnumerator DiveEffect(float t)
    {
        diveEffect.Play();
        float cnt = 0;

        Collider[] hits = new Collider[10]; 
        float radius = MIN_DIVE_RANGE;
        while(cnt < t)
        {
            Debug.Log("시간");
            cnt += Time.deltaTime;
            radius = Mathf.Lerp(radius, MAX_DIVE_RANGE, Time.deltaTime);
            int num = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, LayerMask.GetMask("Player"));
            if(num > 0)
            {
                for(int i = 0; i < num; i++)
                {
                    Rigidbody rigid = hits[i].GetComponent<Rigidbody>();
                    if (rigid != null && rigid.position.y < -5f)
                    {
                        Debug.Log("폭발");
                        rigid.AddExplosionForce(4000f, transform.position, 1000f, 0f);
                        rigid.GetComponent<PlayerControl>().jumpCount++;
                        break;
                    }
                }
            }
            yield return null;
        }
    }
}

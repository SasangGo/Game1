using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Knight : ABoss
{
    private const int DIVEY = 20;
    private const float RUSHSPEED = 100F;
    private const float MIN_DIVE_RANGE = 0.5f;
    private const float MAX_DIVE_RANGE = 100f;
    private const int SPAWNOFFSETX = -15;
    private const float DIVEPOWER = 100f;

    [SerializeField] ParticleSystem rushEffect;
    [SerializeField] ParticleSystem diveEffect;
    [SerializeField] ParticleSystem flameEffect;
    [SerializeField] GameObject pawnPrefab;
    [SerializeField] int amountOfPawn = 10;

    private float rushDelay;

    private float rushTime = 0;
    // 기본적인 정보
    protected override void OnEnable()
    {
        base.OnEnable();
        InvokeRepeating("UpdateTarget", 0.1f, 0.25f);
        Invoke("OnAction", actionDelay);
        speed = 50;
        health = 4;
        cntHealth = 2;
        rushDelay = 10f;
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
    protected override void Trace()
    {
        base.Trace();
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
        action = Random.Range(0, 4);
        switch (action)
        {
            case 0:
                Trace();
                break;
            case 1:
                SpawnPawn();
                break;
            case 2: 
                StartCoroutine(Dive());
                break;
            case 3:
                StartCoroutine(Flame());
                break;
        }
    }
    private IEnumerator Rush(Vector3 dir)
    {
        if (dir == Vector3.zero) yield break;
        bossState = BossState.attack;
        anim.enabled = false;
        CancelInvoke();
        Rotate(dir, true);
        rushEffect.gameObject.SetActive(true);
        while (CheckCanMove(ConvertCellPos(transform.position + dir)))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * RUSHSPEED);
            yield return null;
        }
        transform.position = ConvertCellPos(transform.position - dir);
        bossState = BossState.idle;
        anim.enabled = true;
        rushEffect.gameObject.SetActive(false);
        Invoke("OnAction", actionDelay);
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

        if (target != null) Rotate(target.position);
        for (int i = 0; i < amountOfPawn; i++)
        {
            GameObject pawn = Instantiate(pawnPrefab);
            if (index == 0)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX+ SPAWNOFFSETX + temp, offsetY, sPosZ));
            else if (index == 1)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX + SPAWNOFFSETX, offsetY, sPosZ+temp));
            else if (index == 2)
                pawn.transform.position = ConvertCellPos(new Vector3(sPosX + SPAWNOFFSETX + temp, offsetY, ePosZ));
            else
                pawn.transform.position = ConvertCellPos(new Vector3(ePosX + SPAWNOFFSETX, offsetY, sPosZ + temp));
            
            pawn.transform.Rotate(Vector3.up, 90 * index);
            temp += 5;
        }
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
    private IEnumerator Dive()
    {
        if (target == null) StopCoroutine(Dive());
        Vector3 location = target.position;
        bossState = BossState.attack;
        anim.enabled = false;
        rigid.AddForce(Vector3.up * DIVEPOWER, ForceMode.VelocityChange);
        // 보스 추락 방지
        Ray checkRay = new Ray(location, location - Vector3.down);
        // 타겟 위치에 땅이 없을 시 (낭떠러지일때)
        if (!(Physics.Raycast(checkRay, Mathf.Infinity, LayerMask.GetMask("Ground"))))
        {
            location = transform.position + Vector3.up * DIVEY;
        }
        // 아닐 시 타겟 위치로 이동
        else location = ConvertCellPos(location) + Vector3.up * DIVEY;

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
                rigid.AddForce(Vector3.down * DIVEPOWER * 2f, ForceMode.VelocityChange);
                yield return new WaitWhile(() => Vector3.Distance(transform.position, cell.transform.position) > 3.3f);
                yield return StartCoroutine(DiveEffect(0.5f));
            }
        }
    }
    private IEnumerator DiveEffect(float t)
    {
        diveEffect.Play();
        // 셀 위치가 똑같거나 타겟을 못찾으면 방향회전 X(축비틀어짐 방지)
        if(target != null) Rotate(target.position, true);
        float cnt = 0;

        Collider[] hits = new Collider[10]; 
        float radius = MIN_DIVE_RANGE;
        while (cnt < t)
        {
            cnt += Time.deltaTime;
            radius = Mathf.Lerp(radius, MAX_DIVE_RANGE, Time.deltaTime);
            int num = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, LayerMask.GetMask("Player"));
            if(num > 0)
            {
                for(int i = 0; i < num; i++)
                {
                    Rigidbody rigid = hits[i].GetComponent<Rigidbody>();
                    if (rigid != null && rigid.position.y < -5f && rigid.gameObject.layer == 10)
                    {
                        UpdateTarget();
                        Debug.Log("폭발");
                        rigid.AddExplosionForce(10000f, transform.position, 500f, 0f);
                        rigid.GetComponent<PlayerControl>().jumpCount++;
                        break;
                    }
                }
            }
            yield return null;
        }
        bossState = BossState.idle;
        anim.enabled = true;
        Invoke("OnAction", actionDelay);
    }
    private IEnumerator Flame()
    {
        if (target != null) StopCoroutine(Flame());
        Vector3 location = target.position;
        bossState = BossState.attack;
        //y축은 못따라가게 y축 고정
        location = new Vector3(location.x, GameManager.Instance.CELL_OFFSET_Y,location.z);
        Vector3 dir = (location - transform.position).normalized;
        Quaternion newRot = Quaternion.LookRotation(dir);
        flameEffect.Play();
        anim.enabled = false;
        while (flameEffect.isPlaying)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * speed);
            yield return null;
        }
        anim.enabled = true;
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
    protected override void Enraged()
    {
        base.Enraged();
        StartCoroutine(Smallize(new Vector3(2, 2, 2)));
        rushDelay /= 2;
        actionDelay /= 2;
    }
    private IEnumerator Smallize(Vector3 size)
    {
        anim.enabled = false;
        Debug.Log("광폭");
        while (transform.lossyScale.x >= size.x)
        {
            Debug.Log("작아");
            yield return null;
            transform.localScale *= 0.9f;
        }

        anim.enabled = true;
        anim.SetBool("Rage", true);
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
}

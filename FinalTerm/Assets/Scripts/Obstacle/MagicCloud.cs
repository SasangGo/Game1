using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCloud : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Light fieldLight;
    [SerializeField] Material fieldColor;
    [SerializeField] GameObject thunder;
    [SerializeField] GameObject redBall;


    private const float THUNDER_POSITION_Y = -6.3F;
    private const float OFFSET = 1.01F;
    private const float START_SPEED = 0.015F;
    private const float CLOUD_EXTEND_RATIO = 1.02F;
    private const float UP_SPEED = 5F;
    private const float THUNDER_DELAY = 7F;
    private const float THUNDER_RADIOUS = 6F;
    private const float ALERT_DELAY = 1F;

    private ParticleSystem[] effects;
    private Transform target;
    private float cntUpSpeed;
    private Color originLightColor;
    private void Start()
    {
        cntUpSpeed = START_SPEED;
        effects = thunder.GetComponentsInChildren<ParticleSystem>();
        Invoke("StartThunderField", 4f);
        InvokeRepeating("UpdateTarget", 4f, 0.5f);
        originLightColor = fieldLight.color;
    }
    void Update()
    {
        if (transform.position.y > 200f) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Bishop_Rage") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
        {
            redBall.transform.localScale *= CLOUD_EXTEND_RATIO;
            transform.Translate(Vector3.up * cntUpSpeed);
            cntUpSpeed *= OFFSET;
        }
        else transform.Translate(Vector3.up * UP_SPEED);
    }
    private void UpdateTarget()
    {
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(transform.position, new Vector3(60, 60, 60), Vector3.down, Quaternion.identity, Mathf.Infinity);
        if (hits.Length <= 0) return;

        foreach (RaycastHit hit in hits)
        {
            PlayerControl newTarget = hit.collider.GetComponent<PlayerControl>();
            if (newTarget != null) target = newTarget.transform;
        }
    }

    private void StartThunderField()
    {
        // 메모리 낭비를 막기 위해 보이지않는 레드볼 이펙트 비활성화
        redBall.SetActive(false);
        StartCoroutine(DarknessAction());

    }
    private IEnumerator DarknessAction()
    {
        while (fieldLight.color != fieldColor.color)
        {
            fieldLight.color = Color.Lerp(fieldLight.color, fieldColor.color, 0.1f);
            yield return null;
        }
        InvokeRepeating("StrikeThunder", 1f, THUNDER_DELAY);
    }
    private void StrikeThunder()
    { 
        Vector3 playerPos = target.position;
        int cellSize = GameManager.Instance.CELL_SIZE;
        playerPos = playerPos * cellSize / cellSize; 
        thunder.transform.SetParent(null);
        Vector3 thunderPos = new Vector3(playerPos.x, THUNDER_POSITION_Y, playerPos.z);
        thunder.transform.position = thunderPos;
        StartCoroutine(StrikeAction());
    }
    private IEnumerator StrikeAction()
    {
        RaycastHit[] hits;
        List<Cell> list = new List<Cell>();
        hits = Physics.SphereCastAll(thunder.transform.position,
            THUNDER_RADIOUS, Vector3.down, 5f, LayerMask.GetMask("Ground"));

        if(hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null)
                {
                    list.Add(cell);
                    cell.ChangeColor(cell.hazardColor);
                    Debug.Log("실행됨");
                }
            }
        }
        yield return new WaitForSeconds(ALERT_DELAY);
        foreach (ParticleSystem effect in effects) effect.Play();
        foreach (Cell cell in list) cell.ChangeColor(cell.originColor);
    }
    public void RemoveMagicCloud()
    {
        fieldLight.color = originLightColor;
        CancelInvoke();
        gameObject.SetActive(false);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancePattern : APattern
{
    [SerializeField] Transform ground;

    Vector3 affterRotation;
    float speed = 0.04f;
    private const float PHASETIME = 10f; // 진행시간

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 4f;
        isAlertEnd = true;
        StartCoroutine(PhaseTimer(PHASETIME));
        InvokeRepeating("ChangeRotate", 2f, 2f);
    }

    private void OnDisable()
    {
        ground.eulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        ground.eulerAngles = Vector3.Lerp(ground.eulerAngles, affterRotation, Time.deltaTime * speed);
    }

    private void ChangeRotate()
    {
        float x = (Random.Range(-40f, 40f) + 360f) % 360f;
        float y = (Random.Range(-40f, 40f) + 360f) % 360f;
        float z = (Random.Range(-40f, 40f) + 360f) % 360f;

        affterRotation = new Vector3(x, y, z);
        SkillManager.Instance.debugText.text = "" + affterRotation;
        SkillManager.Instance.debugText2.text = "" + ground.eulerAngles;
    }
}

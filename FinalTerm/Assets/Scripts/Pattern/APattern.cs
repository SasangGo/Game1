using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract를 안썼는데, 실수로 안쓴건지는 모르겠는데 쓰면 ㅈ될거같아서 꼬일까봐 일단 안씀
// 패턴 관리의 부모, 모든 패턴은 해당 스크립트를 상속받으면 됨

public class APattern : MonoBehaviour
{
    [SerializeField] protected Cell[] hazardZones; // 빨간색 셀
    [SerializeField] protected Transform sPos; // 셀의 시작 위치(거리 체크용)
    [SerializeField] protected Transform ePos;// 셀의 끝 위치(거리 체크용)
    [SerializeField] protected GameObject[] alertObjects;

    protected bool isAlertEnd; // 알림의 끝났는지 아닌지 체크
    public float expAmount;

    protected virtual void OnEnable()
    {
        isAlertEnd = false;
    }
    protected virtual void StartPattern()
    {
    }
    // 페이즈가 진행되는 시간(진행 시간)
    protected IEnumerator PhaseTimer(float time)
    {
        // 경고 시간이 끝날때까지 대기
        yield return new WaitUntil(() => isAlertEnd == true);

        float cnt = 0;
        StartPattern(); // 패턴 시작
        while (cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        // 시간이 다 지나면 Invoke 중단
        CancelInvoke();
        //GameManager에 EndPhase 실행
        GameManager.Instance.EndPhase(gameObject);
    }
    // 패턴 경고 코루틴
    protected IEnumerator AlertHazard(Cell[] cells)
    {
        isAlertEnd = false;
        // 빨간색으로 전환

        int count = 2;
        while(count-- > 0)
        {
            foreach(Cell cell in cells)
            {
                cell.ChangeColor(cell.hazardColor);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            foreach (Cell cell in cells)
            {
                cell.ChangeColor(cell.originColor);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
        isAlertEnd = true;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APattern : MonoBehaviour
{
    [SerializeField] protected Cell[] hazardZones;
    [SerializeField] protected Transform sPos;
    [SerializeField] protected Transform ePos;

    protected bool isAlertEnd;

    protected virtual void OnEnable()
    {
        isAlertEnd = false;
    }
    protected virtual void StartPattern()
    {
    }
    protected IEnumerator PhaseTimer(float time)
    {
        yield return new WaitUntil(() => isAlertEnd == true);

        float cnt = 0;
        StartPattern();
        while(cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        CancelInvoke();
        GameManager.Instance.EndPhase(gameObject);
    }

    protected IEnumerator AlertHazard(Cell[] cells)
    {
        isAlertEnd = false;
        for (int i = 0; i < 2; i++)
        {
            foreach (Cell cell in cells) cell.ChangeColor(cell.hazardColor);
            yield return new WaitForSeconds(0.5f);
            foreach (Cell cell in cells) cell.ChangeColor(cell.originColor);
            yield return new WaitForSeconds(0.5f);
        }
        isAlertEnd = true;
    }
}

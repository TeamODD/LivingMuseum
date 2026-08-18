using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveAno : MonoBehaviour
{
    [SerializeField] GameObject MoveObj;
    [SerializeField] GameObject FightObj;
    [SerializeField] bool isMoveX;//체크하면 좌우이동, 아니면 상하이동
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {
        FightObj.transform.localScale = Vector3.one;
        if (isMoveX)
            StartCoroutine("Move");
        else
            StartCoroutine("Move2");
    }
    IEnumerator Move()
    {
        int x = Random.Range(-350, 350);
        transform.DOLocalMoveX(x, 0.2f);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("Move");
    }
    IEnumerator Move2()
    {
        int y = Random.Range(-100, 100);
        transform.DOLocalMoveY(y, 0.2f);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("Move2");
    }

    public void YachaStart()
    {
        MoveObj.SetActive(false);
        FightObj.SetActive(true);
        FightObj.transform.DOScale(5, 10);
    }
}

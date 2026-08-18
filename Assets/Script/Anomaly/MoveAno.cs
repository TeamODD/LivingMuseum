using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveAno : MonoBehaviour
{
    [SerializeField] GameObject MoveObj;
    [SerializeField] bool isMoveX;//체크하면 좌우이동, 아니면 상하이동

    private void OnEnable()
    {       
        if (isMoveX)
            StartCoroutine("Move");
        else
            StartCoroutine("Move2");
    }
    IEnumerator Move()//가로로 움직이는애
    {
        int x = Random.Range(-350, 350);
        transform.DOLocalMoveX(x, 0.2f);
        yield return new WaitForSeconds(0.4f);
        StartCoroutine("Move");
    }
    IEnumerator Move2()//세로로 움직이는애
    {
        int y = Random.Range(-100, 100);
        transform.DOLocalMoveY(y, 0.2f);
        yield return new WaitForSeconds(0.4f);
        StartCoroutine("Move2");
    }
 
}

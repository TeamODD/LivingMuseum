using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveAno : MonoBehaviour
{
    [SerializeField] GameObject MoveObj;
    [SerializeField] GameObject FightObj;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("Move");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        FightObj.transform.localScale= Vector3.one;
        StartCoroutine("Move");
    }
    IEnumerator Move()
    {
        int x = Random.Range(-350, 350);
        transform.DOLocalMoveX(x,0.2f);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("Move");
    }

    public void YachaStart()
    {
        MoveObj.SetActive(false);
        FightObj.SetActive(true);
        FightObj.transform.DOScale(5, 10);
    }    
}

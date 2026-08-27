using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;

public class FightButton : MonoBehaviour
{
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [SerializeField] GameManager gameManager;
    [SerializeField] HideButton hideButton;
    [SerializeField] GameObject GetReadyForTheNextBattle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hideButton = GameObject.Find("HideButton").GetComponent<HideButton>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickFight()
    {
        if(hideButton.isHide) hideButton.ClickHide();//가리기 중이면 가리는거 치우고 바로 야차
        StartCoroutine("Fight");              
        left.transform.position = new Vector3(0, -30, 0);
        right.transform.position = new Vector3(0, -30, 0);
        left.transform.DOMoveY(-27, 3);
        right.transform.DOMoveY(-27, 3);
    }

    public void WalkMode()
    {
        left.transform.DOMoveY(-30, 2);
        right.transform.DOMoveY(-30, 2);
    }

    IEnumerator Fight()
    {
        GetReadyForTheNextBattle.SetActive(true);
        yield return new WaitForSeconds(1);
        GetReadyForTheNextBattle.SetActive(false);
        gameManager.StartFight();//상대 이동 시작
        gameManager.mode = 3;
    }
}

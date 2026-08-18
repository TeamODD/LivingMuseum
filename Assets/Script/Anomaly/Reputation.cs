using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reputation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI RepText;
    [SerializeField] int reputation;
    [SerializeField] Image anoImg;
    [SerializeField] Sprite anospire;
    public bool isMinusRep;//true면 평판이 까임

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reputation = 100;
        StartCoroutine("Change");

    }

    // Update is called once per frame
    void Update()
    {
        RepText.text = "평판 : " + reputation;
    }
    IEnumerator Change()
    {
        yield return new WaitForSeconds(5f);
        anoImg.sprite = anospire;
        StartCoroutine("RepCo");
    }
    IEnumerator RepCo()
    {
        if (isMinusRep) reputation--;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("RepCo");
    }
}

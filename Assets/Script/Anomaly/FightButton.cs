using UnityEngine;
using DG.Tweening;
using TMPro;

public class FightButton : MonoBehaviour
{
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickFight()
    {
        gameManager.mode = 3;
        left.transform.position = new Vector3(0, -30, 0);
        right.transform.position = new Vector3(0, -30, 0);
        left.transform.DOMoveY(-27, 3);
        right.transform.DOMoveY(-27, 3);
    }

    public void WalkMode()
    {
        text.text = "전투모드";
        gameManager.mode = 0;
        left.transform.DOMoveY(-30, 2);
        right.transform.DOMoveY(-30, 2);
    }
}

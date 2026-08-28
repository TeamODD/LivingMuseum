using TMPro;
using UnityEngine;

public class HideButton : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject Hide1;
    [SerializeField] GameObject Hide2;
    [SerializeField] GameObject Hide3;
    [SerializeField] GameObject TmpObj;
    [SerializeField] GameObject passButton;
    [SerializeField] private int rand;
    
    public bool isHide=false;//가리고 있으면 true
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager=GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickHide()
    {
        gameObject.SetActive(false);
        passButton.SetActive(false);
        if (isHide)
        {
            isHide = false;
            gameManager.mode = 0;
            TmpObj.GetComponent<Animator>().SetTrigger("Back");//이 애니메이션 끝나면 자동으로 오브젝트 false되도록 설정해놓음
        }
        else
        {
            isHide= true;
            gameManager.mode = 1;
            rand = Random.Range(0, 3);           
            if (rand == 0)
            {
                TmpObj = Hide1;
                Hide1.SetActive(true);
            }
            else if (rand == 1)
            {
                TmpObj = Hide2;
                Hide2.SetActive(true);
            }
            else
            {
                TmpObj = Hide3;
                Hide3.SetActive(true);
            }
        }
            
    }
}


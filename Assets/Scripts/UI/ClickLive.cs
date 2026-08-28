using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class ClickLive : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject FightButton;
    [SerializeField] GameObject HideButton;
    [SerializeField] GameObject PassButton;
    [SerializeField] GameManager gameManager;
    void Start()
    {
        Transform parentTransform = GameObject.Find("UICanvas").transform;
        FightButton = parentTransform.Find("FightButton")?.gameObject;
        PassButton = parentTransform.Find("PassButton")?.gameObject;
        HideButton = parentTransform.Find("HideButton")?.gameObject;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if(gameManager.mode==0)
        {
            if (gameManager.CanFightHere)//야차뜰수있고 관객도 없으면 야차버튼 활성화
            {
                FightButton.SetActive(true);
                PassButton.SetActive(true);
            }
            else if (gameManager.CanHideHere)//야차가 가능한데 관객이 있으면 가리기 버튼 활성화
            {
                HideButton.SetActive(true);
                PassButton.SetActive(true);
            }
        }       
    }
}

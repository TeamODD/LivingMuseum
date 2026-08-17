using UnityEngine;

public class HideButton : MonoBehaviour
{
    [SerializeField] GameObject hands;
    [SerializeField] GameObject hidePlayer;
    [SerializeField] bool isHide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isHide = false;//플레이어가 가리기 모드면 true
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void clickHide()
    {
        if (isHide)
        {
            hands.SetActive(true);
            hidePlayer.transform.localPosition = new Vector3(999, 999, 0);
            isHide = false;
        }
        else
        {
            hands.SetActive(false);
            hidePlayer.transform.localPosition= new Vector3(0, 0, 0);
            isHide = true;           
        }
    }
}

using UnityEngine;

public class Punch : MonoBehaviour//이 스크립트는 때릴 적 버튼에 넣어줘야함
{
    [SerializeField] private GameObject leftObj;
    [SerializeField] private GameObject rightObj;
    [SerializeField] private Animator left;
    [SerializeField] private Animator right;
    [SerializeField] private bool isleft=true;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        // 오브젝트에 붙어있는 Animator 컴포넌트를 가져옵니다.
        leftObj = GameObject.Find("left");
        rightObj = GameObject.Find("right");
        left = leftObj.GetComponent<Animator>();
        right = rightObj.GetComponent<Animator>();
        gameManager=GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // 버튼 클릭 시 호출할 함수
    public void OnMouseDown()
    {
        if(gameManager.mode==3)
        {
            Vector3 targetPos = gameObject.transform.position;
            targetPos.y -= 26;
            left.transform.position = targetPos;
            right.transform.position = targetPos;
            if (isleft)
            {
                left.SetTrigger("doPunch");
                isleft = false;
            }
            else
            {
                right.SetTrigger("doPunch");
                isleft = true;
            }
        }       
    }
    // 키보드 키 입력을 쓸 경우
    void Update()
    { 

    }
}

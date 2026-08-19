using DG.Tweening;
using UnityEngine;

public class Punch : MonoBehaviour // 이 스크립트는 때릴 적 버튼에 넣어줘야함
{
    [Header("손 오브젝트")]
    [SerializeField] private GameObject leftObj;
    [SerializeField] private GameObject rightObj;
    [SerializeField] private Animator left;
    [SerializeField] private Animator right;

    [Header("펀치 위치 미세 조절")]
    [Tooltip("상대 중심으로부터 높이 조절 (기본값 0이면 적 위치에 정확히 생성)")]
    private float offsetY = -25f;
    [Tooltip("왼손/오른손 좌우 간격 조절")]
    private float offsetX = 0f;

    [SerializeField] private bool isleft = true;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        leftObj = GameObject.Find("left");
        rightObj = GameObject.Find("right");

        if (leftObj != null) left = leftObj.GetComponent<Animator>();
        if (rightObj != null) right = rightObj.GetComponent<Animator>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // 버튼 클릭 시 호출할 함수
    public void OnMouseDown()
    {
        if (gameManager.mode == 3)
        {
            // 진행 중이던 DOTween 이동 취소
            left.transform.DOKill();
            right.transform.DOKill();

            // [수정] 클릭한 적의 현재 위치 기준
            Vector3 enemyPos = transform.position;

            if (isleft)
            {
                // 왼손은 적 위치에서 약간 왼쪽에 생성
                Vector3 targetPos = new Vector3(enemyPos.x - offsetX, enemyPos.y + offsetY, enemyPos.z);
                left.transform.position = targetPos;
                left.SetTrigger("doPunch");
                isleft = false;
            }
            else
            {
                // 오른손은 적 위치에서 약간 오른쪽에 생성
                Vector3 targetPos = new Vector3(enemyPos.x + offsetX, enemyPos.y + offsetY, enemyPos.z);
                right.transform.position = targetPos;
                right.SetTrigger("doPunch");
                isleft = true;
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveAno : MonoBehaviour
{
    [SerializeField] bool isApproach; // 체크하면 접근하는 적
    [SerializeField] bool noMove;     // 체크하면 안움직임
    [SerializeField] bool isMoveX;    // 체크하면 좌우이동, 아니면 상하이동
    [SerializeField] int amount1;
    [SerializeField] int amount2;

    [Header("접근 연출 설정")]
    [SerializeField] float scaleMultiplier = 2f; // 원래 크기 대비 몇 배로 커질지 (기본 2배)
    [SerializeField] float approachDuration = 10f; // 커지는 시간

    private Vector3 beforePosition;
    private Vector3 beforScale;

    public void Move()
    {
        StartCoroutine("JumpScare");
        if (!noMove)
        {
            beforePosition = transform.position;
            beforScale = transform.localScale;
            if (isApproach)
            {
                // [수정] 고정값 2가 아니라 원래 크기(beforScale)에 배율을 곱해 일정하게 확대
                transform.DOScale(beforScale * scaleMultiplier, approachDuration);

                if (TryGetComponent<HitButton>(out var hitButton))
                {
                    hitButton.GlitchStart();
                }
            }
            else
            {
                if (isMoveX)
                {
                    StartCoroutine(nameof(Move1));
                }
                else
                {
                    StartCoroutine(nameof(Move2));
                }
            }
        }
    }

    private void OnDisable()
    {
        // 비활성화 시 트윈 멈추고 원래 상태 복구
        transform.DOKill();
        transform.position = beforePosition;
        transform.localScale = beforScale;
    }

    public void fixPosition()
    {
        transform.DOKill(); // 위치/크기 트윈 중단
        transform.position = beforePosition;
        transform.localScale = beforScale;
    }
    IEnumerator JumpScare()
    {
        yield return new WaitForSeconds(5);//5초안에 못잡으면 패배
        gameObject.GetComponent<Animator>().SetTrigger("Jump");      
        yield return new WaitForSeconds(2f);//점프스퀘어 다 끝나고 패배 사운드
        GameObject.Find("GameManager").GetComponent<GameManager>().YachaLose();
    }
    IEnumerator Move1() // 가로로 움직이는 애
    {
        int x = Random.Range(-amount1, amount2);
        transform.DOLocalMoveX(x, 0.2f);
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(nameof(Move1));
    }

    IEnumerator Move2() // 세로로 움직이는 애
    {
        int y = Random.Range(-amount1, amount2);
        transform.DOLocalMoveY(y, 0.2f);
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(nameof(Move2));
    }
}
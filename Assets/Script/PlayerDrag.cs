using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDrag : MonoBehaviour, IDragHandler
{
    [SerializeField] float anoX;//이상현상의 x좌표
    [SerializeField] float anoY;//이상현상의 y좌표
    [SerializeField] Reputation reputation;//평판 스크립트를 받아서 평판 까일지 안까일지 직접 조작
    public void OnDrag(PointerEventData eventData)//드래그 함수
    {
        transform.position += (Vector3)eventData.delta;
    }
    private void Update()
    {
        Debug.Log(transform.localPosition);
        if (anoX - 10 <= transform.localPosition.x && transform.localPosition.x<=anoX+10)//이상현상이랑 플레이어 좌표가 비슷한지 판정
        {
            if(anoY-10<=transform.localPosition.y&&transform.localPosition.y<=anoY+10)
            {
                reputation.isMinusRep = false;//가리기 성공하면 평판 안까임
            }
            else
            {
                reputation.isMinusRep=true; //가리기 실패하면 평판 안까임
            }
        }
        else
        {
            reputation.isMinusRep = true;
        }
    }
}



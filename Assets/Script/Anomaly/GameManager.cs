using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int mode;//0이면 일반 순찰모드, 1이면 가리기, 2면 청소, 3이면 야차모드
    [SerializeField] FightButton fightButton;
    [SerializeField] int now = 0;//0~7까지 한 방, 8~15까지 한 방. 현재 위치를 표시
    [SerializeField] bool[] anoArr = new bool[16];//이상현상이 있는 방은 true로 표시
    [SerializeField] bool[] fightAno = new bool[16];//이상현상 중 야차를 떠야 하는 애는 이걸 true로 표시

    [SerializeField] GameObject FButton;
    public void WalkMode()
    {
        fightButton.WalkMode();
    }

    private void Update()
    {
        if (fightAno[now])
        {
            FButton.SetActive(true);
        }
        else
        { 
            FButton.SetActive(false); 
        }
    }

}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int mode;//0이면 일반 순찰모드, 1이면 가리기, 2면 청소, 3이면 야차모드
    [SerializeField] FightButton fightButton;

    public void WalkMode()
    {
        fightButton.WalkMode();
    }
    
}

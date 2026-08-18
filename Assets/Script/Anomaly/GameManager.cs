using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int mode;//0이면 일반 순찰모드, 1이면 가리기, 2면 청소, 3이면 야차모드
    [SerializeField] FightButton fightButton;
    [SerializeField] int now = 0;//0~7까지 한 방, 8~15까지 한 방. 현재 위치를 표시
    [SerializeField] int[] anoArr = new int[18];//이상현상이 없으면 0, 사물 이상현상이면 1, 생물 이상현상이면 2
    [SerializeField] GameObject[] AnomalyObjArr = new GameObject[16];
    [SerializeField] int[] anoIndex = new int[18];//0~5는 생명체 6~9는 무생물 10~15는 0~5에 대응하는 생물 방
    //생물은 야차, 무생물은 어루만지기, 관객이 있으면 야차는 불가해서 생물 가리기

    [SerializeField] GameObject FButton;//전투준비 버튼 온오프용

    private void Awake()
    {
        SetupAnomalyIndices();
    }
    void SetupAnomalyIndices()
    {
        // 1. 생명체 그룹 (0~5 값을 6개 지정 인덱스에 무작위 배치)
        int[] pos1 = { 0, 4, 6, 9, 13, 15 };
        int[] val1 = { 0, 1, 2, 3, 4, 5 };
        ShuffleAndAssign(pos1, val1);

        // 2. 무생물 그룹 (6~9 값을 4개 지정 인덱스에 무작위 배치)
        int[] pos2 = { 1, 7, 10, 16 };
        int[] val2 = { 6, 7, 8, 9 };
        ShuffleAndAssign(pos2, val2);

        // 3. 생물 방 그룹 (10~15 값을 6개 지정 인덱스에 무작위 배치)
        int[] pos3 = { 2, 5, 8, 11, 14, 17 };
        int[] val3 = { 10, 11, 12, 13, 14, 15 };
        ShuffleAndAssign(pos3, val3);

    }

    // 값들을 섞은 뒤 지정된 배열 인덱스에 덮어씌우는 함수
    void ShuffleAndAssign(int[] targetIndices, int[] values)
    {
        // 피셔-예이츠 셔플
        for (int i = values.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = values[i];
            values[i] = values[randomIndex];
            values[randomIndex] = temp;
        }

        // 섞인 값을 지정된 배열 인덱스 위치에 순서대로 대입
        for (int i = 0; i < targetIndices.Length; i++)
        {
            anoIndex[targetIndices[i]] = values[i];
        }
    }
    public void WalkMode()
    {
        fightButton.WalkMode();
    }

    private void Update()
    {
        if (anoArr[now]==2)//현재 생물 이상현상이 있으면(나중에 관객 여부 if문까지 추가해야함)
        {
            FButton.SetActive(true);
        }
        else
        { 
            FButton.SetActive(false); 
        }

        if(Keyboard.current.aKey.wasPressedThisFrame && now!=0&&now!=8)
        {
            now--;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame && now!=7&&now!=15)
        {
            now++;
        }
    }

}

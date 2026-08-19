using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int mode;//0이면 일반 순찰모드, 1이면 가리기, 2면 청소, 3이면 야차모드
    [SerializeField] FightButton fightButton;
    [SerializeField] public int now = 0;//0~10까지 한 방, 11~21까지 한 방. 현재 위치를 표시
    [SerializeField] int[] anoArr = new int[22];//이상현상이 없으면 0, 사물 이상현상이면 1, 생물 이상현상이면 2, 관객 이상현상이면 3.
    [SerializeField] GameObject[] AnomalyObjArr = new GameObject[12];
    [SerializeField] int[] anoIndex = new int[18];//0~5는 생명체 6~9는 무생물 10,11은 관객 12~17는 0~5에 대응하는 생물 출현 방 
    //실제 인덱스에는 0~8까지가 방1 9~17까지가 방2
    //생물은 야차, 무생물은 어루만지기, 관객이 있으면 야차는 불가해서 생물 가리기

    private Vector3[] basePositions;
    [Header("위치 간격 설정")]
    [SerializeField] private float roomOffsetX = 4.725f;  
    [SerializeField] private float floorOffsetY = 6.55f; 

    [SerializeField] GameObject FButton;//전투준비 버튼 온오프용

    void SetupAnomalyIndices()
    {
        // 1. 고정 위치 지정 (16, 17)
        anoIndex[2] = 16;
        anoIndex[11] = 17;

        // 2. 생명체 그룹 (0~5 값을 지정된 6개 인덱스에 무작위 배치)
        int[] pos1 = { 0, 4, 6, 9, 13, 15 };
        int[] val1 = { 0, 1, 2, 3, 4, 5 };
        ShuffleAndAssign(pos1, val1);

        // 3. 무생물 그룹 (6~9 값을 지정된 4개 인덱스에 무작위 배치)
        int[] pos2 = { 1, 7, 10, 16 };
        int[] val2 = { 6, 7, 8, 9 };
        ShuffleAndAssign(pos2, val2);

        // 4. 관객 그룹 (10~11 값을 누락되었던 3, 12번 인덱스에 무작위 배치)
        int[] pos4 = { 3, 12 };
        int[] val4 = { 10, 11 };
        ShuffleAndAssign(pos4, val4);

        // 5. 생물 방 그룹 (고정된 2, 11번을 제외한 남은 4개 인덱스에 12~15 무작위 배치)
        int[] pos3 = { 5, 8, 14, 17 };
        int[] val3 = { 12, 13, 14, 15 };
        ShuffleAndAssign(pos3, val3);
    }

    // 값들을 섞은 뒤 지정된 배열 인덱스에 덮어씌우는 함수 (anoIndex에 저장)
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


    void SpawnAno()
    {
        if (basePositions == null || basePositions.Length != AnomalyObjArr.Length)
        {
            basePositions = new Vector3[AnomalyObjArr.Length];
            for (int i = 0; i < AnomalyObjArr.Length; i++)
            {
                if (AnomalyObjArr[i] != null)
                {
                    basePositions[i] = AnomalyObjArr[i].transform.position;
                }
            }
        }

        System.Array.Clear(anoArr, 0, anoArr.Length);

        for (int slot = 0; slot < anoIndex.Length; slot++)
        {
            int anoValue = anoIndex[slot];

            if (anoValue >= 0 && anoValue < AnomalyObjArr.Length)
            {
                int objIndex = anoValue;
                GameObject targetObj = AnomalyObjArr[objIndex];

                if (targetObj != null)
                {
                    int roomX = slot % 9;        // 0 ~ 8 (구역 번호)
                    int floorIndex = slot / 9;   // 0 (1층) 또는 1 (2층)

                    // 변수로 뺀 간격 수치 적용
                    Vector3 offset = new Vector3(roomX * roomOffsetX, floorIndex * floorOffsetY, 0f);
                    targetObj.transform.position = basePositions[objIndex] + offset;
                }

                if (anoValue <= 5)
                {
                    anoArr[slot] = 2;
                }
                else if (anoValue <= 9)
                {
                    anoArr[slot] = 1;
                }
            }
        }
        ReorderAnoArr();//ANOARR을 재정렬

    }

    void ReorderAnoArr()
    {
        // 원본 anoArr(0~17 값) 복사
        int[] temp = (int[])anoArr.Clone();

        // anoArr 전체 0으로 초기화 (문 위치 0, 10, 11, 21 자동 0 처리)
        System.Array.Clear(anoArr, 0, anoArr.Length);

        // 1층 이상현상: 0~8번 -> 1~9번으로 1칸씩 미룸
        for (int i = 0; i < 9; i++)
        {
            anoArr[i + 1] = temp[i];
        }

        // 2층 이상현상: 9~17번 -> 12~20번으로 3칸씩 미룸 (0, 10, 11번 문 오프셋 고려)
        for (int i = 9; i < 18; i++)
        {
            anoArr[i + 3] = temp[i];
        }

        anoArr[4] = 3;
        anoArr[15] = 3;
        //관객 이상현상은 3으로 설정
    }

    public void WalkMode()
    {
        fightButton.WalkMode();
    }

    private void Awake()
    {
        SetupAnomalyIndices();
        Debug.Log("최종 배치 결과: " + string.Join(", ", anoIndex));
        SpawnAno();
    }

    private void Update()
    {
        Debug.Log("현재"+now);
        Debug.Log(anoArr[now]);
        if (anoArr[now]==2)//현재 생물 이상현상이 있으면(나중에 관객 여부 if문까지 추가해야함)
        {
            FButton.SetActive(true);
        }
        else
        { 
            FButton.SetActive(false); 
        }

        if(Keyboard.current.aKey.wasPressedThisFrame && now!=0&&now!=11)
        {
            now--;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame && now!=10&&now!=21)
        {
            now++;
        }
    }

}

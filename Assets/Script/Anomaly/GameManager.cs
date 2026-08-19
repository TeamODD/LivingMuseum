using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int mode;//0이면 일반 순찰모드, 1이면 가리기, 2면 청소, 3이면 야차모드
    [SerializeField] FightButton fightButton;
    [SerializeField] public int now = 0;//0~10까지 한 방, 11~21까지 한 방. 현재 위치를 표시
    [SerializeField] int[] anoArr = new int[22];//이상현상이 없으면 0, 사물 이상현상이면 1, 생물 이상현상이면 2, 관객 이상현상이면 3.
    [SerializeField] GameObject[] AnomalyObjArr = new GameObject[12];
    [SerializeField] int[] anoIndex = new int[22];//0~5는 생명체 6~9는 무생물 10,11은 관객 12~17는 0~5에 대응하는 생물 출현 방 
    //실제 인덱스에는 0~8까지가 방1 9~17까지가 방2
    //생물은 야차, 무생물은 어루만지기, 관객이 있으면 야차는 불가해서 생물 가리기
    [SerializeField] bool[] anoEnabled = new bool[22];//현재 그 구역에서 이상현상이 발생중인지 판정하는 배열. true면 있음
    [SerializeField] bool[] canYacha = new bool[22];//그 구역에서 야차를 뜰 수 있는지 판정하는 배열.
    [SerializeField] bool[] isCrowd = new bool[22];//그 구역에 관객이 있는지 판정하는 배열.

    private Vector3[] basePositions;
    [Header("위치 간격 설정")]
    [SerializeField] private float roomOffsetX = 4.725f;  
    [SerializeField] private float floorOffsetY = 6.55f; 

    [SerializeField] GameObject FButton;//야차 버튼 온오프용
    [SerializeField] GameObject Warning;
    [SerializeField] GameObject CButton;//청소버튼

    [SerializeField] float timer = 8f;//8초마다 이상현상 출몰

    [SerializeField] AudioSource bgm1;//기본브금
    [SerializeField] AudioSource bgm2;//야차브금
    [SerializeField] AudioSource yachawin;
    [SerializeField] AudioSource yachalose;    
    [SerializeField] AudioSource yachastart;    
    [SerializeField] AudioSource spawnsound;
    [SerializeField] AudioSource clean;
    [SerializeField] AudioSource footstep;

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

        // [수정] anoIndex.Length(22) 대신 실제 셔플 구역 수인 18로 변경
        for (int slot = 0; slot < 18; slot++)
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

        ReorderAnoArr(); // ANOARR 및 anoIndex 재정렬
    }

    void ReorderAnoArr()
    {
        int[] tempArr = (int[])anoArr.Clone();
        int[] tempIndex = (int[])anoIndex.Clone();

        // 두 배열 모두 0으로 초기화 (0, 10, 11, 21번 문 위치는 0이 됨)
        System.Array.Clear(anoArr, 0, anoArr.Length);
        System.Array.Clear(anoIndex, 0, anoIndex.Length);

        // 1층 (0~8번 -> 1~9번으로 1칸씩 이동)
        for (int i = 0; i < 9; i++)
        {
            anoArr[i + 1] = tempArr[i];
            anoIndex[i + 1] = tempIndex[i];
        }

        // 2층 (9~17번 -> 12~20번으로 3칸씩 이동)
        for (int i = 9; i < 18; i++)
        {
            anoArr[i + 3] = tempArr[i];
            anoIndex[i + 3] = tempIndex[i];
        }

        anoArr[4] = 3;
        anoArr[15] = 3;
        //관객 이상현상은 3으로 설정

        anoIndex[0] = 99;
        anoIndex[10] = 99;
        anoIndex[11] = 99;
        anoIndex[21] = 99;
        //문 칸에 해당하는 이상객체는 없으므로 99로

        anoIndex[3] = 107;
        anoIndex[6] = 101;
        anoIndex[9] = 105;
        anoIndex[14] = 118;
        anoIndex[17] = 112;
        anoIndex[20] = 116;
        //괴물 스폰되는 방은 그 괴물 인덱스 +100
    }

    public void YachaWin()
    {
        if (canYacha[now])
        {
            yachawin.Play();
            bgm1.Play();
            bgm2.Stop();
        }      
        canYacha[now] = false;
        anoEnabled[now] = false;
        fightButton.WalkMode();
        AnomalyObjArr[anoIndex[now]].transform.GetChild(0).gameObject.SetActive(false);
        mode = 0;
    }

    public void YachaLose()//이러면 평판까임
    {
        bgm1.Play();
        bgm2.Stop();
        yachalose.Play();
        canYacha[now] = false;
        anoEnabled[now] = false;
        fightButton.WalkMode();
        AnomalyObjArr[anoIndex[now]].transform.GetChild(0).gameObject.SetActive(false);
        mode = 0;
    }

    private void Awake()
    {
        SetupAnomalyIndices();
        SpawnAno();
        StartCoroutine("ChangeAno");
    }

    private void Update()
    {
        if (canYacha[now])//야차뜰수있으면 야차버튼 활성화
        {
            FButton.SetActive(true);
        }
        else
        { 
            FButton.SetActive(false); 
        }

        if (anoEnabled[now]&& anoArr[now]==1)
        {
            CButton.SetActive(true);
        }
        else
        {
            CButton.SetActive(false); 
        }
        if (Keyboard.current.aKey.wasPressedThisFrame && now != 0 && now != 11)
        {
            footstep.Play();
            now--;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame && now!=10&&now!=21)
        {
            footstep.Play();
            now++;
        }
    }

    public void StartFight()//야차 시작하면 호출. 이때부터 상대가 다가오거나 이동함
    {
        bgm1.Stop();
        bgm2.Play();
        int objIndex = anoIndex[now] % 100;
        AnomalyObjArr[objIndex].transform.GetChild(0).GetComponent<MoveAno>().Move();
        AnomalyObjArr[objIndex].transform.GetChild(0).GetComponent<HitButton>().GlitchStart();
        mode = 3;
    }

    IEnumerator ChangeAno()//8초마다 호출되며 이상현상을 발생시킴.
    {
        yield return new WaitForSeconds(timer);
        Warning.SetActive(true);//화면에 경고표시
        spawnsound.Play();
        // [추가] 1. 스폰 가능한 방이 있는지 먼저 확인
        bool isRoomAvailable = false;
        for (int i = 1; i <= 20; i++)
        {
            if (anoArr[i] != 99 && anoIndex[i] != 99 && !anoEnabled[i] && anoIndex[i] < 100)
            {
                isRoomAvailable = true;
                break;
            }
        }

        // [추가] 2. 빈 방이 없으면 무한 루프에 들어가지 않고 다음 사이클로 넘김
        if (!isRoomAvailable)
        {
            Warning.SetActive(false);
            StartCoroutine("ChangeAno");
            yield break;
        }

        int rand;
        while (true)
        {
            rand = Random.Range(1, 21);
            if (anoArr[rand] != 99 && anoIndex[rand] != 99 && !anoEnabled[rand] && anoIndex[rand] < 100)
            {
                break;
            }
        }

        anoEnabled[rand] = true;//괴물이 그 방에 있음
        int objIndex = anoIndex[rand];
        AnomalyObjArr[objIndex].transform.GetChild(0).gameObject.SetActive(true);//그 이상현상 나오게

        if (anoArr[rand] == 2)
        {
            canYacha[rand] = true;//그 방에서 야차뜰수 있으면 true
        }

        yield return new WaitForSeconds(0.8f);
        Warning.SetActive(false);
        StartCoroutine("ChangeAno");
    }
}

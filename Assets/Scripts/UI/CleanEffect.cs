using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class CleanEffect : MonoBehaviour
{
    [Header("생성할 프리팹")]
    [SerializeField] private GameObject prefabToSpawn;

    [Header("카메라 설정")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool is2D = true;

    [Header("청소 동작 설정")]
    [SerializeField] private float cleanDuration = 0.5f; // 청소 소요 시간(초)
    [SerializeField] private float cleanRadius = 0.5f;   // 닦아내는 원의 크기(반지름)
    [SerializeField] private GameManager gameManager;

    private bool isCleaning = false; // 중복 클릭 방지 플래그

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void OnDisable()
    {
        isCleaning = false;
    }

    // UI 또는 2D 콜라이더 클릭 시 실행되는 함수
    public void OnMouseDown()
    {
        if (prefabToSpawn == null || targetCamera == null) return;
        if (gameManager.mode != 2) return; // 청소 모드가 아니면 취소
        if (isCleaning) return;            // 이미 청소 중이면 중복 클릭 방지

        isCleaning = true;
        // 1. 마우스 위치 가져오기 (Input System)
        Vector3 mouseScreenPos = Vector3.zero;
        if (Mouse.current != null)
        {
            mouseScreenPos = Mouse.current.position.ReadValue();
        }

        // 2. 월드 좌표 변환
        if (is2D)
        {
            mouseScreenPos.z = Mathf.Abs(targetCamera.transform.position.z);
        }
        else
        {
            mouseScreenPos.z = 10f;
        }

        Vector3 centerWorldPos = targetCamera.ScreenToWorldPoint(mouseScreenPos);

        // 3. 손 프리팹 생성
        GameObject spawnedObj = Instantiate(prefabToSpawn, centerWorldPos, Quaternion.identity);

        // 4. [수정] GameManager의 주체로 코루틴을 실행하여 이 이상현상이 꺼져도 손이 정상적으로 닦고 사라지게 함
        gameManager.StartCoroutine(CleanMotionAndDestroy(spawnedObj, centerWorldPos, cleanDuration, cleanRadius));
    }

    private IEnumerator CleanMotionAndDestroy(GameObject targetObj, Vector3 centerPos, float duration, float radius)
    {
        float elapsedTime = 0f;
        Quaternion fixedRotation = targetObj.transform.rotation; // 자체 회전각 고정

        while (elapsedTime < duration)
        {
            if (targetObj == null) yield break;

            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration; // 0.0 ~ 1.0
            float angle = progress * Mathf.PI * 2f;   // 0 ~ 360도 (라디안)

            // 원형 궤적 계산
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            targetObj.transform.position = centerPos + new Vector3(x, y, 0f);
            targetObj.transform.rotation = fixedRotation;

            yield return null;
        }

        // 1. 닦기 동작 완료 후 손 프리팹 파괴
        if (targetObj != null)
        {
            Destroy(targetObj);
        }

        // 2. [수정] 청소가 모두 끝난 후 게임 승리 처리 및 이상현상 오브젝트 비활성화
        gameManager.YachaWin();
        gameObject.SetActive(false);
        isCleaning = false;
    }
}
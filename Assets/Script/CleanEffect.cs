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

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    // UI 버튼 On Click()에 연결할 함수
    public void SpawnObjectAtMouse()
    {
        if (prefabToSpawn == null || targetCamera == null) return;

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

        // 3. 오브젝트 생성 (최초 위치는 마우스 중심)
        GameObject spawnedObj = Instantiate(prefabToSpawn, centerWorldPos, Quaternion.identity);

        // 4. 궤적 회전(청소) 코루틴 실행
        StartCoroutine(CleanMotionAndDestroy(spawnedObj, centerWorldPos, cleanDuration, cleanRadius));
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

            // 마우스 중심점(centerPos) 기준으로 삼각함수를 사용해 원형 궤적 계산
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // 위치만 원형으로 이동시키고, 회전값은 고정
            targetObj.transform.position = centerPos + new Vector3(x, y, 0f);
            targetObj.transform.rotation = fixedRotation;

            yield return null;
        }

        // 청소 완료 후 파괴
        Destroy(targetObj);
    }
}
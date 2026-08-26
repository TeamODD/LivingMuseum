using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    public string gameSceneName = "MainGame";
    public AudioSource click;

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        Time.timeScale = 1f;

        if (click != null && click.clip != null)
        {
            click.Play();
            // 오디오 클립의 재생 길이만큼 대기 (시간 정지 상태 영향 받지 않도록 Realtime 사용)
            yield return new WaitForSecondsRealtime(0.3f);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator QuitGameRoutine()
    {
        if (click != null && click.clip != null)
        {
            click.Play();
            yield return new WaitForSecondsRealtime(0.3f);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
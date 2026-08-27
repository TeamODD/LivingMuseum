using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneController : MonoBehaviour
{
    public string gameSceneName = "MainGame";
    public AudioSource restartSfx;

    public void Retry()
    {
        StartCoroutine("RetryCo");
    }

    IEnumerator RetryCo()
    {
        restartSfx.Play();
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}

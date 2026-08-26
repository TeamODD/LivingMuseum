using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject manual;

    bool isClosed;

    void Awake()
    {
        Time.timeScale = 0f;

        if (manual != null)
            manual.SetActive(true);
    }

    public void CloseTutorial()
    {
        if (isClosed)
            return;

        isClosed = true;

        if (manual != null)
            manual.SetActive(false);

        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class RadioWarning : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI messageText;
    public AudioSource staticSound;

    float hiddenY = -280f;
    float shownY = 60f;
    public float slideDuration = 0.4f;
    public float holdDuration = 2.5f;

    Coroutine routine;

    void Start()
    {
        SetY(hiddenY);
    }

    public void Show(string message)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (staticSound != null)
            staticSound.Play();

        yield return Slide(hiddenY, shownY);
        yield return new WaitForSeconds(holdDuration);
        yield return Slide(shownY, hiddenY);

        routine = null;
    }

    IEnumerator Slide(float fromY, float toY)
    {
        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            SetY(Mathf.Lerp(fromY, toY, timer / slideDuration));
            yield return null;
        }

        SetY(toY);
    }

    void SetY(float y)
    {
        if (panel == null)
            return;

        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, y);
    }
}

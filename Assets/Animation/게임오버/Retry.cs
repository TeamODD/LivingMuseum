using UnityEngine;

public class Retry : MonoBehaviour
{
    [SerializeField] GameObject retryButton;
    public void ShowRetry()
    {
        retryButton.SetActive(true);
    }
}

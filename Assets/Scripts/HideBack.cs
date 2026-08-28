using UnityEngine;

public class HideBack : MonoBehaviour
{
    [SerializeField] AudioSource goSound;
    [SerializeField] AudioSource backSound;
    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void GoSound()
    {
        goSound.Play();
    }

    public void BackSound()
    {
        backSound.Play();
    }
}

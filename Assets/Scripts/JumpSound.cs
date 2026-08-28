using UnityEngine;

public class JumpSound : MonoBehaviour
{
    [SerializeField] private AudioSource jumpSound;

    // 애니메이션 이벤트에서 직접 호출할 함수
    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            jumpSound.Play();
        }
    }

    //public void ActiveFalse()
    //{
    //    gameObject.SetActive(false);
    //}
}

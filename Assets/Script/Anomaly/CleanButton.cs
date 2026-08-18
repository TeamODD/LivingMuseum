using System.ComponentModel;
using UnityEngine;
using UnityEngine.Video;

public class CleanButtion : MonoBehaviour
{
    [SerializeField] GameObject hands;
    [SerializeField] GameObject cleanHand;
    [SerializeField] bool isClean;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isClean = false;//플레이어가 가리기 모드면 true
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ClickClean()
    {
        if (isClean)
        {
            isClean = false;
        }
        else
        {
            isClean = true;
        }
    }
}

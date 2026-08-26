using System.ComponentModel;
using UnityEngine;
using UnityEngine.Video;

public class CleanButtion : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ClickClean()
    {
        gameManager.mode = 2;
    }
}

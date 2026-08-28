using UnityEngine;

public class ClickDead : MonoBehaviour
{
    [SerializeField] GameObject CleanButton;
    [SerializeField] GameObject PassButton;
    [SerializeField] GameManager gameManager;
    void Start()
    {
        Transform parentTransform = GameObject.Find("UICanvas").transform;
        CleanButton = parentTransform.Find("CleanButton")?.gameObject;
        PassButton = parentTransform.Find("PassButton")?.gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (gameManager.mode==0)
        {
            CleanButton.SetActive(true);
            PassButton.SetActive(true);
        }      
    }
}

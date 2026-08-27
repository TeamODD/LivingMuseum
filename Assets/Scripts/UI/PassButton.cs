using UnityEngine;

public class PassButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject fightButton;
    [SerializeField] GameObject cleanButton;
    [SerializeField] GameObject hideButton; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickPass()
    {
        fightButton.SetActive(false);
        cleanButton.SetActive(false);
        hideButton.SetActive(false);
        gameObject.SetActive(false);
    }
}

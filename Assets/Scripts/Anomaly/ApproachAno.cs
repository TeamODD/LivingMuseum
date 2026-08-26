using UnityEngine;
using DG.Tweening;

public class ApproachAno : MonoBehaviour
{
    [SerializeField] GameObject ApproachObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        
    }
    public void YachaStart()//다가오는 애 클릭하면
    {
        gameObject.SetActive(false);
        ApproachObj.SetActive(true);
        ApproachObj.transform.localScale = Vector3.one;
        ApproachObj.transform.DOScale(5, 10);
    }
}

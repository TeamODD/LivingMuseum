using UnityEngine;
using UnityEngine.UI;

public class ReputationUI : MonoBehaviour
{
    public ReputationSystem reputationSystem;
    public Image gaugeFill;
    public Image faceImage;

    public Sprite happyFace;
    public Sprite badFace;

    public Color greenColor = new Color(0.3f, 0.8f, 0.3f);
    public Color yellowColor = new Color(0.9f, 0.85f, 0.2f);
    public Color orangeColor = new Color(0.95f, 0.55f, 0.1f);
    public Color redColor = new Color(0.85f, 0.15f, 0.15f);
    public Color darkRedColor = new Color(0.35f, 0.02f, 0.02f);

    void OnEnable()
    {
        if (reputationSystem != null)
            reputationSystem.OnReputationChanged += Refresh;
    }

    void OnDisable()
    {
        if (reputationSystem != null)
            reputationSystem.OnReputationChanged -= Refresh;
    }

    void Start()
    {
        if (reputationSystem != null)
            Refresh(reputationSystem.Current);
    }

    void Refresh(int value)
    {
        if (gaugeFill != null)
        {
            gaugeFill.fillAmount = (float)value / reputationSystem.maxReputation;
            gaugeFill.color = GetGaugeColor(value);
        }

        if (faceImage != null)
            faceImage.sprite = GetFace(value);
    }

    Color GetGaugeColor(int value)
    {
        if (value >= 71)
            return greenColor;

        if (value >= 41)
            return yellowColor;

        if (value >= 11)
            return orangeColor;

        if (value >= 1)
            return redColor;

        return darkRedColor;
    }

    Sprite GetFace(int value)
    {
        if (value >= 41)
            return happyFace;

        return badFace;
    }
}

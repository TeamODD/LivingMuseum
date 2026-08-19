using UnityEngine;
using UnityEngine.UI;

public class AudienceGroup : MonoBehaviour
{
    AudienceSystem system;
    RoomZoneLayout zoneLayout;
    RectTransform rect;

    int headZone;
    float moveTimer;
    bool isFinished;

    public bool IsFinished
    {
        get { return isFinished; }
    }

    public void Setup(AudienceSystem system, RoomZoneLayout zoneLayout)
    {
        this.system = system;
        this.zoneLayout = zoneLayout;

        rect = GetComponent<RectTransform>();

        headZone = system.occupiedZoneCount - 1;

        rect.anchoredPosition = new Vector2(zoneLayout.GetZoneCenterX(headZone), system.personY);

        CreatePeople();
    }

    void Update()
    {
        if (isFinished)
            return;

        moveTimer += Time.deltaTime;
        if (moveTimer < system.moveDuration)
            return;

        moveTimer = 0f;
        headZone++;
        rect.anchoredPosition += new Vector2(zoneLayout.GetZoneWidth(), 0f);

        if (headZone >= zoneLayout.zoneCount - 1)
        {
            isFinished = true;
            Destroy(gameObject);
        }
    }

    public bool IsZoneOccupied(int zoneIndex)
    {
        if (isFinished)
            return false;

        return zoneIndex <= headZone && zoneIndex > headZone - system.occupiedZoneCount;
    }

    void CreatePeople()
    {
        float zoneWidth = zoneLayout.GetZoneWidth();

        for (int i = 0; i < system.occupiedZoneCount; i++)
        {
            float zoneOffsetX = -zoneWidth * i;
            int peopleCount = Random.Range(system.minPeoplePerZone, system.maxPeoplePerZone + 1);

            for (int p = 0; p < peopleCount; p++)
            {
                float offsetX = (p - (peopleCount - 1) * 0.5f) * system.personSpacing;
                CreatePerson(new Vector2(zoneOffsetX + offsetX, 0f));
            }
        }
    }

    void CreatePerson(Vector2 localPosition)
    {
        GameObject obj = new GameObject("Audience");
        obj.layer = gameObject.layer;
        obj.transform.SetParent(transform, false);

        RectTransform personRect = obj.AddComponent<RectTransform>();
        personRect.sizeDelta = system.personSize;
        personRect.anchoredPosition = localPosition;

        Image image = obj.AddComponent<Image>();
        image.raycastTarget = false;
        image.sprite = system.GetRandomCrowdSprite();
        image.preserveAspect = true;
    }
}

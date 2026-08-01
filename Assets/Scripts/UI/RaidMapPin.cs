using UnityEngine;
using UnityEngine.UI;

public class RaidMapPin : MonoBehaviour
{
    public int VillageId;

    [Header("핀 스프라이트")]
    public Sprite normalSprite;
    public Sprite selectedSprite;

    private Image pinImage;

    public System.Action<int> OnPinClicked;

    void Start()
    {
        pinImage = GetComponent<Image>();
        GetComponent<Button>()?.onClick.AddListener(() => OnPinClicked?.Invoke(VillageId));
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (pinImage != null)
            pinImage.sprite = isSelected ? selectedSprite : normalSprite;
    }
}
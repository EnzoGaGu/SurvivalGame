using UnityEngine;
using TMPro;

public class InventorySlotDraggable : MonoBehaviour
{
    public UnityEngine.UI.Image icon;

    public void SetItem(Sprite itemSprite)
    {
        icon.sprite = itemSprite;

        //Change icon opacity
        Color iconColor = icon.color;
        iconColor.a = 1f;
        icon.color = iconColor;

        //Change background opacity
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f; // 50%
        }
    }
}

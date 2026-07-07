using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private RectTransform rectTransform;
    private Image buttonImage;
    private Vector3 originalScale;
    private Color originalColor;

    [Header("Scale Settings")]
    public float scaleFactor = 1.12f;
    public float duration = 0.2f;

    [Header("Color Accent Settings")]
    [Tooltip("The color the button changes to when highlighted/selected")]
    public Color selectedColor = new Color(0.5f, 0.2f, 0.2f, 1f); // Soft, stylish crimson red


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();

        originalScale = rectTransform.localScale;

        // Grab the starting color of the button image
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
        else
        {
            originalColor = Color.white;
        }
    }

    // ==========================================
    // ACTION: BUTTON SELECTED / HOVERED
    // ==========================================
    public void OnSelect(BaseEventData eventData)
    {
        // FIXED: Instead of repeating old code here, we call the fixed method!
        AnimateSelection();
    }

    // ==========================================
    // ACTION: BUTTON DESELECTED / MOUSE LEAVE
    // ==========================================
    public void OnDeselect(BaseEventData eventData)
    {
        ResetButtonVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Force the Unity EventSystem to recognize this hover as a selection
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Clear focus if mouse leaves UI completely
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // ==========================================
    // HELPER FUNCTIONS FOR ANIMATION LOGIC
    // ==========================================
    private void AnimateSelection()
    {
        rectTransform.DOKill();
        // Added .SetUpdate(true) so it works when the reward screen freezes time
        rectTransform.DOScale(originalScale * scaleFactor, duration).SetEase(Ease.OutBack).SetUpdate(true);

        if (buttonImage != null)
        {
            buttonImage.DOKill();
            buttonImage.DOColor(selectedColor, duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        // Dim all OTHER sibling buttons in the same menu panel
        DimOtherButtons();
    }

    private void DimOtherButtons()
    {
        Transform parentContainer = transform.parent;
        if (parentContainer == null) return;

        foreach (Transform child in parentContainer)
        {
            // Skip ourselves
            if (child == transform) continue;

            MenuButtonEffects neighborEffects = child.GetComponent<MenuButtonEffects>();

            if (neighborEffects == null)
                continue;

            Image neighborImage = child.GetComponent<Image>();

            if (neighborImage == null)
                continue;
        }
    }

    private void ResetButtonVisuals()
    {
        rectTransform.DOKill();
        rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);

        if (buttonImage != null)
        {
            buttonImage.DOKill();
            buttonImage.DOColor(originalColor, duration).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        // FIXED: Restore neighbors back to their normal look when this button is deselected
        RestoreOtherButtons();
    }

    private void RestoreOtherButtons()
    {
        Transform parentContainer = transform.parent;
        if (parentContainer == null) return;

        foreach (Transform child in parentContainer)
        {
            if (child == transform)
                continue;

            MenuButtonEffects neighborEffects = child.GetComponent<MenuButtonEffects>();

            // Skip everything that isn't another menu button
            if (neighborEffects == null)
                continue;

            Image neighborImage = child.GetComponent<Image>();

            if (neighborImage == null)
                continue;

            neighborImage.DOKill();

            neighborImage.DOColor(
                neighborEffects.originalColor,
                duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
    }

    private void OnDisable()
    {
        // Clean cleanup if a canvas panel transitions off-screen mid-tween
        rectTransform.DOKill();
        rectTransform.localScale = originalScale;
        if (buttonImage != null)
        {
            buttonImage.DOKill();
            buttonImage.color = originalColor;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class UIHoverEffect :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
   
{

    [Header("Scale")]
    public float hoverScale = 1.1f;
    public float duration = 0.15f;

    [Header("Glow")]
    public GameObject glowObject;

    [Header("Rotation")]
    public bool rotateGlow = true;
    public float rotateSpeed = 100f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSFX;
    public AudioClip clickSFX;

    private Vector3 originalScale;
    private Tween scaleTween;

    private Button childButton;

    private void Start()
    {
        originalScale = transform.localScale;

        transform
        .DOLocalMoveY(
            transform.localPosition.y + 5f,
            1.5f
        )
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine)
        .SetUpdate(true);

        if (glowObject != null)
            glowObject.SetActive(false);

        // FIND CHILD BUTTON
        childButton = GetComponentInChildren<Button>();

        if (childButton != null)
        {
            childButton.onClick.AddListener(PlayClickEffect);
        }
    }

    private void Update()
    {
        if (
            rotateGlow &&
            glowObject != null &&
            glowObject.activeSelf
        )
        {
            glowObject.transform.Rotate(
                0,
                0,
                rotateSpeed * Time.unscaledDeltaTime
            );
        }
    }

    // HOVER ENTER
    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTween?.Kill();

        scaleTween =
            transform.DOScale(
                originalScale * hoverScale,
                duration
            )
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (glowObject != null)
            glowObject.SetActive(true);

        // PLAY HOVER SOUND
        if (hoverSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSFX);
        }
    }

    // HOVER EXIT
    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween?.Kill();

        scaleTween =
            transform.DOScale(
                originalScale,
                duration
            )
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        if (glowObject != null)
            glowObject.SetActive(false);
    }

    void PlayClickEffect()
    {
        Debug.Log("CLICKED");

        scaleTween?.Kill();

        transform
            .DOPunchScale(
                Vector3.one * 0.15f,
                0.2f,
                8,
                0.5f
            )
            .SetUpdate(true);

        // PLAY CLICK SOUND
        if (clickSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }
    }
}
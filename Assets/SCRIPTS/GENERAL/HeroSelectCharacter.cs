using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroSelectCharacter : MonoBehaviour
{
    [Header("Hero")]
    public HeroDefinition heroDefinition;

    [Header("Movement")]
    public float moveDistance = 0.8f;
    public float moveDuration = 0.4f;

    [Header("Selection VFX")]
    [SerializeField] private GameObject selectionVFXPrefab;
    [SerializeField] private Transform vfxSpawnPoint;

    private Animator animator;

    private Vector3 idlePosition;
    private Vector3 selectedPosition;

    private bool isSelected;

    private HeroSelectManager manager;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        idlePosition = transform.position;
        selectedPosition = idlePosition + transform.forward * moveDistance;

        manager = FindFirstObjectByType<HeroSelectManager>();
    }

    private void OnMouseDown()
    {
        manager.SelectCharacter(this);
    }

    public void Select()
    {
        if (isSelected)
            return;

        isSelected = true;

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsSelected", false);

        transform.DOMove(selectedPosition, moveDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsSelected", true);
            });
    }

    public void SpawnSelectVFX()
    {
        if (selectionVFXPrefab == null || vfxSpawnPoint == null)
            return;

        GameObject vfx = Instantiate(
            selectionVFXPrefab,
            vfxSpawnPoint.position,
            vfxSpawnPoint.rotation
        );

        // Automatically destroy after all particles have finished
        Destroy(vfx, 5f);
    }

    public void Deselect()
    {
        if (!isSelected)
            return;

        isSelected = false;

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsSelected", false);

        transform.DOMove(idlePosition, moveDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                animator.SetBool("IsWalking", false);
            });
    }
}
using UnityEngine;

public class ChestReward : MonoBehaviour
{
    [Header("Settings")]
    public float activationRadius = 2f;

    [Header("Optional")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip openSound;
    public GameObject openVFX;

    private bool opened = false;

    // Shared between all chests in the current level
    private static bool rewardAlreadyClaimed = false;

    private Transform player;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }

    void Update()
    {
        if (opened)
            return;

        if (rewardAlreadyClaimed)
            return;

        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance <= activationRadius)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        opened = true;
        rewardAlreadyClaimed = true;

        Debug.Log("Chest Opened!");

        if (animator != null)
            animator.SetTrigger("Open");

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        if (openVFX != null)
            Instantiate(openVFX, transform.position, Quaternion.identity);

        RewardUI.Instance.ShowRewards(
            RewardManager.Instance.GenerateRewards(3));
    }

    public static void ResetTreasureRoom()
    {
        rewardAlreadyClaimed = false;
    }
}
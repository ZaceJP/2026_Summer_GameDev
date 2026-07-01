using UnityEngine;


public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    static readonly int PrimaryHash =
        Animator.StringToHash("PrimaryAttack");

    static readonly int SecondaryHash =
        Animator.StringToHash("SecondaryAttack");

    static readonly int Skill1Hash =
        Animator.StringToHash("Skill1");

    static readonly int Skill2Hash =
        Animator.StringToHash("Skill2");

    static readonly int DeathHash =
        Animator.StringToHash("Death");

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("No Animator found in children!");
    }

    public void SetMoving(bool moving)
    {
        animator.SetBool(IsMovingHash, moving);
    }

    public void PlayPrimaryAttack()
    {
        animator.SetTrigger(PrimaryHash);
    }

    public void PlaySecondaryAttack()
    {
        animator.SetTrigger(SecondaryHash);
    }

    public void PlaySkill1()
    {
        animator.SetTrigger(Skill1Hash);
    }

    public void PlaySkill2()
    {
        animator.SetTrigger(Skill2Hash);
    }

    public void PlayDeath()
    {
        animator.SetTrigger(DeathHash);
    }
}
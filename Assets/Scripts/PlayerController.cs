using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : EntityController
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private MovementHandler move;

    [Header("Gameplay Numbers")]
    [SerializeField] private float parryWindow;
    [Header("Animation Curves")]
    [SerializeField] private float attackSpeed;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private float specialSpeed;
    [SerializeField] private AnimationCurve specialCurve;
    [SerializeField] private float dashSpeed;
    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private float backdashSpeed;
    [SerializeField] private AnimationCurve backdashCurve;
    [SerializeField] private float deflectSpeed;
    [SerializeField] private AnimationCurve deflectCurve;
    [SerializeField] private float heavyDeflectSpeed;
    [SerializeField] private AnimationCurve heavyDeflectCurve;

    private InputHandler input; 
    private bool guarding, acting, moving, forceGuarding;
    private bool isGuarding => guarding || forceGuarding;
    private bool cancellable;
    private int attackChainIndex;
    private int facing = 1;
    private float nextParry;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputHandler.Instance;
    }

    void FixedUpdate() {
        HandleInputs();
    }

    void HandleInputs() {

        if (input.menu.pressed)
        {
            SceneManager.LoadScene("MenuScene");
        }

        guarding = false;
        moving = false;

        if (!acting && input.guard.released)
        {
            move.ForceStop();
        }

        if (!acting && !input.guard.down) {
            if (input.move.pressed) {
                UpdateFlip();
                move.StartAcceleration(input.dir);
                moving = true;
            } else if (input.move.down)
            {
                UpdateFlip();
                move.UpdateMovement(input.dir);
                moving = true;
            } else if (input.move.released)
            {
                move.StartDeceleration();
            }
        }

        if ((!acting || cancellable) && input.special.pressed)
        {
            StartAction();
            move.OverrideCurve(specialSpeed, specialCurve, facing);
            if (input.guard.down)
            {
                animator.SetInteger("attackId", 1);
            } else
            {
                animator.SetInteger("attackId", 0);
            }
            VFXManager.Instance.CreateVFX(VFXType.DUST_LARGE,
                     transform.position, facing == -1);
            animator.SetTrigger("onspecial");
        }
        else if ((!acting || cancellable) && input.attack.pressed)
        {
            if (!cancellable)
                attackChainIndex = 0;

            StartAction();
            
            if (attackChainIndex != 1 && input.move.down) {
                move.OverrideCurve(attackSpeed, attackCurve, facing);

                VFXManager.Instance.CreateVFX(VFXType.DUST_SMALL,
                     transform.position, facing == -1);
            }

            animator.SetInteger("attackId", attackChainIndex);
            animator.SetTrigger("onattack");

            attackChainIndex = (attackChainIndex + 1) % 3;
        }
        else if ((!acting || cancellable) && input.dash.pressed)
        {
            StartAction();
            if (input.dir == 0)
            {
                VFXManager.Instance.CreateVFX(VFXType.DUST_LARGE,
                     transform.position + facing * Vector3.right, facing == 1);
                move.OverrideCurve(backdashSpeed, backdashCurve, -facing);
                animator.SetTrigger("backdash");
            } else
            {
                VFXManager.Instance.CreateVFX(VFXType.DUST_SMALL,
                     transform.position, facing == -1);
                move.OverrideCurve(dashSpeed, dashCurve, facing);
                animator.SetTrigger("dash");
            }
        }
        else if (!acting && input.guard.down)
        {
            guarding = true;
            if (move.IsMoving())
                move.StartDeceleration();
        }

        if (input.guard.pressed)
        {
            AttemptParry();
        }

        animator.SetBool("guarding", guarding);
        animator.SetBool("moving", moving && !acting && !isGuarding);
    }

    public override bool OnHit()
    {
        if (isGuarding)
        {
            animator.SetBool("guarding", false);
            animator.SetBool("moving", false);
            if (Time.time <= nextParry)
            {
                animator.SetInteger("deflectWeight", 1);
                move.OverrideCurve(heavyDeflectSpeed, heavyDeflectCurve, -facing);
                VFXManager.Instance.CreateVFX(VFXType.DUST_SMALL,
                     transform.position, facing == 1);
            } else
            {
                animator.SetInteger("deflectWeight", 0);
                move.OverrideCurve(deflectSpeed, deflectCurve, -facing);
                cancellable = true;
            }
            VFXManager.Instance.CreateVFX(VFXType.HITSPARK_SMALL, transform.position + facing * Vector3.right, facing == -1);
            animator.SetTrigger("ondeflect");
            acting = true;
            forceGuarding = true;
        }

        return true;
    }

    public void UpdateFlip()
    {
        if (input.dir != 0)
        {
            sprite.flipX = input.dir < 0;

            facing = input.dir < 0 ? -1 : 1;
        }
    }

    public void EndAction()
    {
        move.ResetCurves();
        move.ForceStop();
        acting = false;
        cancellable = false;
        forceGuarding = false;
        attackChainIndex = 0;

        // allow for snap reguard after animations
        if (!acting && input.guard.down)
        {
            guarding = true;
            if (move.IsMoving())
                move.StartDeceleration();
            animator.SetBool("guarding", guarding);
        }
    }

    private void AttemptParry()
    {
        nextParry = Time.time + parryWindow;
    }

    public void SetCancellable()
    {
        cancellable = true;
    }
    public void CreateAttack()
    {
        
    }
    private void StartAction()
    {
        move.ResetCurves();
        move.StartDeceleration();
        UpdateFlip();
        acting = true;
        cancellable = false;
        forceGuarding = false;
    }

}

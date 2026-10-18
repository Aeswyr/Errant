using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    
    private InputHandler input; 
    private bool guarding, acting, moving, forceGuarding;
    private bool isGuarding => guarding || forceGuarding;
    private bool cancellable;
    private int attackChainIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputHandler.Instance;
    }

    void FixedUpdate() {
        HandleInputs();
    }

    void HandleInputs() {
        guarding = false;
        moving = false;

        if (!acting && input.move.down) {
            UpdateFlip();
            moving = true;
        }

        if ((!acting || cancellable) && input.special.pressed)
        {
            StartAction();
            if (input.guard.down)
            {
                animator.SetInteger("attackId", 1);
            } else
            {
                animator.SetInteger("attackId", 0);
            }
            animator.SetTrigger("onspecial");
        }
        else if ((!acting || cancellable) && input.attack.pressed)
        {
            if (!cancellable)
                attackChainIndex = 0;
            StartAction();

            animator.SetInteger("attackId", attackChainIndex);
            animator.SetTrigger("onattack");

            attackChainIndex = (attackChainIndex + 1) % 3;
        }
        else if ((!acting || cancellable) && input.dash.pressed)
        {
            StartAction();
            if (input.dir == 0)
            {
                animator.SetTrigger("backdash");
            } else
            {
                animator.SetTrigger("dash");
            }
        }
        else if (!acting && input.guard.down)
        {
            guarding = true;
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
            // deflect weight 1 if the deflect would stance break an opponent
            animator.SetInteger("deflectWeight", 0);
            animator.SetTrigger("ondeflect");
            
            acting = true;
            cancellable = true;
            forceGuarding = true;
        }

        return true;
    }

    public void UpdateFlip()
    {
        if (input.dir != 0)
        {
            sprite.flipX = input.dir < 0;
        }
    }

    public void EndAction()
    {
        acting = false;
        cancellable = false;
        forceGuarding = false;
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
        UpdateFlip();
        acting = true;
        cancellable = false;
        forceGuarding = false;
    }

}

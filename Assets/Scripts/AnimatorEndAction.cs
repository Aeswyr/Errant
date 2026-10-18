using UnityEngine;

public class AnimatorEndAction : StateMachineBehaviour
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.GetComponentInParent<PlayerController>().EndAction();
    }  
}

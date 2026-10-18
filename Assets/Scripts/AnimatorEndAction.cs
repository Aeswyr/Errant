using UnityEngine;

public class AnimatorEndAction : StateMachineBehaviour
{
    private PlayerController player;
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (player == null)
            player = animator.GetComponentInParent<PlayerController>();
        player.EndAction();
    }  
}

using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    
    public void SetCancellable()
    {
        player.SetCancellable();
    }

    public void CreateAttack()
    {
        player.CreateAttack();
    }
}

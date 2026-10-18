using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityEvent onHit;
    [SerializeField] private EntityController owner;

    void OnTriggerEnter2D(Collider2D collision)
    {
        onHit?.Invoke();
        if (owner.OnHit())
            collision.GetComponent<HitboxController>().OnHit();
    }
}
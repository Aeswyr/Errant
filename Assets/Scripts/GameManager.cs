using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject arrow;

    private float nextArrow = 4f;
    void FixedUpdate()
    {
        if (Time.time > nextArrow)
        {
            Instantiate(arrow, 16 * Vector3.right, Quaternion.identity);
            nextArrow = Time.time + 6;
        }
    }

    public Transform GetObjectParent()
    {
        return transform;
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    private float nextArrow = 4f;
    void FixedUpdate()
    {
        if (Time.time > nextArrow)
        {
            Instantiate(arrow, 16 * Vector3.right, Quaternion.identity);
            nextArrow = Time.time + 10;
        }
    }
}

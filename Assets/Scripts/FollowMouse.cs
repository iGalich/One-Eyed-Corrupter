using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    Vector3 mousePosition;
    Rigidbody2D rb;
    Vector2 position = new Vector2(0f, 0f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        position = transform.position;
    }
    private void Update()
    {
        mousePosition = Input.mousePosition;
        //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        position = Vector2.MoveTowards(transform.position, mousePosition, moveSpeed);
    }
    private void FixedUpdate()
    {
        rb.MovePosition(position);
    }
}

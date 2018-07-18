using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float jetpack;

    private Rigidbody2D rb;
    private CircleCollider2D feet;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        feet = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        bool skiing = Input.GetKey(KeyCode.Space);
        bool flying = Input.GetKey(KeyCode.W);

        if (skiing || moveHorizontal != 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        } else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if (moveHorizontal != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position + feet.offset, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(rb.position + feet.offset, Vector2.down * 0.2f, Color.yellow, 1, false);

            if (hit.collider == null)
            {
                rb.AddForce(new Vector2(moveHorizontal * speed / (rb.velocity.x * moveHorizontal < 0 ? 1 : Mathf.Max(1, Mathf.Abs(rb.velocity.x))), 0));
            } else if (!skiing)
            {
                rb.velocity = new Vector2(moveHorizontal * speed / 2, rb.velocity.y);
            }
        }

        if (flying)
        {
            rb.AddForce(new Vector2(0, jetpack));
        }
    }

}
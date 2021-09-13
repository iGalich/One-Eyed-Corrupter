using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{
    protected BoxCollider2D boxCollider;

    protected Vector3 moveDelta;

    protected RaycastHit2D hit;

    [SerializeField] protected float moveSpeed = 1.0f;
    //[SerializeField] protected float xSpeed = 1.0f;

    protected override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset moveDelta
        moveDelta = new Vector3(input.x * moveSpeed, input.y * moveSpeed, 0);

        Vector3 characterScale = transform.localScale;
        if (moveDelta.x < 0 && characterScale.x > 0)
            characterScale.x *= -1;
        if (moveDelta.x > 0)
            characterScale.x = Mathf.Abs(characterScale.x);
        transform.localScale = characterScale;

        // add push vector, if any
        
        //moveDelta += pushDirection;

        //reduce pushforce everyframe, based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, GetPushRecoverySpeed());


        //makes sure we can move in this direction, by casting a box there first, if the box returns null, we're free to move 
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // Movement
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // Movement
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        }
    }

    
}
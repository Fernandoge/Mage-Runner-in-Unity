using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpSpeed;
    public int totalMana;
    public bool isGrounded;
    public Transform feetPos;
    public PlayerSpellsData spellsData;

    [System.NonSerialized]
    public Rigidbody2D rigidBody;
    [System.NonSerialized]
    public Animator animator;
    [System.NonSerialized]
    public int spellsAmount;
    [System.NonSerialized]
    public float originalGravity;
    [System.NonSerialized]
    public bool isGliding;
    [System.NonSerialized]
    public float glideSpeed;

    private PlayerSpells _spells;
    private Vector2 _beginTouchPosition, _endTouchPosition;

    private void Start()
    {
        _spells = new PlayerSpells(this);
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalGravity = rigidBody.gravityScale;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, 0.01f);
        if (isGrounded && rigidBody.velocity.y < 0)
        {
            isGliding = false;
            rigidBody.gravityScale = originalGravity;
            animator.SetBool("Running", true);
            animator.SetBool("FastFall", false);
        }
       
        if (isGliding && rigidBody.velocity.y < 0)
        {
            animator.SetBool("HighJump", false);
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.down * glideSpeed;
        }

        #if UNITY_EDITOR
            PlayerInputDebug();
        #else
            PlayerInput();
        #endif
    }

    public void PlayerInputDebug()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _beginTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _endTouchPosition = Input.mousePosition;

            if (_beginTouchPosition == _endTouchPosition)
                Jump();
         // else
             // gesture handler starts to recognize draw
        }
    }

    public void PlayerInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _beginTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    _endTouchPosition = touch.position;

                    if (_beginTouchPosition == _endTouchPosition)
                        Jump();
                 // else
                     // gesture handler starts to recognize draw
                    break;
            }
        }
    }

    public void Jump()
    {
        if (isGrounded == true)
        {
            rigidBody.velocity = Vector2.up * jumpSpeed;
        }
    }

    public void BeginCastingSpell(string id)
    {
        _spells.CastSpell(id);
    }

    public void CastingSpellFailed()
    {
        print("Casting spell failed");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float initialJumpSpeed;
    public float maxJumpSpeed;
    public float jumpBoost;
    public int totalMana;
    public Transform feetPos;
    public PlayerSpellsData spellsData;
    public GameObject jumpPowerIndicator;

    [System.NonSerialized]
    public Rigidbody2D rigidBody;
    [System.NonSerialized]
    public Animator animator;
    [System.NonSerialized]
    public int spellsAmount;
    [System.NonSerialized]
    public float originalGravity;
    [System.NonSerialized]
    public bool isGrounded;
    [System.NonSerialized]
    public bool isGliding;
    [System.NonSerialized]
    public float glideSpeed;

    private float originalJumpSpeed;
    private PlayerSpells _spells;
    private bool _jumpCancelled;
    private Vector2 _beginTouchPosition, _endTouchPosition;

    private void Start()
    {
        _spells = new PlayerSpells(this);
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalGravity = rigidBody.gravityScale;
        originalJumpSpeed = initialJumpSpeed;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, 0.22f);
        if (!isGrounded)
        {
            initialJumpSpeed = originalJumpSpeed;
            if (jumpPowerIndicator.activeSelf == true)
            {
                jumpPowerIndicator.SetActive(false);
            }
        }
        else if (isGrounded && rigidBody.velocity.y <= 0)
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
            _jumpCancelled = false;
            initialJumpSpeed = originalJumpSpeed;
            _beginTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _endTouchPosition = Input.mousePosition;

            if (_beginTouchPosition != _endTouchPosition)
            {
                jumpPowerIndicator.SetActive(false);
                _jumpCancelled = true;
            }
            else if (initialJumpSpeed < maxJumpSpeed && isGrounded)
            {
                initialJumpSpeed += jumpBoost * Time.deltaTime; 
                UpdateJumpIndicator();
            }
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
                    _jumpCancelled = false;
                    initialJumpSpeed = originalJumpSpeed;
                    _beginTouchPosition = touch.position;
                    break;

                case TouchPhase.Stationary:
                    if (initialJumpSpeed < maxJumpSpeed && isGrounded)
                    {
                        initialJumpSpeed += jumpBoost * Time.deltaTime;
                        UpdateJumpIndicator();
                    }
                    break;

                case TouchPhase.Moved:
                    _jumpCancelled = true;
                    jumpPowerIndicator.SetActive(false);
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
            rigidBody.velocity = Vector2.up * initialJumpSpeed;
            jumpPowerIndicator.SetActive(false);
        }
    }

    public void UpdateJumpIndicator()
    {
        if (_jumpCancelled)
            return;
        
        float jumpDifference = (maxJumpSpeed - originalJumpSpeed) * 1/3;
        if (initialJumpSpeed > originalJumpSpeed + jumpDifference * 3)
            jumpPowerIndicator.transform.localScale = new Vector2(10f, 10f);
        else if (initialJumpSpeed > originalJumpSpeed + jumpDifference * 5/3)
            jumpPowerIndicator.transform.localScale = new Vector2(6f, 6f);
        else if (initialJumpSpeed > originalJumpSpeed + jumpDifference * 1/2)
        {
            jumpPowerIndicator.transform.localScale = new Vector2(3f, 3f);
            jumpPowerIndicator.SetActive(true);
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

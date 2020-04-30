﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float initialJumpSpeed;
    public float maxJumpSpeed;
    public float jumpBoost;
    public int totalMana;
    public Transform feetPos;
    public GameObject spellShooter;
    public GameObject jumpPowerIndicator;
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
    public bool isGrounded;
    [System.NonSerialized]
    public bool isGliding;
    [System.NonSerialized]
    public float glideSpeed;
    [System.NonSerialized]
    public bool readyToShoot;

    private float originalJumpSpeed;
    private GestureSpells _gestureSpells;
    private bool _jumpCancelled;
    private Vector2 _beginTouchPosition, _endTouchPosition;

    private void Start()
    {
        _gestureSpells = new GestureSpells(this);
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
            animator.SetBool("Glide", false);
        }
       
        if (isGliding && rigidBody.velocity.y < 0)
        {
            animator.SetBool("HighJump", false);
            animator.SetBool("Glide", true);
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
            if (readyToShoot)
            {
                ShootSpell(Input.mousePosition);
            }
            else
            {
                _jumpCancelled = false;
                initialJumpSpeed = originalJumpSpeed;
                _beginTouchPosition = Input.mousePosition;
            }
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
                    if (readyToShoot)
                    {
                        ShootSpell(touch.position);
                    }
                    else
                    {
                        _jumpCancelled = false;
                        initialJumpSpeed = originalJumpSpeed;
                        _beginTouchPosition = touch.position;
                    }
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

    public void ShootSpell(Vector3 shootPosition)
    {
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(shootPosition) - spellShooter.transform.position;
        float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        float rotationZ = lookAngle - 90f;
        if (rotationZ > 0 || rotationZ < -180)
        {
            float newZ = rotationZ > 0 ? 0 : -180;
            spellShooter.transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
        else
        {
            spellShooter.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
        _gestureSpells.ShootSpell();
    }

    public void BeginCastingSpell(string id)
    {
        _gestureSpells.CastSpell(id);
    }

    public void CastingSpellFailed()
    {
        print("Casting spell failed");
    }
}

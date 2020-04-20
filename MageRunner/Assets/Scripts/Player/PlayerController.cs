using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpSpeed;
    public float jumpTime;
    public int totalMana;
    public bool isGrounded;
    public Transform feetPos;
    public PlayerSpellsData spellsData;

    [System.NonSerialized]
    public int spellsAmount;

    private Rigidbody2D _rigidBody;
    private PlayerSpells _spells;
    private Vector2 _beginTouchPosition, _endTouchPosition;

    private void Start()
    {
        _spells = new PlayerSpells(this);
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, 0.01f);

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
            _rigidBody.velocity = Vector2.up * jumpSpeed;
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

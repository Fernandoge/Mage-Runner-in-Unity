using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float jumpTime;
    public int totalMana;
    public Transform feetPos;
    public GameObject spellShooter;
    public GameObject reflectAura;
    public Image drawArea;
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
    public bool jumpAvailable;
    [System.NonSerialized]
    public bool isJumping;
    [System.NonSerialized]
    public Collider2D groundCollider;
    [System.NonSerialized] 
    public bool isHighJumping;
    [System.NonSerialized] 
    public float glideSpeed;
    [System.NonSerialized] 
    public bool readyToShoot;
    [System.NonSerialized]
    public bool isBlocking;
    [System.NonSerialized] 
    public bool isReflecting;
    [System.NonSerialized]
    public float reflectingDuration;
    [System.NonSerialized] 
    public List<EnemyAttack> reflectedAttacks = new List<EnemyAttack>();

    private float _jumpTimeCounter;
    private GestureSpells _gestureSpells;
    private LayerMask _notGroundLayer;

    private void Start()
    {
        _gestureSpells = new GestureSpells(this);
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalGravity = rigidBody.gravityScale;
        _notGroundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        groundCollider = Physics2D.OverlapCircle(feetPos.position, 0.1f, _notGroundLayer);

        // Check velocity, because if velocity is higher than 0, High Jump could have been casted and then the animation state of High Jump isn't overrided by Running
        if (groundCollider != null && rigidBody.velocity.y <= 0)
            Running();
        else if (isHighJumping)
            HighJump();

        ReflectingAura();

        #if UNITY_EDITOR
            PlayerInputDebug();
        #else
            PlayerInput();
        #endif
    }

    private void PlayerInputDebug()
    {
        if (Input.GetMouseButtonDown(0) && readyToShoot && !EventSystem.current.IsPointerOverGameObject())
            Shoot(Input.mousePosition); 

        if (Input.GetKey(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;
    }

    private void PlayerInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (readyToShoot && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        Shoot(touch.position);
                    break;
            }
        }
        else if (Input.touchCount > 1)
        {
            Touch touch = Input.GetTouch(1);
            if (readyToShoot)
                Shoot(touch.position);
        }
    }

    public void Running()
    {
        if (_gestureSpells.fastFallGroundCollider?.enabled == false)
            _gestureSpells.fastFallGroundCollider.enabled = true;

        isHighJumping = false;
        jumpAvailable = true;
        rigidBody.gravityScale = originalGravity;
        animator.SetInteger("StateNumber", 1);
        animator.SetBool("Jump", false);
    }

    public void Jump()
    {
        if (jumpAvailable)
        {
            rigidBody.velocity = Vector2.up * jumpForce;
            _jumpTimeCounter = jumpTime;
            isJumping = true;
            jumpAvailable = false;
            animator.SetBool("Jump", true);
        }

        if (isJumping)
        {
            if (_jumpTimeCounter > 0)
            {
                rigidBody.velocity = Vector2.up * jumpForce;
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
                isJumping = false;
        }
    }

    private void HighJump()
    {
        if (rigidBody.velocity.y >= 0)
            animator.SetInteger("StateNumber", 2);
        else
        {
            animator.SetInteger("StateNumber", 3);
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.down * glideSpeed;
        }
    }
    

    private void Shoot(Vector3 shootPosition)
    {
        drawArea.raycastTarget = true;
        if (reflectedAttacks.Count == 0)
        {
            AdjustShootRotation(shootPosition, spellShooter);
            _gestureSpells.ShootSpell();    
        }
        else
        {
            foreach (EnemyAttack attack in reflectedAttacks)
            {
                AdjustShootRotation(shootPosition, attack.gameObject);
                ShootAttackReflected(attack);
            }
            readyToShoot = false;
            reflectedAttacks.Clear();
            animator.SetInteger("StateNumber", 6);
            animator.SetBool("ReadyToShootSimplified", false);
        }       
    }

    private void AdjustShootRotation(Vector3 shootPosition, GameObject obj)
    {
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(shootPosition) - obj.transform.position;
        float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (lookAngle > 90 || lookAngle < -90)
        {
            float newZ = lookAngle > 90 ? 90 : -90;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
        else
        {
            obj.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        }
    }

    private void ShootAttackReflected(EnemyAttack attack)
    {
        attack.rigBody.velocity = attack.transform.right * attack.speed;
        attack.rigBody.simulated = true;
    }

    private void ReflectingAura()
    {
        if (!isReflecting)
            return;
        else
        {
            reflectingDuration -= Time.deltaTime;
            if (reflectingDuration < 0)
            {
                reflectAura.SetActive(false);
                isReflecting = false;
                _gestureSpells.lockedCasting = false;
                animator.SetBool("Reflecting", false);

                // If attacks are ready to be reflected
                if (reflectedAttacks.Count > 0)
                {
                    ReadyToShoot(simplified: true);
                    foreach (EnemyAttack attack in reflectedAttacks)
                    {
                        attack.preparingReflect = false;
                    }
                }

                // Avoiding the player animator in the default state
                if (animator.GetInteger("StateNumber") == 7)
                    animator.SetInteger("StateNumber", 1);
            }
        }
    }

    public void ReadyToShoot(bool simplified = false)
    {
        readyToShoot = true;
        drawArea.raycastTarget = false;
        animator.SetInteger("StateNumber", 5);
        if (simplified)
            animator.SetBool("ReadyToShootSimplified", true);
        else
            animator.SetBool("ReadyToShoot", true);
    }

    public void BeginCastingSpell(string id)
    {
        _gestureSpells.CastSpell(id);
    }

    public void CastingSpellFailed()
    {
        print("Casting spell failed");
    }

    // Used in Blocking animation
    public void Blocking()
    {
        if (isBlocking)
        {
            isBlocking = false;
            _gestureSpells.lockedCasting = false;
            animator.SetBool("Blocking", false);
        }
        else
            isBlocking = true;
    }
}

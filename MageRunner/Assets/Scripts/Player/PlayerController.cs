using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float jumpTime;
    public int totalMana;
    public LayerMask instantSpellsCollision;
    public Transform feetPos;
    public GameObject spellShooter;
    public GameObject reflectAura;
    public Image drawArea;
    public PlayerSpellsData spellsData;

    [System.NonSerialized] public int spellsAmount;
    [System.NonSerialized] public float originalGravity;
    [System.NonSerialized] public bool jumpAvailable;
    [System.NonSerialized] public float glideSpeed;
    [System.NonSerialized] public float reflectingDuration;
    [System.NonSerialized] public EAttackSpellType spellToShootType;
    [System.NonSerialized] public Rigidbody2D rigidBody;
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public PlayerStateHandler stateHandler;
    [System.NonSerialized] public Collider2D groundCollider;
    [System.NonSerialized] public GestureSpells gestureSpells;
    [System.NonSerialized] public List<EnemyAttack> reflectedAttacks = new List<EnemyAttack>();

    private float _jumpTimeCounter;
    private LayerMask _notGroundLayer;
    private Vector3 _shooterSpellOriginalPos;

    private void Start()
    {
        GameManager.Instance.player = this;
        stateHandler = new PlayerStateHandler();
        gestureSpells = new GestureSpells();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalGravity = rigidBody.gravityScale;
        _shooterSpellOriginalPos = spellShooter.transform.localPosition;
        _notGroundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        groundCollider = Physics2D.OverlapCircle(feetPos.position, 0.1f, _notGroundLayer);

        // Check velocity, because if velocity is higher than 0, High Jump could have been casted and then the animation state of High Jump isn't override by Running
        if (groundCollider != null && rigidBody.velocity.y <= 0)
            Running();
        else if (stateHandler.isHighJumping)
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
        //for gestures and shooting with the same tap remove drawArea field and use this: EventSystem.current.currentSelectedGameObject == null)
        if (Input.GetMouseButtonDown(0) && stateHandler.readyToShoot && !EventSystem.current.IsPointerOverGameObject())
            Shoot(Input.mousePosition); 

        if (Input.GetKey(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space))
            stateHandler.DisableState(EPlayerState.Jumping);
    }

    private void PlayerInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    //for gestures and shooting with the same tap remove drawArea field and use this: EventSystem.current.currentSelectedGameObject == null)
                    if (stateHandler.readyToShoot && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)) 
                        Shoot(touch.position);
                    break;
            }
        }
        else if (Input.touchCount > 1)
        {
            Touch touch = Input.GetTouch(1);
            if (stateHandler.readyToShoot)
                Shoot(touch.position);
        }
    }

    public void Running()
    {
        if (gestureSpells.fastFallGroundCollider?.enabled == false)
            gestureSpells.fastFallGroundCollider.enabled = true;
        
        stateHandler.EnableState(EPlayerState.Running);
    }

    private void Jump()
    {
        if (jumpAvailable)
        {
            stateHandler.EnableState(EPlayerState.Jumping);
            _jumpTimeCounter = jumpTime;
        }

        if (stateHandler.isJumping == false)
            return;

        if (_jumpTimeCounter > 0)
        {
            rigidBody.velocity = Vector2.up * jumpForce;
            _jumpTimeCounter -= Time.deltaTime;
        }
        else
            stateHandler.DisableState(EPlayerState.Jumping);
    }

    private void HighJump()
    {
        if (rigidBody.velocity.y >= 0)
            stateHandler.EnableState(EPlayerState.HighJumping);
        else
        {
            stateHandler.EnableState(EPlayerState.Gliding);
        }
    }
    

    private void Shoot(Vector3 shootPosition)
    {
        drawArea.raycastTarget = true;
        spellShooter.transform.localPosition = _shooterSpellOriginalPos;
        spellShooter.transform.localRotation = Quaternion.identity;
        if (reflectedAttacks.Count == 0)
        {
            switch (spellToShootType)
            {
                case EAttackSpellType.Projectile:
                    AdjustShootRotation(shootPosition, spellShooter); break;
                case EAttackSpellType.Instant:
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint((Input.mousePosition));
                    Vector3 tapPosition = new Vector2(worldPoint.x, worldPoint.y);
                    RaycastHit2D spellLineHit = Physics2D.Linecast(tapPosition + new Vector3(0f, 50f, 0f), tapPosition + new Vector3(0f, -50f, 0f), instantSpellsCollision);
                    spellShooter.transform.position = spellLineHit.point;
                    break;
            }
            gestureSpells.ShootSpell();
        }
        else
        {
            foreach (EnemyAttack attack in reflectedAttacks)
            {
                AdjustShootRotation(shootPosition, attack.gameObject);
                ShootAttackReflected(attack);
            }
            reflectedAttacks.Clear();
            stateHandler.DisableState(EPlayerState.ReadyToShootSimplified);
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
        if (!stateHandler.isReflecting)
            return;

        reflectingDuration -= Time.deltaTime;
        if (reflectingDuration >= 0)
            return;
        
        // Reflecting ended
        stateHandler.DisableState(EPlayerState.Reflecting);

        // If attacks are ready to be reflected
        if (reflectedAttacks.Count > 0)
        {
            drawArea.raycastTarget = false;
            stateHandler.EnableState(EPlayerState.ReadyToShootSimplified);
            foreach (EnemyAttack attack in reflectedAttacks)
            {
                attack.preparingReflect = false;
            }
        }

        // Avoiding the player animator in the default state
        if (animator.GetInteger("StateNumber") == 7)
            animator.SetInteger("StateNumber", 1);
    }
    
    public void BeginCastingSpell(string id) => gestureSpells.CastSpell(id);

    public void CastingSpellFailed() => print("Casting spell failed");

    // Used in Blocking animation
    public void Blocking()
    {
        if (stateHandler.isBlocking)
        {
            stateHandler.DisableState(EPlayerState.Blocking);
        }
        else
            stateHandler.isBlocking = true;
    }
}
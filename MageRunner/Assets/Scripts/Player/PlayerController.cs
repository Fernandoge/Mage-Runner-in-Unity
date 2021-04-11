using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private int _totalHealthpoints;
    [SerializeField] private int _totalMana;
    [SerializeField] private float _jumpTime;
    public float jumpForce;
    
    [Header("References")]
    public GameObject spellShooter;
    public HealthController healthController;
    public ManaController manaController;
    public ChatBubbleController companionChatBubble;
    public PlayerSpellsData spellsData;
    
    [System.NonSerialized] public float originalGravity;
    [System.NonSerialized] public int groundLayer;
    [System.NonSerialized] public bool jumpAvailable;
    [System.NonSerialized] public bool jumpStillPressed;
    [System.NonSerialized] public bool idleCastEnabled;
    [System.NonSerialized] public float glideSpeed;
    [System.NonSerialized] public Rigidbody2D rigidBody;
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public PlayerStateHandler stateHandler;
    [System.NonSerialized] public Collider2D groundCollider;
    
    [SerializeField] private Transform _feetPos;
    [SerializeField] private Selectable _jumpButton;
    
    private float _jumpTimeCounter;
    private Camera _mainCamera;
    private LayerMask _notGroundLayerMask;
    private GestureSpellsController _gestureSpellsController;
    
    private void Start()
    {
        GameManager.Instance.player = this;
        _mainCamera = Camera.main;
        stateHandler = new PlayerStateHandler(this);
        _gestureSpellsController = new GestureSpellsController(this);
        healthController.Initialize(_totalHealthpoints);
        manaController.Initialize(_totalMana);
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalGravity = rigidBody.gravityScale;
        _notGroundLayerMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("BottomGround");
    }

    private void Update()
    {
        groundCollider = Physics2D.OverlapCircle(_feetPos.position, 0f, _notGroundLayerMask);

        // Check velocity, because if velocity is higher than 0 High Jump could have been casted and then the animation state of High Jump isn't override by Running
        if (groundCollider != null && rigidBody.velocity.y <= 0)
        {
            Running();
            groundLayer = groundCollider.gameObject.layer;
        }
        else if (stateHandler.isHighJumping)
            HighJump();

        _jumpButton.interactable = GameManager.Instance.level.isMoving;
        
#if UNITY_EDITOR
        PlayerInputDebug();
#else
        // PlayerInput();
#endif
    }

    private void PlayerInputDebug()
    {
        // For gesture and shooting with the same tap remove drawArea field and use this: EventSystem.current.currentSelectedGameObject == null)
        // if (Input.GetMouseButtonDown(0) && stateHandler.readyToShoot && !EventSystem.current.IsPointerOverGameObject())
        //     Shoot(Input.mousePosition);

        if (Input.GetKey(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stateHandler.DisableState(EPlayerState.Jumping);
            jumpStillPressed = false;
        }
    }

    // private void PlayerInput()
    // {
    //     ShootValidTouch(0);
    //     ShootValidTouch(1);
    // }
    //
    // public void ShootValidTouch(int touchNumber)
    // {
    //     if (Input.touchCount > touchNumber)
    //     {
    //         Touch touch = Input.GetTouch(touchNumber);
    //         if (touch.phase == TouchPhase.Began)
    //             if (stateHandler.readyToShoot && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
    //                 Shoot(touch.position);
    //     }
    // }
    
    public void Running()
    {
        if (_gestureSpellsController.fastFallGroundCollider?.enabled == false)
            _gestureSpellsController.fastFallGroundCollider.enabled = true;

        stateHandler.EnableState(EPlayerState.Running);
    }

    public void Jump()
    {
        if (GameManager.Instance.level.isMoving == false)
            return;
        
        if (jumpAvailable && jumpStillPressed == false)
        {
            stateHandler.EnableState(EPlayerState.Jumping);
            _jumpTimeCounter = _jumpTime;
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
            stateHandler.EnableState(EPlayerState.Gliding);
    }

    public IEnumerator Dash(float dashDuration, float dashSpeed)
    {
        foreach (RepeatingBG bg in GameManager.Instance.level.movingBgs.OfType<RepeatingBG>())
        {
            bg.startX += dashDuration;
            bg.endX += dashDuration;
        }
    
        foreach (MovingParticles particle in GameManager.Instance.level.movingParticles)
            particle.ModifyVelocityOverLifetimeSpeed(dashSpeed);
        
        Vector3 newPlayerPosition = new Vector3(transform.localPosition.x + dashDuration, transform.localPosition.y, transform.localPosition.z);
        
        while (transform.localPosition.x - newPlayerPosition.x < 0f)
        {
            transform.Translate(Vector2.right * dashSpeed * Time.deltaTime);
            _mainCamera.transform.Translate(Vector2.right * dashSpeed * Time.deltaTime);
    
            foreach (MovingBG bg in GameManager.Instance.level.movingBgs)
                bg.transform.Translate(Vector2.right * (dashSpeed - bg.speed) * Time.deltaTime);
    
            foreach (MovingParticles particle in GameManager.Instance.level.movingParticles)
                particle.transform.Translate(Vector2.right * dashSpeed * Time.deltaTime);
    
            yield return null;
        }
    
        foreach (MovingParticles particle in GameManager.Instance.level.movingParticles)   
            particle.ResetVelocityOverLifetimeSpeed();
    
        stateHandler.DisableState(EPlayerState.Dashing);
    }

    public void BeginCastingSpell(string id) => _gestureSpellsController.CastSpell(id);

    public void CastingSpellFailed() => print("Casting spell failed");

    // Used in Blocking animation
    public void StopBlocking() => stateHandler.DisableState(EPlayerState.Blocking);

    // Used in Shooting animation
    public void StopShooting() => stateHandler.DisableState(EPlayerState.Shooting);
}
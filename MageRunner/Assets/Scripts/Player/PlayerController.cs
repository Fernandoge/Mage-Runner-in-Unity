using System;
using System.Collections;
using System.Linq;
using MageRunner.Combat;
using MageRunner.Dialogues;
using MageRunner.Levels;
using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private int _totalHealthpoints;
        [SerializeField] private int _totalMana;
        public float groundJumpForce; 
        public float airJumpForce;
    
        [Header("References")]
        public GameObject spellShooter;
        public HealthController healthController;
        public ManaController manaController;
        public ChatBubbleController companionChatBubble;
        public PlayerSpellsData spellsData;
        public Selectable jumpButton;
    
        [NonSerialized] public float originalGravity;
        [NonSerialized] public bool groundJumpAvailable;
        [NonSerialized] public bool airJumpAvailable;
        [NonSerialized] public bool highJumpAvailable;
        [NonSerialized] public bool idleCastEnabled;
        [NonSerialized] public float glideSpeed;
        [NonSerialized] public Rigidbody2D rigidBody;
        [NonSerialized] public Animator animator;
        [NonSerialized] public PlayerStateHandler stateHandler;
        [NonSerialized] public Collider2D groundCollider; 
    
        [SerializeField] private Transform _feetPos;
        
        private Camera _mainCamera;
        private LayerMask _notGroundLayerMask;
        
        public GestureSpellsController gestureSpellsController { get; private set; }
        public event Action playerDash;

        private void Start()
        {
            GameManager.Instance.player = this;
            _mainCamera = GameManager.Instance.mainCamera;
            stateHandler = new PlayerStateHandler(this);
            gestureSpellsController = new GestureSpellsController(this);
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

            // Check velocity, because if velocity is higher than 0 High Jump could have been casted and with this check the animation state of High Jump isn't override by Running
            if (groundCollider != null && rigidBody.velocity.y <= 0)
                Running();
            else if (stateHandler.isHighJumping)
                HighJump();
            else
                groundJumpAvailable = false;

#if UNITY_EDITOR
            PlayerInputDebug();
#endif
        }

        private void PlayerInputDebug()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }

        public void Running()
        {
            stateHandler.EnableState(EPlayerState.Running);
            
            if (gestureSpellsController.fastFallGroundCollider == null)
                return;
            
            if (gestureSpellsController.fastFallGroundCollider.enabled == false)
                gestureSpellsController.fastFallGroundCollider.enabled = true;

        }

        public void Jump()
        {
            if (GameManager.Instance.level.isMoving == false || stateHandler.isDashing)
                return;

            if (groundJumpAvailable)
                stateHandler.EnableState(EPlayerState.GroundJumping);
            else if (airJumpAvailable)
                stateHandler.EnableState(EPlayerState.AirJumping);
        }

        private void HighJump()
        {
            if (rigidBody.velocity.y >= 0)
                stateHandler.EnableState(EPlayerState.HighJumping);
            else
                stateHandler.EnableState(EPlayerState.Gliding);
        }
        
        public IEnumerator Block(float blockDuration)
        {
            while (blockDuration > 0)
            {
                blockDuration -= Time.deltaTime;
                yield return null;
            }
            
            stateHandler.DisableState(EPlayerState.Blocking);
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
        
            while (transform.localPosition.x - newPlayerPosition.x < 0f && stateHandler.isDashing)
            {
                transform.Translate(Vector2.right * (dashSpeed * Time.deltaTime));
                _mainCamera.transform.Translate(Vector2.right * (dashSpeed * Time.deltaTime));
    
                foreach (MovingBG bg in GameManager.Instance.level.movingBgs)
                    bg.transform.Translate(Vector2.right * ((dashSpeed - bg.speed) * Time.deltaTime));
    
                foreach (MovingParticles particle in GameManager.Instance.level.movingParticles)
                    particle.transform.Translate(Vector2.right * (dashSpeed * Time.deltaTime));
    
                yield return null;
            }

            playerDash?.Invoke();
            
            foreach (MovingParticles particle in GameManager.Instance.level.movingParticles)   
                particle.ResetVelocityOverLifetimeSpeed();
    
            stateHandler.DisableState(EPlayerState.Dashing);
        }

        public void BeginCastingSpell(string id) => gestureSpellsController.CastSpell(id);

        public void CastingSpellFailed() => print("Casting spell failed");
    }
}
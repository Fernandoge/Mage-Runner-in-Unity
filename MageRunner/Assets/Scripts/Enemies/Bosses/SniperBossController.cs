using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MageRunner.Constants;
using MageRunner.Dialogues;
using MageRunner.Levels;
using MageRunner.Managers.GameManager;
using MageRunner.Player;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MageRunner.Enemies.Bosses
{
    public class SniperBossController : EnemyController
    {
        [Header("Sniper References")] 
        [SerializeField] private Collider2D _currentPlatformCollider;
        [SerializeField] private EnemyAttack _objectToShoot;
        [SerializeField] private Transform _SniperIndicator;
        [SerializeField] private Transform _SnipeShootPosition;
        [SerializeField] private ChatBubbleController _chatBubble;
        [SerializeField] private SpriteRenderer _bossSprite;

        [Header("Snipe Values")] 
        [SerializeField] private float _objectToShootSpeed;
        [SerializeField] private float _sniperIndicatorMoveSpeed;
        [SerializeField] private float _snipeNoFlashDuration;
        [SerializeField] private float _snipeSlowFlashDuration;
        [SerializeField] private float _snipeFastFlashDuration;
        [SerializeField] private GameObject _weapon;

        [Header("Quick Snipe Values")] 
        [SerializeField] private float _quickSnipeShootSpeed;

        [Header("Jumping Values")] 
        [SerializeField] private float _jumpSpeed;

        // For now use vector X and Y as the loop start and end X respectively
        [Header("Loops")] 
        [SerializeField] private LoopValues[] _loops;

        [Header("Health")]
        [SerializeField] private int[] HPPerLoop;
        
        private SpriteRenderer _sniperIndicatorSpriteRender;
        private bool _isSniping;
        private bool _isQuickSniping;
        private bool _introCompleted;
        private bool _isPlayingIntro;
        private bool _isPlatformJumping;
        private bool _isLevelBeingLooped;
        private int _loopIndex = 0;
        private int _currentPhase = 0;
        private LayerMask _notGroundLayerMask;

        private void OnEnable() => gesturesHolderController.zeroHpCallback += OnZeroHP;
        
        private void OnDisable() => gesturesHolderController.zeroHpCallback -= OnZeroHP;
        
        private void OnBecameVisible()
        {
            enabled = true;

            if (!_isSniping) 
                return;
            
            // Stop Sniping
            _isSniping = false;
            _SniperIndicator.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        private void Start()
        {
            DOTween.Init();
            enabled = false;
            _notGroundLayerMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("BottomGround");

            // Start Sniping
            _SniperIndicator.SetParent(GameManager.Instance.mainCamera.transform);
            _sniperIndicatorSpriteRender = _SniperIndicator.GetComponent<SpriteRenderer>();
            
            //TODO: Make melee enemies and FTUE block during the snipe
            StartCoroutine(Snipe(_snipeNoFlashDuration));
        }

        private void Update()
        {
            if (_isQuickSniping)
            {
                Vector2 weaponLookDirection = GameManager.Instance.player.transform.position - _weapon.transform.position;
                float weaponLookAngle = Mathf.Atan2(weaponLookDirection.y, weaponLookDirection.x) * Mathf.Rad2Deg;
                _weapon.transform.rotation = Quaternion.Euler(0f, 0f, weaponLookAngle);

                if (gesturesHolderController.distanceBetweenPlayerX < Constant.SniperQuickSnipeDistance)
                {
                    SnipeShoot(_weapon.transform, _weapon.transform.right, _quickSnipeShootSpeed, GameManager.Instance.level.movingObjects);
                    _isQuickSniping = false;
                    _weapon.SetActive(false);
                }
            }

            if (gesturesHolderController.distanceBetweenPlayerX > Constant.SniperPlatformJumpDistance) 
                return;
            
            if (!_introCompleted && !_isPlayingIntro)
                PlayIntro();
            else if (_introCompleted && !_isPlatformJumping && !_isLevelBeingLooped)
                StartCoroutine(PlatformJump());
        }

        private void PlayIntro()
        {
            _isPlayingIntro = true;
            _chatBubble.StartChatCoroutine(new Message("sup, I'm the boss", 0f, 3f, changeLevelMovingState: true));
            _chatBubble.StartChatCoroutine(new Message("it's time to d-d-d-d-duel", 0f, 3f, changeLevelMovingState: true, onTextClosed: () => StartPhase(1)));
        }
        
        private void StartPhase(int phaseNumber)
        {
            switch (phaseNumber)
            {
                case 1:
                    _introCompleted = true;
                    GameManager.Instance.level.StartLooping(_loops[_loopIndex].startX, _loops[_loopIndex].endX, distanceReturned => StartCoroutine(TryLoopLevel(distanceReturned)));
                    gesturesHolderController.currentHealthpoints = HPPerLoop[_loopIndex];
                    break;
                case 2:
                    //TODO: Make a big jump to the right and start sniping again
                    break;
            }
        }
        
        private IEnumerator Snipe(float durationSniping, float delayToStartAiming = 0, int disableFrame = 0, int enableFrame = 0)
        {
            if (!_isSniping)
            {
                _isSniping = true;
                yield return new WaitForSeconds(delayToStartAiming);
                
                float randomY = Random.Range(-5f, 6.5f);
                _SniperIndicator.localPosition = new Vector3(12f, randomY, 10f);
                _SniperIndicator.gameObject.SetActive(true);
            }
            
            int frameNumber = 0;
            while (durationSniping > 0)
            {
                var sniperIndicatorLocalPosition = _SniperIndicator.transform.localPosition;
                var playerNewY = new Vector3(sniperIndicatorLocalPosition .x, GameManager.Instance.player.transform.position.y, 10f);
                _SniperIndicator.transform.localPosition = Vector3.MoveTowards(sniperIndicatorLocalPosition, playerNewY, _sniperIndicatorMoveSpeed * Time.deltaTime);
                
                durationSniping -= Time.deltaTime;
                
                frameNumber++;
                if (frameNumber == enableFrame)
                {
                    _sniperIndicatorSpriteRender.enabled = true;
                    frameNumber = 0;
                }
                if (frameNumber == disableFrame)
                    _sniperIndicatorSpriteRender.enabled = false;

                yield return null;
            }
            
            if (disableFrame == 0)
                StartCoroutine(Snipe(_snipeSlowFlashDuration, disableFrame: 150, enableFrame: 300));
            else if (disableFrame == 150)
                StartCoroutine(Snipe(_snipeFastFlashDuration, disableFrame: 50, enableFrame: 100));
            else if (disableFrame == 50)
            {
                _SniperIndicator.gameObject.SetActive(false);
                SnipeShoot(_SnipeShootPosition, Vector2.left, _objectToShootSpeed, transform.parent);
                _isSniping = false;
                StartCoroutine(Snipe(_snipeNoFlashDuration, 3f));
            }
        }
        
        private void SnipeShoot(Transform bulletOrigin, Vector3 bulletDirection, float bulletSpeed, Transform bulletParent)
        {
            GameObject objectShot = Instantiate(_objectToShoot.gameObject, bulletOrigin.position, bulletOrigin.rotation, bulletParent);
            EnemyAttack enemyAttackShot = objectShot.GetComponent<EnemyAttack>();
            float speedAddition = GameManager.Instance.level.isMoving == false ? 0f : GameManager.Instance.level.movingSpeed / 2;
            enemyAttackShot.rigBody.velocity = bulletDirection * (bulletSpeed + speedAddition);
        }

        private IEnumerator PlatformJump()
        {
            _isPlatformJumping = true;
            
            var platformsList = FindNextPlatform();
            if (platformsList.Count == 0)
            {
                print("No more platforms to jump");
                yield break;
            }
            
            platformsList.Remove(_currentPlatformCollider);
            var platformToJump = platformsList[Random.Range(0, platformsList.Count)];

            Vector2 platformToJumpPosition = new Vector2(platformToJump.transform.position.x, platformToJump.transform.position.y + 0.5f + platformToJump.bounds.size.y / 2);

            animator.SetBool("Jumping", true);
            _bossSprite.transform.DORotate(new Vector3(0, 0, -360f), 1.2f, RotateMode.FastBeyond360);
            Sequence jumpSequence = transform.DOJump(platformToJumpPosition, 5, 1, 1.2f).SetEase(Ease.Linear);

            yield return jumpSequence.WaitForCompletion();
            
            _currentPlatformCollider = platformToJump;
            animator.SetBool("Jumping", false);
            _isPlatformJumping = false;

            _isQuickSniping = true;
            _weapon.SetActive(true);
        }

        private IEnumerator TryLoopLevel(float distanceReturned)
        {
            if (_isLevelBeingLooped)
                yield break;
            
            _isLevelBeingLooped = true;
            while (_isLevelBeingLooped)
            {
                yield return null;
                
                if (_isPlatformJumping) 
                    continue;

                LevelController currentLevel = GameManager.Instance.level;
                Vector3 movingObjectsLocalPosition = currentLevel.movingObjects.transform.localPosition;
                float diff = movingObjectsLocalPosition.x - currentLevel.repeatingEndX;
                
                // Move moving objects, player and player spells to make the loop 
                GameManager.Instance.level.movingObjects.transform.localPosition = new Vector2(currentLevel.repeatingStartX + diff, movingObjectsLocalPosition.y);
                transform.position = new Vector3(transform.position.x - distanceReturned, transform.position.y, transform.position.z);
                foreach (PlayerAttackSpell spell in GameManager.Instance.gameActivePlayerSpells) 
                    spell.transform.position = new Vector3(spell.transform.position.x - distanceReturned, spell.transform.position.y, spell.transform.position.z);

                _isLevelBeingLooped = false;
                print("level looped");
            }
        }

        private List<Collider2D> FindNextPlatform()
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(_notGroundLayerMask);
            List<Collider2D> platformsList = new List<Collider2D>();
            var bossPosition = transform.position;
            Physics2D.OverlapArea(new Vector2(bossPosition.x + 15, 10), new Vector2(bossPosition.x + 20, -10), contactFilter, platformsList);
            return platformsList;
        }

        private void OnZeroHP()
        {
            GameManager.Instance.level.StopLooping();

            _currentPhase += 1;
            StartPhase(_currentPhase);
        }
    }
}
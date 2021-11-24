using System;
using System.Collections.Generic;
using System.Linq;
using MageRunner.Gestures;
using MageRunner.Levels;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Player
{
    public class GestureSpellsController
    {
        public Collider2D fastFallGroundCollider;

        private string[] _basicSpellsIds = {"highJump", "fastFall", "dash", "block"};
        private Dictionary<EPlayerSpells, Action> _spellsDict = new Dictionary<EPlayerSpells, Action>();
        private EAttackSpellType _spellToShootType;
        private Vector3 _playerShooterSpellOriginalPos;
        private GesturesHolderController _targetedGesturesHolder;
        private PlayerController _player;
        private GameObject _spellToShoot;
        private float _spellToShootSpeed;
        
        public Dictionary<string, Action> basicSpellsDict = new Dictionary<string, Action>();

        public GestureSpellsController(PlayerController player)
        {
            _player = player;
            _playerShooterSpellOriginalPos = _player.spellShooter.transform.localPosition;
            LoadSpells();
        }
    
        public void CastSpell(string id)
        {
            if (_basicSpellsIds.Contains(id))
                basicSpellsDict[id]();
        
            else
            {
                bool gestureFound = false;
                foreach (Gesture gesture in GameManager.Instance.activeGestures.ToList())
                {
                    if (gesture.pattern.id == id)
                    {
                        gestureFound = true;
                        gesture.iconRenderer.gameObject.SetActive(false);
                        _targetedGesturesHolder = gesture.holder;
                        GameManager.Instance.RemoveGesture(gesture);
                        
                        _spellsDict[gesture.spell]();
                    }
                }

                if (gestureFound == false)
                    _player.CastingSpellFailed();
            }
        }

    
        public void ShootSpell()
        {
            _player.spellShooter.transform.localPosition = _playerShooterSpellOriginalPos;
            _player.spellShooter.transform.localRotation = Quaternion.identity;
            switch (_spellToShootType)
            {
                case EAttackSpellType.Projectile:
                    Vector2 lookDirection = _targetedGesturesHolder.transform.position - _player.spellShooter.transform.position;
                    float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                    _player.spellShooter.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
                    break;
                case EAttackSpellType.Instant:
                    _player.spellShooter.transform.position = _targetedGesturesHolder.transform.position;
                    break;
            }

            LevelController currentLevel = GameManager.Instance.level;
            Transform spellParent = currentLevel.timeFrames[currentLevel.currentTimeFrameIndex].staticGO.transform;
            if (_targetedGesturesHolder.isMoving)
                spellParent = currentLevel.timeFrames[currentLevel.currentTimeFrameIndex].movingGO.transform;
            
            GameObject objectShot = UnityEngine.Object.Instantiate(_spellToShoot, _player.spellShooter.transform.position, _player.spellShooter.transform.rotation, spellParent);
            PlayerAttackSpell attackSpellShot = objectShot.GetComponent<PlayerAttackSpell>();
            float speedBoost = _targetedGesturesHolder.isMoving ? 0f : currentLevel.movingSpeed;
            float speedReduction = currentLevel.isMoving == false ? 0f : (_spellToShootSpeed.Equals(0f) ? 0f : currentLevel.movingSpeed / 2);
            attackSpellShot.StartMovingToTarget(_targetedGesturesHolder, _spellToShootSpeed + speedBoost - speedReduction, _player.spellShooter);
        }

        private void LoadSpells()
        {
            LoadBasicSpells();

            foreach (EPlayerSpells playerSpell in Enum.GetValues(typeof(EPlayerSpells)))
            {
                Action newSpellAction = GetSpellData(playerSpell);
                _spellsDict.Add(playerSpell, newSpellAction);
            }
        }

        private void LoadBasicSpells()
        {
            // High Jump
            PlayerSpellsData.HighJump highJump = _player.spellsData.highJump;
            Action highJumpCast = () =>
            {
                if (_player.highJumpAvailable == false || _player.stateHandler.isDashing)
                    return;
                
                _player.glideSpeed = highJump.glideSpeed;
                _player.rigidBody.velocity = Vector2.up * highJump.jumpSpeed;
                _player.stateHandler.EnableState(EPlayerState.HighJumping);
                _player.stateHandler.DisableState(EPlayerState.Blocking);
            };
            basicSpellsDict.Add(highJump.gesture.id, highJumpCast);
       
            // Fast Fall
            PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
            Action fastFallCast = () =>
            {
                if ((_player.groundCollider != null && _player.groundCollider.gameObject.layer == LayerMask.NameToLayer("BottomGround")) || _player.stateHandler.isDashing)
                    return;
                
                _player.stateHandler.EnableState(EPlayerState.FastFalling);
                _player.stateHandler.DisableState(EPlayerState.Blocking);
                _player.rigidBody.velocity = Vector2.down * fastFall.fallSpeed;

                if (_player.groundCollider == null)
                    return;
            
                fastFallGroundCollider = _player.groundCollider;
                fastFallGroundCollider.enabled = false;
                
            };
            basicSpellsDict.Add(fastFall.gesture.id, fastFallCast);

            // Block
            PlayerSpellsData.Block block = _player.spellsData.block;
            Action blockCast = () =>
            {
                if (_player.stateHandler.isBlocking)
                    return;
                
                _player.stateHandler.EnableState(EPlayerState.Blocking);
                _player.StartCoroutine(_player.Block(block.duration));
            };
            basicSpellsDict.Add(block.gesture.id, blockCast);

            // Dash
            PlayerSpellsData.Dash dash = _player.spellsData.dash;
            Action dashCast = () =>
            {
                _player.stateHandler.EnableState(EPlayerState.Dashing);
                _player.stateHandler.DisableState(EPlayerState.Blocking);
                _player.StartCoroutine(_player.Dash(dash.duration, dash.speed));
            };
            basicSpellsDict.Add(dash.gesture.id, dashCast);
        }
    
        private Action GetSpellData(EPlayerSpells playerSpell)
        {
            switch (playerSpell)
            {
                case EPlayerSpells.Fireball:
                    return LoadAttackSpell(_player.spellsData.fireball, EAttackSpellType.Projectile);

                case EPlayerSpells.Ice:
                    return LoadAttackSpell(_player.spellsData.ice, EAttackSpellType.Projectile);

                case EPlayerSpells.Earth:
                    return LoadAttackSpell(_player.spellsData.earth, EAttackSpellType.Projectile);

                case EPlayerSpells.Wind:
                    return LoadAttackSpell(_player.spellsData.wind, EAttackSpellType.Projectile);

                case EPlayerSpells.Lightning:
                    return LoadAttackSpell(_player.spellsData.lightning, EAttackSpellType.Instant);

                case EPlayerSpells.Nature:
                    return LoadAttackSpell(_player.spellsData.nature, EAttackSpellType.Projectile);

                case EPlayerSpells.Water:
                    return LoadAttackSpell(_player.spellsData.water, EAttackSpellType.Projectile);
            
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerSpell), playerSpell, null);
            } 
        }
    
        private Action LoadAttackSpell(PlayerSpellsData.AttackSpell attackSpell, EAttackSpellType attackSpellType, Action extraAction = null)
        {
            Action attackSpellCast = () =>
            {
                _spellToShoot = attackSpell.spellObject;
                _spellToShootSpeed = attackSpell.speed;
                _spellToShootType = attackSpellType;
                ShootSpell();
                _player.stateHandler.EnableState(EPlayerState.Shooting);

                extraAction?.Invoke();
            };
        
            return attackSpellCast;
        }
    }
}
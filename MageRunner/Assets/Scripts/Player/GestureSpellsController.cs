using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GestureRecognizer;

public class GestureSpellsController
{
    public Collider2D fastFallGroundCollider;

    private string[] _basicSpellsIds = {"highJump", "fastFall", "reflect", "block"};
    private Dictionary<string, Action> _basicSpellsDict = new Dictionary<string, Action>();
    private Dictionary<EPlayerSpells, Action> _spellsDict = new Dictionary<EPlayerSpells, Action>();
    private EAttackSpellType _spellToShootType;
    private Vector3 _playerShooterSpellOriginalPos;
    private Vector3 _gesturesHolderPosition;
    private PlayerController _player;
    private GameObject _spellToShoot;

    public GestureSpellsController(PlayerController player)
    {
        _player = player;
        _playerShooterSpellOriginalPos = _player.spellShooter.transform.localPosition;
        LoadSpells();
    }
    
    public void CastSpell(string id)
    {
        if (_basicSpellsIds.Contains(id))
            _basicSpellsDict[id]();
        
        else
        {
            bool gestureFound = false;
            foreach (Gesture gesture in GameManager.Instance.activeGestures.ToArray())
            {
                if (gesture.pattern.id == id)
                {
                    gestureFound = true;
                    GameObject gestureGO = gesture.iconRenderer.gameObject;
                    _gesturesHolderPosition = gesture.holder.position;
                    GameManager.Instance.activeGestures.Remove(gesture);
                    gestureGO.SetActive(false);
                    
                    _spellsDict[gesture.spell]();
                }
            }

            if (gestureFound == false)
                _player.CastingSpellFailed();
        }
    }

    
    public void PrepareToShoot()
    {
        _player.spellShooter.transform.localPosition = _playerShooterSpellOriginalPos;
        _player.spellShooter.transform.localRotation = Quaternion.identity;
        switch (_spellToShootType)
        {
            case EAttackSpellType.Projectile:
                Vector2 lookDirection = _gesturesHolderPosition - _player.spellShooter.transform.position;
                float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                _player.spellShooter.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
                break;
            case EAttackSpellType.Instant:
                _player.spellShooter.transform.position = _gesturesHolderPosition;
                break;
        }

        ShootSpell();
    }
    
    public void ShootSpell()
    {
        LevelController currentLevel = GameManager.Instance.level;
        Transform spellParent = currentLevel.movingObjects;
        GameObject shootedSpell = UnityEngine.Object.Instantiate(_spellToShoot, _player.spellShooter.transform.position, _player.spellShooter.transform.rotation, spellParent);
        PlayerSpell shootedSpellComponent = shootedSpell.GetComponent<PlayerSpell>();
        float speedReduction = currentLevel.isMoving == false ? 0f : (shootedSpellComponent.speed.Equals(0f) ? currentLevel.movingSpeed : currentLevel.movingSpeed / 2);
        shootedSpellComponent.rigBody.velocity = _player.spellShooter.transform.right * (shootedSpellComponent.speed - speedReduction);
        shootedSpellComponent.shooterLayer = _player.gameObject.layer;
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
            if (_player.jumpAvailable)
            {
                _player.stateHandler.EnableState(EPlayerState.HighJumping);
                _player.rigidBody.velocity = Vector2.up * highJump.jumpSpeed;
                _player.glideSpeed = highJump.glideSpeed;
            }
        };
        _basicSpellsDict.Add(highJump.gesture.id, highJumpCast);
       
        // Fast Fall
        PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
        Action fastFallCast = () =>
        {
            if (_player.groundCollider == null || _player.groundLayer != LayerMask.NameToLayer("BottomGround"))
            {
                _player.stateHandler.EnableState(EPlayerState.FastFall);
                _player.rigidBody.velocity = Vector2.down * fastFall.fallSpeed;

                if (_player.groundCollider == null)
                    return;
                
                fastFallGroundCollider = _player.groundCollider;
                fastFallGroundCollider.enabled = false;
            }
        };
        _basicSpellsDict.Add(fastFall.gesture.id, fastFallCast);

        // Block
        PlayerSpellsData.BaseSpell block = _player.spellsData.block;
        Action blockCast = () =>
        {
            _player.stateHandler.EnableState(EPlayerState.Blocking);
        };
        _basicSpellsDict.Add(block.gesture.id, blockCast);

        // Reflect
        PlayerSpellsData.Aura reflect = _player.spellsData.reflect;
        Action reflectCast = () =>
        {
            Debug.LogError("Need to implement Blink");
        };
        _basicSpellsDict.Add(reflect.gesture.id, reflectCast);
    }
    
    private Action GetSpellData(EPlayerSpells playerSpell)
    {
        switch (playerSpell)
        {
            case EPlayerSpells.Fireball:
                Action checkFtueStep = () =>
                {
                    if (GameManager.Instance.ftueController.ftueStepIndex == 6)
                        GameManager.Instance.ftueController.NextFtueStep();
                };
                return LoadAttackSpell(_player.spellsData.fireball, EAttackSpellType.Projectile, checkFtueStep);

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
                _spellToShootType = attackSpellType;
                PrepareToShoot();
                _player.stateHandler.EnableState(EPlayerState.Shooting);
                // ReSharper disable once UseNullPropagation
                if (extraAction != null)
                    extraAction();
        };
        
        return attackSpellCast;
    }
}
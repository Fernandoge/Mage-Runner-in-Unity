using System.Collections.Generic;
using UnityEngine;
using System;

public class GestureSpells
{
    private struct Spell
    {
        public string id;
        public string name;
        public int mana;
        public Action castSpell;

        public Spell(string id, string name, int mana, Action castSpell)
        {
            this.id = id;
            this.name = name;
            this.mana = mana;
            this.castSpell = castSpell;
        }
    }

    public bool lockedCasting;
    public Collider2D fastFallGroundCollider;

    private Dictionary<string, Spell> _spellsDict = new Dictionary<string, Spell>();
    private PlayerController _player;
    private GameObject _spellToShoot;

    public GestureSpells(PlayerController player)
    {
        _player = player;
        LoadSpells();
    }
    
    public void CastSpell(string id)
    {
        Spell spell = _spellsDict[id];
        if (_player.manaController.currentMana >= spell.mana)
        {
            _player.manaController.UpdateMana(-spell.mana);
            // _player.companionChatBubble.StartChat(new Message(spell.name, 1.2f, false));
            spell.castSpell();
        }
        else
        {
            _player.manaController.NoManaFeedback();
            Debug.Log("Not enough mana");
        }
    }

    public void ShootSpell()
    {
        LevelController currentLevel = GameManager.Instance.level;
        Transform spellParent = currentLevel.movingObjects;
        GameObject shootedSpell = UnityEngine.Object.Instantiate(_spellToShoot, _player.spellShooter.transform.position, _player.spellShooter.transform.rotation, spellParent);
        Attack shootedSpellComponent = shootedSpell.GetComponent<Attack>();
        float speedReduction = currentLevel.isMoving == false ? 0f : (shootedSpellComponent.speed.Equals(0f) ? currentLevel.movingSpeed : currentLevel.movingSpeed / 2);
        shootedSpellComponent.rigBody.velocity = _player.spellShooter.transform.right * (shootedSpellComponent.speed - speedReduction);
        shootedSpellComponent.shooterLayer = _player.gameObject.layer;
        _player.stateHandler.DisableState(EPlayerState.ReadyToShoot);
    }
    
    private void PrepareAttack(PlayerSpellsData.AttackSpell attackSpell, EAttackSpellType spellType)
    {
        if (lockedCasting == false)
        {
            _spellToShoot = attackSpell.spellObject;
            _player.drawArea.raycastTarget = false;
            _player.spellToShootType = spellType;
            _player.stateHandler.EnableState(EPlayerState.ReadyToShoot);
        }
    }
    

    private void LoadSpells()
    {
        for (int i = 0; i < _player.spellsAmount; i++)
        {
            Spell newSpell = GetSpellData(i);
            _spellsDict.Add(newSpell.id, newSpell);
        }
    }
    
    private Spell GetSpellData(int spellNumber)
    {
        switch (spellNumber)
        {
            // High Jump
            case 0:
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
                return new Spell(highJump.gesture.id, highJump.name, highJump.mana, highJumpCast);
           
            // Fast Fall
            case 1:
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
                return new Spell(fastFall.gesture.id, fastFall.name, fastFall.mana, fastFallCast);

            // Block
            case 2:
                PlayerSpellsData.BaseSpell block = _player.spellsData.block;
                Action blockCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        _player.stateHandler.EnableState(EPlayerState.Blocking);
                    }
                };
                return new Spell(block.gesture.id, block.name, block.mana, blockCast);

            // Reflect
            case 3:
                PlayerSpellsData.Aura reflect = _player.spellsData.reflect;
                Action reflectCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        _player.stateHandler.EnableState(EPlayerState.Reflecting);
                        _player.reflectingDuration = reflect.duration;
                    }
                };
                return new Spell(reflect.gesture.id, reflect.name, reflect.mana, reflectCast);
            
            // Fireball
            case 4:
                Action checkFtueStep = () =>
                {
                    if (GameManager.Instance.ftueController.ftueStepIndex == 6)
                        GameManager.Instance.ftueController.NextFtueStep();
                };
                return LoadAttackSpell(_player.spellsData.fireball, EAttackSpellType.Projectile, checkFtueStep);
            
            // Fireball Heavy
            case 5: return LoadAttackSpell(_player.spellsData.fireballHeavy, EAttackSpellType.Projectile);

            // Ice
            case 6: return LoadAttackSpell(_player.spellsData.ice, EAttackSpellType.Projectile);

            // Ice Heavy
            case 7: return LoadAttackSpell(_player.spellsData.iceHeavy, EAttackSpellType.Projectile);
            
            // Earth
            case 8: return LoadAttackSpell(_player.spellsData.earth, EAttackSpellType.Projectile);

            // Earth Heavy
            case 9: return LoadAttackSpell(_player.spellsData.earthHeavy, EAttackSpellType.Projectile);
            
            // Wind
            case 10: return LoadAttackSpell(_player.spellsData.wind, EAttackSpellType.Projectile);

            // Wind Heavy
            case 11: return LoadAttackSpell(_player.spellsData.windHeavy, EAttackSpellType.Projectile);

            // Lighting
            case 12: return LoadAttackSpell(_player.spellsData.lightning, EAttackSpellType.Instant);

            // Lightning Heavy
            case 13: return LoadAttackSpell(_player.spellsData.lightningHeavy, EAttackSpellType.Instant);

            // Nature
            case 14: return LoadAttackSpell(_player.spellsData.nature, EAttackSpellType.Projectile);
            
            // Nature Heavy
            case 15: return LoadAttackSpell(_player.spellsData.natureHeavy, EAttackSpellType.Projectile);

            // Water
            case 16: return LoadAttackSpell(_player.spellsData.water, EAttackSpellType.Projectile);
            
            // Water Heavy
            case 17: return LoadAttackSpell(_player.spellsData.waterHeavy, EAttackSpellType.Projectile);
            
            default:
                Debug.LogError("There are missing spells for the gesture patterns");
                return new Spell();
        }
    }
    
    private Spell LoadAttackSpell(PlayerSpellsData.AttackSpell attackSpell, EAttackSpellType attackSpellType, Action extraAction = null)
    {
        Action attackSpellCast = () =>
        {
            if (lockedCasting == false)
            {
                _spellToShoot = attackSpell.spellObject;
                _player.drawArea.raycastTarget = false;
                _player.spellToShootType = attackSpellType;
                _player.stateHandler.EnableState(EPlayerState.ReadyToShoot);
                // ReSharper disable once UseNullPropagation
                if (extraAction != null)
                    extraAction();
            }
        };
        
        return new Spell(attackSpell.gesture.id, attackSpell.name, attackSpell.mana, attackSpellCast);
    }
}
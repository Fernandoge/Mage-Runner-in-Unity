using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler
{
    public bool isReflecting;
    public bool isJumping;
    public bool isHighJumping;
    public bool isBlocking;
    public bool readyToShoot;

    private PlayerController _player;

    public PlayerStateHandler()
    {
        _player = GameManager.Instance.player;
    }

    public void EnableState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Jumping:
                isJumping = true;
                _player.rigidBody.velocity = Vector2.up * _player.jumpForce;
                _player.jumpAvailable = false;
                _player.animator.SetBool("Jump", true);
                break;
            
            case EPlayerState.Running:
                _player.jumpAvailable = true;
                _player.rigidBody.gravityScale = _player.originalGravity;
                _player.stateHandler.DisableState(EPlayerState.HighJumping);
                _player.animator.SetInteger("StateNumber", 1);
                _player.animator.SetBool("Jump", false);
                break;
            
            case EPlayerState.HighJumping:
                _player.jumpAvailable = false;
                _player.stateHandler.isHighJumping = true;
                _player.animator.SetInteger("StateNumber", 2);
                break;
            
            case EPlayerState.Gliding:
                _player.rigidBody.gravityScale = 0;
                _player.rigidBody.velocity = Vector2.down * _player.glideSpeed;
                _player.animator.SetInteger("StateNumber", 3);
                break;
            
            case EPlayerState.FastFall:
                _player.jumpAvailable = false;
                _player.rigidBody.gravityScale = _player.originalGravity;
                _player.stateHandler.DisableState(EPlayerState.HighJumping);
                _player.stateHandler.DisableState(EPlayerState.Jumping);
                _player.animator.SetInteger("StateNumber", 4);
                break;
            
            case EPlayerState.ReadyToShoot:
                readyToShoot = true;
                _player.animator.SetInteger("StateNumber", 5);
                _player.animator.SetBool("ReadyToShoot", true);
                break;
            
            case EPlayerState.ReadyToShootSimplified:
                readyToShoot = true;
                _player.animator.SetInteger("StateNumber", 5);
                _player.animator.SetBool("ReadyToShootSimplified", true);
                break;
            
            case EPlayerState.Reflecting:
                isReflecting = true;
                _player.gestureSpells.lockedCasting = true;
                _player.reflectAura.SetActive(true);
                _player.animator.SetInteger("StateNumber", 7);
                _player.animator.SetBool("Reflecting", true);
                break;
            
            case EPlayerState.Blocking:
                _player.gestureSpells.lockedCasting = true;
                _player.animator.SetInteger("StateNumber", 8);
                _player.animator.SetBool("Blocking", true);
                break;
        }
    }

    public void DisableState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Jumping:
                isJumping = false;
                _player.animator.SetBool("Jump", false);
                break;
            
            case EPlayerState.HighJumping:
                _player.stateHandler.isHighJumping = false;
                break;
            
            case EPlayerState.Reflecting:
                isReflecting = false;
                _player.gestureSpells.lockedCasting = false;
                _player.reflectAura.SetActive(false);
                _player.animator.SetBool("Reflecting", false);
                break;
            
            case EPlayerState.Blocking:
                isBlocking = false;
                _player.gestureSpells.lockedCasting = false;
                _player.animator.SetBool("Blocking", false);
                break;
            
            case EPlayerState.ReadyToShoot:
                readyToShoot = false;
                _player.animator.SetInteger("StateNumber", 6);
                _player.animator.SetBool("ReadyToShoot", false);
                break;
            
            case EPlayerState.ReadyToShootSimplified:
                readyToShoot = false;
                _player.animator.SetInteger("StateNumber", 6);
                _player.animator.SetBool("ReadyToShootSimplified", false);
                break;
            
            case EPlayerState.FastFall:
                break;
            
            case EPlayerState.Running:
                break;
            
            case EPlayerState.Gliding:
                break;
        }
    }
}

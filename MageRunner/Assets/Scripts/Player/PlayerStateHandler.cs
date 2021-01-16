using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler
{
    public bool isJumping;
    public bool isHighJumping;
    public bool isFastFalling;
    public bool isBlocking;

    private PlayerController _player;

    public PlayerStateHandler(PlayerController player)
    {
        _player = player;
    }

    public void EnableState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Jumping:
                isJumping = true;
                _player.rigidBody.velocity = Vector2.up * _player.jumpForce;
                _player.jumpAvailable = false;
                _player.jumpStillPressed = true;
                _player.animator.SetBool("Jumping", true);
                break;
            
            case EPlayerState.Running:
                _player.jumpAvailable = true;
                _player.rigidBody.gravityScale = _player.originalGravity;
                DisableState(EPlayerState.HighJumping);
                DisableState(EPlayerState.FastFalling);
                _player.animator.SetInteger("StateNumber", 1);
                _player.animator.SetBool("Jumping", false);
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
            
            case EPlayerState.FastFalling:
                isFastFalling = true;
                _player.jumpAvailable = false;
                _player.rigidBody.gravityScale = _player.originalGravity;
                DisableState(EPlayerState.HighJumping);
                DisableState(EPlayerState.Jumping);
                _player.animator.SetInteger("StateNumber", 4);
                break;
            
            case EPlayerState.Shooting:
                DisableState(EPlayerState.Blocking);
                DisableState(EPlayerState.Blinking);
                _player.animator.SetInteger("StateNumber", 5);
                _player.animator.SetBool("Shooting", true);
                break;

            case EPlayerState.Blinking:
                _player.animator.SetInteger("StateNumber", 6);
                _player.animator.SetBool("Blinking", true);
                break;
            
            case EPlayerState.Blocking:
                DisableState(EPlayerState.Blinking);
                _player.animator.SetInteger("StateNumber", 7);
                _player.animator.SetBool("Blocking", true);
                break;
            
            case EPlayerState.Idle:
                _player.animator.SetBool("Idle", true);
                break;
        }
    }

    public void DisableState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Jumping:
                isJumping = false;
                _player.animator.SetBool("Jumping", false);
                break;
            
            case EPlayerState.HighJumping:
                _player.stateHandler.isHighJumping = false;
                break;
            
            case EPlayerState.Blinking:
                _player.animator.SetBool("Blinking", false);
                break;
            
            case EPlayerState.Blocking:
                isBlocking = false;
                _player.animator.SetBool("Blocking", false);
                break;
            
            case EPlayerState.Shooting:
                _player.animator.SetBool("Shooting", false);
                break;

            case EPlayerState.FastFalling:
                isFastFalling = false;
                break;
            
            case EPlayerState.Running:
                break;
            
            case EPlayerState.Gliding:
                break;
            
            case EPlayerState.Idle:
                _player.animator.SetBool("Idle", false);
                break;
        }
    }
}

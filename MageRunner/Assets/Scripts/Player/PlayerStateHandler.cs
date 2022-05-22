using System;
using UnityEngine;
using Random = System.Random;

namespace MageRunner.Player
{
    public class PlayerStateHandler
    {
        public bool isHighJumping;
        public bool isFastFalling;
        public bool isBlocking;
        public bool isDashing;

        private PlayerController _player;
        private Camera _mainCamera;

        public PlayerStateHandler(PlayerController player)
        {
            _player = player;
            _mainCamera = Camera.main;
        }

        public void EnableState(EPlayerState state)
        {
            switch (state)
            {
                case EPlayerState.GroundJumping:
                    _player.rigidBody.velocity = Vector2.up * _player.groundJumpForce;
                    _player.groundJumpAvailable = false;
                    break;
                
                case EPlayerState.AirJumping:
                    _player.rigidBody.velocity = Vector2.up * _player.airJumpForce;
                    _player.airJumpAvailable = false;
                    DisableState(EPlayerState.FastFalling);
                    DisableState(EPlayerState.Blocking);
                    _player.animator.SetTrigger("AirJump");
                    _player.animator.SetInteger("StateNumber", 8);
                    break;
            
                case EPlayerState.FastFalling:
                    DisableState(EPlayerState.HighJumping);
                    isFastFalling = true;
                    _player.groundJumpAvailable = false;
                    _player.rigidBody.gravityScale = _player.originalGravity;
                    _player.animator.SetInteger("StateNumber", 4);
                    break;
                
                case EPlayerState.Running:
                    DisableState(EPlayerState.HighJumping);
                    DisableState(EPlayerState.FastFalling);
                    _player.groundJumpAvailable = true;
                    _player.airJumpAvailable = true;
                    _player.highJumpAvailable = true;
                    _player.rigidBody.gravityScale = _player.originalGravity;
                    _player.animator.SetInteger("StateNumber", 1);
                    break;

                case EPlayerState.Dashing:
                    DisableState(EPlayerState.HighJumping);
                    DisableState(EPlayerState.FastFalling);
                    isDashing = true;
                    _player.rigidBody.gravityScale = 0;
                    _player.rigidBody.velocity = Vector2.zero;
                    _player.animator.SetInteger("StateNumber", 6);
                    _player.animator.SetBool("Dashing", true);
                    break;
                
                case EPlayerState.Blocking:
                    isBlocking = true;
                    _player.animator.SetInteger("StateNumber", 7);
                    _player.animator.SetBool("Blocking", true);
                    break;
                
                case EPlayerState.HighJumping:
                    _player.highJumpAvailable = false;
                    _player.groundJumpAvailable = false;
                    _player.airJumpAvailable = false;
                    isHighJumping = true;
                    _player.animator.SetInteger("StateNumber", 2);
                    break;
            
                case EPlayerState.Gliding:
                    _player.rigidBody.gravityScale = 0;
                    _player.rigidBody.velocity = Vector2.down * _player.glideSpeed;
                    _player.animator.SetInteger("StateNumber", 3);
                    break;

                case EPlayerState.Shooting:
                    DisableState(EPlayerState.Blocking);
                    _player.animator.SetInteger("ShootIndex", UnityEngine.Random.Range(0, 2));
                    _player.animator.SetTrigger("Shoot");
                    _player.animator.SetInteger("StateNumber", 5);
                    break;

                case EPlayerState.Idle:
                    DisableState(EPlayerState.Dashing);
                    _player.animator.SetBool("Idle", true);
                    break;
            }
        }

        public void DisableState(EPlayerState state)
        {
            switch (state)
            {
                case EPlayerState.FastFalling:
                    isFastFalling = false;
                    break;

                case EPlayerState.Dashing:
                    isDashing = false;
                    _player.rigidBody.gravityScale = _player.originalGravity;
                    _player.animator.SetBool("Dashing", false);
                    break;
                
                case EPlayerState.Blocking:
                    isBlocking = false;
                    _player.animator.SetBool("Blocking", false);
                    break;
                
                case EPlayerState.HighJumping:
                    isHighJumping = false;
                    break;

                case EPlayerState.Idle:
                    _player.animator.SetBool("Idle", false);
                    break;
            }
        }

        public void DisableAllStates()
        {
            foreach (EPlayerState playerState in Enum.GetValues(typeof(EPlayerState)))
                DisableState(playerState);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using MageRunner.Dialogues;
using MageRunner.Enemies;
using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class CheckpointController : MonoBehaviour
    {
        [SerializeField] private Vector2 _movingObjectsPosition;
        [SerializeField] private Vector2 _playerSpawnPosition;

        private int _gesturesHolderIndexAtActivation;
        private bool _wasSpawnedBefore;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                ActivateCheckpoint();
        }

        private void ActivateCheckpoint()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.level.currentCheckpoint = this;
            _gesturesHolderIndexAtActivation = GameManager.Instance.level.currentGesturesHolderIndex;
        }

        public void SpawnPlayerInCheckpoint()
        {
            // Remove gestures holders behind the checkpoint and reset active flying enemies if it's the first time spawning in this checkpoint
            if (_wasSpawnedBefore == false)
            {
                GameManager.Instance.level.gesturesHolders.RemoveRange(0, _gesturesHolderIndexAtActivation);
                foreach (GesturesHolderController flyingEnemyGesturesHolderController in GameManager.Instance.level.flyingEnemiesGesturesHolderController)
                    flyingEnemyGesturesHolderController.ResetOriginalPosition();
                
                _wasSpawnedBefore = true;
            }
            
            // Clear lists, disable player states and reset backgrounds positions
            GameManager.Instance.activeGestures.Clear();
            GameManager.Instance.ResetGestures();
            GameManager.Instance.ToggleCinematicMode(false);
            GameManager.Instance.ResetCameraPosition();
            GameManager.Instance.player.stateHandler.DisableAllStates();
            GameManager.Instance.player.Running();
            GameManager.Instance.player.companionChatBubble.ForceClose();
            foreach (RepeatingBG bg in GameManager.Instance.level.movingBgs.OfType<RepeatingBG>())
                bg.ResetOriginalValues();
            
            // Set the checkpoint spawn positions
            GameManager.Instance.level.movingObjects.transform.position = _movingObjectsPosition;
            GameManager.Instance.player.transform.localPosition = _playerSpawnPosition;
            GameManager.Instance.transform.localPosition = new Vector3(0f,0f,-10f);

            // Reset gestures holders
            foreach (GesturesHolderController gesturesHolderController in GameManager.Instance.level.gesturesHolders)
            {
                gesturesHolderController.gesturesLoaded = false;
                gesturesHolderController.gameObject.SetActive(false);
                gesturesHolderController.ResetOriginalPosition();
                foreach (Gesture gesture in gesturesHolderController.gestures)
                    gesture.iconRenderer.gameObject.SetActive(false);
            }
        }
    }
}

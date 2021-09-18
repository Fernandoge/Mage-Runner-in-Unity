using System.Collections.Generic;
using System.Linq;
using MageRunner.Dialogues;
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

        private void OnTriggerEnter2D(Collider2D other)
        {
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
            // Remove gestures holders behind the checkpoint if it's the first time spawning in this checkpoint
            if (_wasSpawnedBefore == false)
            {
                GameManager.Instance.level.gesturesHolders.RemoveRange(0, _gesturesHolderIndexAtActivation);
                _wasSpawnedBefore = true;
            }
            
            // Clear lists, disable player states and reset backgrounds positions
            GameManager.Instance.activeGestures.Clear();
            GameManager.Instance.ResetGestures();
            GameManager.Instance.ToggleCinematicMode(false);
            GameManager.Instance.player.stateHandler.DisableAllStates();
            GameManager.Instance.player.Running();
            GameManager.Instance.player.companionChatBubble.ForceClose();
            foreach (RepeatingBG bg in GameManager.Instance.level.movingBgs.OfType<RepeatingBG>())
                bg.ResetOriginalValues();
            
            // Set the checkpoint spawn positions
            GameManager.Instance.level.movingObjects.transform.position = _movingObjectsPosition;
            GameManager.Instance.player.transform.localPosition = _playerSpawnPosition;
            Camera.main.transform.localPosition = new Vector3(0f,0f,-10f);
            
            // TODO: Reset gestures holders to their original position
            // Load new gestures and deactivate gestures holders
            foreach (GesturesHolderController gesturesHolderController in GameManager.Instance.level.gesturesHolders)
            {
                gesturesHolderController.gameObject.SetActive(false);
                gesturesHolderController.LoadGestures();
            }
        }
    }
}

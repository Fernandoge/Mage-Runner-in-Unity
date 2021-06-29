﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GestureRecognizer;
using System.Linq;
using MageRunner.Managers.GameManager;
using MageRunner.Player;

public class GestureHandler : MonoBehaviour {

	public PlayerController player;

	public Text textResult;
	public DrawDetector detector;
	public Transform referenceRoot;

	GesturePatternDraw[] references;

	void Start () {
		references = referenceRoot.GetComponentsInChildren<GesturePatternDraw> ();
	}

	void ShowAll(){
		for (int i = 0; i < references.Length; i++) {
			references [i].gameObject.SetActive (true);
		}
	}

	public void OnRecognize(RecognitionResult result)
	{
		if (GameManager.Instance.level.isMoving == false && GameManager.Instance.player.idleCastEnabled == false)
			return;
		
		StopAllCoroutines ();
		ShowAll ();
		detector.ClearLines();
		if (result != RecognitionResult.Empty) {
			textResult.text = result.gesture.id + "\n" + Mathf.RoundToInt (result.score.score * 100) + "%" + "\n" + string.Format("{0:0.000}s", result.recognitionTime);
			// StartCoroutine (Blink (result.gesture.id));
			player.BeginCastingSpell(result.gesture.id);
		} else {
			textResult.text = "?";
			player.CastingSpellFailed();
		}
	}

	IEnumerator Blink(string id){
		var draw = references.Where (e => e.pattern.id == id).FirstOrDefault ();
		if (draw != null) {
			var seconds = new WaitForSeconds (0.1f);
			for (int i = 0; i <= 20; i++) {
				draw.gameObject.SetActive (i % 2 == 0);
				yield return seconds;
			}
			draw.gameObject.SetActive (true);
		}
	}

}

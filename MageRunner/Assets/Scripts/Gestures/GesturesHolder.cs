using System;
using System.Collections.Generic;
using GestureRecognizer;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GesturesHolder : MonoBehaviour
{
    public List<Gesture> gestures = new List<Gesture>();
    
    [Header("")]
    [SerializeField] private float _distanceToSpawn;
    [SerializeField] private int _healthpoints;
    [SerializeField] private Transform _healthpointsBarHolder;
    
    private float _currentHealthpoints;
    
    public float DistanceToSpawn => _distanceToSpawn;


    private void Awake()
    {
        _currentHealthpoints = _healthpoints;
        LoadGestures();
    }
    
    // public virtual void HandleAttack(int attackDamage, EElement attackElement)
    // {
    //     // _currentHealthpoints -= attackDamage * GameManager.Instance.elementsMultipliersDict[(_element, attackElement)];
    //     float barValue = _currentHealthpoints / _healthpoints;
    //     _healthpointsBarHolder.localScale = new Vector3(barValue, _healthpointsBarHolder.localScale.y, _healthpointsBarHolder.localScale.z);
    //
    //     if (_healthpointsBarHolder.parent.gameObject.activeSelf == false)
    //         _healthpointsBarHolder.parent.gameObject.SetActive(true);
    //     
    //     if (_currentHealthpoints <= 0)
    //         DestroyGameObject();
    // }

    public virtual void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void LoadGestures()
    {
        foreach (Gesture gesture in gestures.ToArray())
        {
            gestures.Remove(gesture);
            GesturePattern pattern = PickRandomGesture(gesture.difficulty);
            gesture.iconRenderer.sprite = pattern.icon;
            Gesture loadedGesture = new Gesture(gesture.spell, gesture.difficulty, gesture.iconRenderer, pattern, transform, false);
            gestures.Add(loadedGesture);
        }
    }

    public void ActivateGestures()
    {
        foreach (Gesture gesture in gestures)
        {
            GameManager.Instance.activeGestures.Add(gesture);
        }
    }
    
    public GesturePattern PickRandomGesture(EGestureDifficulty difficulty)
    {
        switch (difficulty)
        {
            case EGestureDifficulty.Easy:
                GesturePattern[] easyGestures = GameManager.Instance.gesturesDifficultyData.easy;
                return easyGestures[Random.Range(0, easyGestures.Length)];
            case EGestureDifficulty.Medium:
                GesturePattern[] mediumGestures = GameManager.Instance.gesturesDifficultyData.medium;
                return mediumGestures[Random.Range(0, mediumGestures.Length)];
            default:
                return ScriptableObject.CreateInstance<GesturePattern>();
        }
    }
}

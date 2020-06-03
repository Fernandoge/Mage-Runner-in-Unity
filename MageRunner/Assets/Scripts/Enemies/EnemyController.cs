﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _distanceToSpawn;
    [SerializeField] private float _minDistanceToShoot; [SerializeField] private float _maxDistanceToShoot;
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private EnemyAttack _objectToShoot;

    private float _fireRate;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        _fireRate = Random.Range(_minFireRate, _maxFireRate);
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y <= -0.9f)
                {
                    Destroy(gameObject);
                    GameManager.Instance.player.rigidBody.velocity = Vector2.up * GameManager.Instance.player.jumpForce;
                    GameManager.Instance.player.Running();
                }
                else
                {
                    GameManager.Instance.level.ResetLevel();
                }
            }
        }
    }

    private void Update()
    {
        float distanceBetweenPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

        if (spriteRenderer.enabled == false && distanceBetweenPlayer <= _distanceToSpawn)
        {
            spriteRenderer.enabled = true;
            boxCollider.enabled = true;
        }

        if (distanceBetweenPlayer >= _minDistanceToShoot && distanceBetweenPlayer <= _maxDistanceToShoot)
        {
            if (_fireRate > 0)
                _fireRate -= Time.deltaTime;
            else
                Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 lookDirection = GameManager.Instance.player.transform.position - _weapon.transform.position;
        float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        float lookAngleRandomized = lookAngle + Random.Range(-20, 20);
        _weapon.transform.rotation = Quaternion.Euler(0f, 0f, lookAngleRandomized);

        GameObject shootedObject = Instantiate(_objectToShoot.gameObject, _weapon.transform.position, _weapon.transform.rotation, transform.parent);
        shootedObject.GetComponent<Rigidbody2D>().velocity = _weapon.transform.right * _objectToShoot.speed;

        _fireRate = Random.Range(_minFireRate, _maxFireRate);
    }
}

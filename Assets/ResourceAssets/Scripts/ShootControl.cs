using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShootControl : MonoBehaviour
{

    private Animator _selfAnim;

    public AudioSource gunSource;

    public AudioClip powerupSound;
    public AudioClip oneshotSound;
    public AudioClip burstshotSound;
    public AudioClip killedSound;

    public EnemyManager enemyManager;

    public float BurstStep;

    public GameObject muzzleFlash;
    public GameObject bulletDecal;
    public GameObject decalContainer;

    private GameObject _flash;
    private GameObject _particle;
    private GameObject _muzzleObj;
    private GameManager gameManager;


    private void Start()
    {
        _selfAnim = GetComponent<Animator>();
        _muzzleObj = Instantiate<GameObject>(muzzleFlash);
        _muzzleObj.transform.SetParent(gunSource.transform);
        _flash = _muzzleObj.transform.Find("MuzzleFlash").gameObject;
        _particle = _muzzleObj.transform.Find("Particles").gameObject;
        _muzzleObj.transform.SetLocalPositionAndRotation(new Vector3(1.227f, -0.035f, 0.187f), Quaternion.Euler(new Vector3(0, -90, 0)));
        _muzzleObj.SetActive(false);

        gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.IsPause = true;
    }


    void Update()
    {
        if (gameManager.IsPause || gameManager.IsPlayerDie)
            return;
        if (!gameManager.IsPowerup && Vector3.Distance(gameManager.starUp.transform.position, transform.position) <= 3) 
        {
            Powerup();
        }
        if (Input.GetMouseButtonDown(0))
        {
            FlashReactivate();
            if (gameManager.IsPowerup)
            {
                gunSource.clip = burstshotSound;
                _selfAnim.SetTrigger("BurstShot");
                transform.position -= transform.forward * Time.deltaTime * BurstStep;
                _flash.SetActive(true);
                _particle.SetActive(true);
            }
            else
            {
                gunSource.clip = oneshotSound;
                _selfAnim.SetTrigger("OneShot");
                _flash.SetActive(false);
                _particle.SetActive(false);
            }
            gunSource.Play();

            RaycastHit hit;
            Vector3 camPosition = Camera.main.transform.position;
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            Vector3 camUp = Camera.main.transform.up;
            Physics.Raycast(camPosition, camForward, out hit, Mathf.Infinity);
            if (gameManager.IsPowerup)
            {
                RaycastHit hitRight;
                RaycastHit hitLeft;
                RaycastHit hitUp;
                RaycastHit hitDown;
                Physics.Raycast(camPosition, camForward + camRight * 0.05f, out hitRight, Mathf.Infinity);
                Physics.Raycast(camPosition, camForward - camRight * 0.05f, out hitLeft, Mathf.Infinity);
                Physics.Raycast(camPosition, camForward + camUp * 0.05f, out hitUp, Mathf.Infinity);
                Physics.Raycast(camPosition, camForward - camUp * 0.05f, out hitDown, Mathf.Infinity);

                ShootHit(hitRight);
                ShootHit(hitLeft);
                ShootHit(hitUp);
                ShootHit(hitDown);
            }

            ShootHit(hit);
        }

    }
    public void Powerup()
    {
        gameManager.IsPowerup = true;
        _selfAnim.SetTrigger("Powerup");
        gunSource.clip = powerupSound;
        gunSource.Play();
    }

    public void Die()
    {
        gunSource.clip = killedSound;
        gunSource.Play();
    }

    private void ShootHit(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Combined Mesh"))
            {
                PaintBulletHole(hit.point, hit.normal, hit.collider);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                enemyManager.Die(hit.transform);
            }

            if (hit.collider.CompareTag("Boss"))
            {
                enemyManager.BossHit();
            }
        }
    }


    private void FlashReactivate()
    {
        _muzzleObj.SetActive(false);
        _muzzleObj.SetActive(true);
    }

    private void PaintBulletHole(Vector3 point, Vector3 normal, Collider collider)
    {
        GameObject decal = Instantiate(bulletDecal, point, Quaternion.identity);
        decal.transform.parent = decalContainer.transform;
        DecalProjector projector = decal.GetComponent<DecalProjector>();
        decal.transform.forward = -normal;
        decal.transform.position += normal.normalized * 0.3f;
        projector.size = new Vector3(0.3f, 0.3f, 1f);
    }

    public void DestroyAllDecals()
    {
        var totalCount = decalContainer.transform.childCount;
        for (int i = 0; i < totalCount; i++)
        {
            Destroy(decalContainer.transform.GetChild(i).gameObject);
        }


    }

}

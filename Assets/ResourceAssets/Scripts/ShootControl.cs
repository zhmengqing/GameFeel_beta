
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
    private float _shootTime;


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
        _shootTime = 0;
    }


    void Update()
    {
        if (gameManager.IsPause || gameManager.IsPlayerDie)
            return;
        if (!gameManager.IsPowerup && Vector3.Distance(gameManager.starUp.transform.position, transform.position) <= 3) 
        {
            Powerup();
        }
        var curShootTime = Time.fixedTime - _shootTime;
        if ((Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Q)) && curShootTime > 0.1f) 
        {
            _shootTime = Time.fixedTime;
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

            var delay = 0;
            Physics.Raycast(camPosition, camForward, out hit, Mathf.Infinity);
            ShootHit(hit, ref delay);
            if (gameManager.IsPowerup)
            {
                ShootRacast(camPosition, camForward, camRight, camUp, 0.05f, ref delay);
                ShootRacast(camPosition, camForward, camRight, camUp, 0.1f, ref delay);
                ShootRacast(camPosition, camForward, camRight, camUp, 0.15f, ref delay);

            }

        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            enemyManager.EnemyPosSet();
        }

    }

    private void ShootRacast(Vector3 camPosition, Vector3 camForward, Vector3 camRight, Vector3 camUp, float camOffset, ref int delay)
    {
        RaycastHit hitInfo;
        Physics.Raycast(camPosition, camForward + camRight * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward - camRight * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward + camUp * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward - camUp * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward + (camUp + camRight) * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward + (camUp - camRight) * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward - (camUp - camRight) * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
        Physics.Raycast(camPosition, camForward - (camUp + camRight) * camOffset, out hitInfo, Mathf.Infinity);
        ShootHit(hitInfo, ref delay);
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

    private void ShootHit(RaycastHit hit, ref int delay)
    {
        if (hit.collider != null)
        {
            if (delay < 5 && hit.collider.CompareTag("Combined Mesh")) 
            {
                PaintBulletHole(hit.point, hit.normal, hit.collider);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                enemyManager.Die(hit.transform, delay);
            }

            if (hit.collider.CompareTag("Boss"))
            {
                enemyManager.BossHit();
            }
            delay++;
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

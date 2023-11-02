using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{
    public GameObject chickenPrefab;
    public GameObject born;
    
    public int chickenCount;
    //总数多少只时boss出现,包括死亡
    public int bossChickenCount;
    //出生间隔
    public float bornTime;

    public Transform hero;

    public AudioClip talkClip;
    public AudioClip screamClip;
    public GameObject blow;

    //小怪达到这个数量开始提速
    public int speedUpCount = 100;

    private bool _isSpeedUp;

    //当前的小怪数量
    private int _currentNum;
    //所有小怪数量,包括已死
    private int _totalNum = 0;
    private float _waitTime;
    private GameManager _gameManager;
    private GameObject _bossBlow;
    private TextMeshProUGUI _liveTxt;
    private TextMeshProUGUI _deadTxt;
    private EnemyPosObject _enemyPos;

    private List<GameObject> _enemyPool;
    private List<GameObject> _enemyList;

    public void EnemyPosSet()
    {
        _enemyPos.triger.isDestroy = true;
    }

    private void Start()
    {
        _waitTime = Time.fixedTime;
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _bossBlow = _gameManager.boss.transform.Find("Blow").gameObject;
        _isSpeedUp = false;

        _liveTxt = _gameManager.uiManager.transform.Find("Board/LiveTxt").GetComponent<TextMeshProUGUI>();
        _deadTxt = _gameManager.uiManager.transform.Find("Board/DeadTxt").GetComponent<TextMeshProUGUI>();

        _enemyPos = GetComponent<EnemyPosObject>();
        _enemyPos.triger = new TrigerObj();
        _enemyPos.trans = new TransformObj();
        _enemyPool = new List<GameObject>();
        _enemyList = new List<GameObject>();
    }

    private void Update()
    {
        if (_gameManager.IsPause || _gameManager.IsPlayerDie)
        {
            var total = transform.childCount;
            for (int i = 0; i < total; i++)
            {
                var chicken = transform.GetChild(i).gameObject;
                AudioSource source = chicken.GetComponent<AudioSource>();
                source.enabled = false;
                var nav = chicken.GetComponent<NavMeshAgent>();
                nav.enabled = false;
            }
            return;
        }

        _currentNum = _enemyList.Count;

        if (!_gameManager.IsBossShow && Time.fixedTime - _waitTime >= bornTime)
        {
            if (_currentNum < chickenCount)
            {
                var bornCount = born.transform.childCount;
                for (int i = 0; i < bornCount; i++)
                {
                    var chicken = SpawnChicken().transform;
                    chicken.parent = transform;
                    chicken.transform.position = born.transform.GetChild(i).gameObject.transform.position;
                    var nav = chicken.GetComponent<NavMeshAgent>();
                    nav.enabled = true;
                    var random = UnityEngine.Random.Range(1f, 5f);
                    nav.speed = random;
                    nav.stoppingDistance = 4f;

                    var collider = chicken.GetComponent<Collider>();
                    collider.enabled = true;

                    var anim = chicken.GetComponent<Animator>();
                    anim.SetInteger("MoveSpeed", (int)nav.speed);
                    
                    AudioSource source = chicken.GetComponent<AudioSource>();
                    source.enabled = true;
                    if (_currentNum % 20 == 0)
                    {
                        source.loop = true;
                        source.clip = talkClip;
                        source.Play();
                    }
                    else
                    {
                        source.Pause();
                    }
                    _totalNum++;
                }

            }
            _waitTime = Time.fixedTime;
        }

        for (int i = 0; i < _currentNum; i++) 
        {
            var chicken = _enemyList[i].gameObject;
            if (chicken.CompareTag("Enemy"))
            {
                AudioSource source = chicken.GetComponent<AudioSource>();
                source.enabled = true;
                var nav = chicken.GetComponent<NavMeshAgent>();
                nav.enabled = true;
                if (!_isSpeedUp && _totalNum > speedUpCount) 
                {
                    nav.speed *= 1.5f;
                }
                if (nav.isOnNavMesh && Vector3.Distance(transform.position, hero.position) >= 5f)
                {
                    nav.destination = hero.position;
                }
            }            
        }
        _liveTxt.text = "存活：" + _currentNum;
        _deadTxt.text = "死亡：" + (_totalNum - _currentNum);
        //小怪提速
        if (_totalNum > speedUpCount)
        {
            _isSpeedUp = true;
        }
        if (!_gameManager.IsBossShow && _totalNum > bossChickenCount) 
        {
            _gameManager.ShowBoss();            
        }
        else
        {
            NavMeshAgent bossNav = _gameManager.boss.GetComponent<NavMeshAgent>();
            if (Vector3.Distance(_gameManager.boss.transform.position, hero.position) < 4)
            {
                Animator bossAnim = _gameManager.boss.GetComponent<Animator>();
                bossAnim.SetTrigger("Attack");
                var dir = _gameManager.boss.transform.position - hero.position;
                _gameManager.boss.transform.forward = -dir;
            }
            else if(bossNav.isOnNavMesh)
            {
                bossNav.speed = _isSpeedUp ? 8 : 6;
                bossNav.speed = _currentNum == 0 ? 12 : bossNav.speed;
                bossNav.destination = hero.position;
                bossNav.stoppingDistance = 3;
            }
        }


        //ChickenArrayAuthoring chickenCom = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentObject<ChickenArrayAuthoring>(GetEntity());
        //Debug.Log(chickenCom.posArr.x + " " + chickenCom.posArr.y + " " + chickenCom.posArr.z);
    }

    public void Die(Transform chicken, int delay)
    {
        //blow
        if(_gameManager.IsPowerup)
        {
            GameObject blowEff;
            blowEff = chicken.Find("Blow(Clone)")?.gameObject;
            if (blowEff == null)
            {
                blowEff = Instantiate(blow);
            }
            
            blowEff.transform.parent = chicken;
            blowEff.SetActive(true);
            blowEff.transform.localPosition = Vector3.zero;
            blowEff.transform.localScale = Vector3.one * 0.5f;
        }
        chicken.transform.tag = "Die";
        var anim = chicken.GetComponent<Animator>();
        anim.SetTrigger("Die");
        var nav = chicken.GetComponent<NavMeshAgent>();
        nav.enabled = false;
        var collider = chicken.GetComponent<Collider>();
        collider.enabled = false;
        AudioSource source = chicken.GetComponent<AudioSource>();
        source.enabled = true;
        source.loop = false;
        source.clip = screamClip;
        source.Play();
        StartCoroutine(DestroyEnemy(chicken, delay));
    }

    private IEnumerator DestroyEnemy(Transform transform, int delay)
    {
        yield return new WaitForSeconds(1f + delay * 0.3f);

        _enemyPos.trans.pos = transform.position + transform.up * 0.35f + transform.forward * 0.1f;//_totalNum - _currentNum
        _enemyPos.trans.rot = Quaternion.Euler(-95f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); 
        _enemyPos.triger.isTriger = true;
        yield return new WaitForSeconds(0.1f);
        transform.gameObject.SetActive(false);
        _enemyPool.Add(transform.gameObject);
        _enemyList.Remove(transform.gameObject);
        //Debug.Log("回收 " + transform.position.ToString());
        //Destroy(transform.gameObject);
    }

    private GameObject SpawnChicken()
    {
        GameObject spawner;
        if (_enemyPool.Count > 0) 
        {
            spawner = _enemyPool[0];
            _enemyPool.RemoveAt(0);
        }
        else
        {
            spawner = GameObject.Instantiate(chickenPrefab);
        }
        spawner.transform.tag = "Enemy";
        spawner.SetActive(true);
        _enemyList.Add(spawner);
        return spawner;
    }

    public void BossHit()
    {
        _gameManager.boss.transform.position -= _gameManager.boss.transform.forward * 0.2f;
        _bossBlow.SetActive(false);
        _bossBlow.SetActive(true);
    }

    public void DestroyAllEnemys()
    {
        while (_enemyList.Count > 0) 
        {
            _enemyList[0].SetActive(false);
            _enemyList[0].transform.tag = "Die";
            _enemyPool.Add(_enemyList[0]);
            _enemyList.RemoveAt(0);
        }            

    }

    public void ResetEnemey()
    {
        _waitTime = Time.fixedTime;
        _totalNum = 0;
        _enemyPos.triger.isDestroy = true;
    }

}

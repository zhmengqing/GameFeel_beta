using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public GameObject chickenPrefab;
    public GameObject born;
    
    public int chickenCount;
    //总数多少只时boss出现,包括死亡
    public int bossChickenCount;
    //出生间隔
    public int bornTime;

    public Transform hero;

    public AudioClip talkClip;
    public AudioClip screamClip;
    public GameObject blow;

    //小怪达到这个数量开始提速
    public int speedUpCount = 100;

    private bool _isSpeedUp;

    //当前的小怪数量
    private int _currentNum = 0;
    //所有小怪数量,包括已死
    private int _totalNum = 0;
    private float _waitTime;
    private GameManager _gameManager;
    private GameObject _bossBlow;
    private TextMeshProUGUI _liveTxt;
    private TextMeshProUGUI _deadTxt;

    private void Start()
    {
        _waitTime = Time.fixedTime;
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _bossBlow = _gameManager.boss.transform.Find("Blow").gameObject;
        _isSpeedUp = false;

        _liveTxt = _gameManager.uiManager.transform.Find("Board/LiveTxt").GetComponent<TextMeshProUGUI>();
        _deadTxt = _gameManager.uiManager.transform.Find("Board/DeadTxt").GetComponent<TextMeshProUGUI>();
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
            
        if (!_gameManager.IsBossShow && Time.fixedTime - _waitTime >= bornTime)
        {
            if (_currentNum < chickenCount)
            {
                var bornCount = born.transform.childCount;
                for (int i = 0; i < bornCount; i++)
                {
                    var chicken = Instantiate(chickenPrefab).transform;
                    chicken.parent = transform;
                    chicken.transform.position = born.transform.GetChild(i).gameObject.transform.position;
                    var nav = chicken.GetComponent<NavMeshAgent>();

                    var random = Random.Range(1f, 5f);
                    nav.speed = random;
                    nav.stoppingDistance = 4f;
                    var anim = chicken.GetComponent<Animator>();
                    anim.SetInteger("MoveSpeed", (int)nav.speed);

                    
                    AudioSource source = chicken.GetComponent<AudioSource>();
                    source.enabled = true;
                    source.loop = true;
                    if (_currentNum % 20 == 0)
                    {
                        source.clip = talkClip;
                        source.Play();
                    }
                    _totalNum++;
                }

            }
            _waitTime = Time.fixedTime;
        }


        var totalCount = transform.childCount;
        _currentNum = 0;
        for (int i = 0; i < totalCount; i++) 
        {
            var chicken = transform.GetChild(i).gameObject;
            if (chicken.CompareTag("Enemy"))
            {
                AudioSource source = chicken.GetComponent<AudioSource>();
                source.enabled = true;
                _currentNum++;
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
    }

    public void Die(Transform chicken)
    {
        //blow
        if(_gameManager.IsPowerup)
        {
            var blowEff = Instantiate(blow);
            blowEff.transform.parent = chicken;
            blowEff.SetActive(true);
            blowEff.transform.localPosition = Vector3.zero;
            blowEff.transform.localScale = Vector3.one * 0.5f;
        }

        var anim = chicken.GetComponent<Animator>();
        anim.SetTrigger("Die");
        var nav = chicken.GetComponent<NavMeshAgent>();
        nav.enabled = false;
        var collider = chicken.GetComponent<Collider>();
        collider.enabled = false;
        chicken.transform.tag = "Die";
        AudioSource source = chicken.GetComponent<AudioSource>();
        source.enabled = true;
        source.loop = false;
        source.clip = screamClip;
        source.Play();
        //StartCoroutine(DestroyEnemy(hit.transform));
    }

    public void BossHit()
    {
        _gameManager.boss.transform.position -= _gameManager.boss.transform.forward * 0.2f;
        _bossBlow.SetActive(false);
        _bossBlow.SetActive(true);
    }

    public void DestroyAllEnemys()
    {
        var totalCount = transform.childCount;
        for (int i = 0; i < totalCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
            

    }

    public void ResetEnemey()
    {
        _waitTime = Time.fixedTime;
        _totalNum = 0;
        _currentNum = 0;
    }

}

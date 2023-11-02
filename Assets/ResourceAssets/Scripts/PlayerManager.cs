using System;
using System.Collections;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerManager : MonoBehaviour
{
    public float speed;
    public float velocityDamping;
    public float jumpTime;

    public bool rotatePlayer = true;

    private Vector3 _currentVleocity;
    private float _currentJumpSpeed;
    private float _restY;

    private Animator _selfAnim;

    private AudioSource _audioSource;
    public AudioClip runningSound;
    public AudioClip jumpingSound;

    public float mouseYSpeed;
    public float mouseXSpeed;
    public PlayableDirector director;
    public CinemachineVirtualCamera vCamera;

    private GameManager gameManager;

    private float _mouseY, _mouseX;
    private float _startSpeed;
    private ShootControl shoot;
    private EnemyManager enemyManager;
    private CinemachineComposer composer;

    private void Start()
    {
        _startSpeed = speed;
        _selfAnim = GetComponent<Animator>();
        _restY = transform.position.y;

        _audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        gameManager.IsPause = true;
        gameManager.uiManager.ShowUI(Resume, true);
        director.Stop();
        shoot = GetComponent<ShootControl>();
        composer = vCamera.GetCinemachineComponent<CinemachineComposer>();

    }

    private void Update()
    {
        if (gameManager.IsPause) return;
        Vector3 fwd = transform.forward;

        fwd.y = 0;
        fwd = fwd.normalized;
        if (fwd.sqrMagnitude < 0.01f)
            return;

        if (!gameManager.IsPlayerDie)
        {

            Quaternion inputFrame = Quaternion.LookRotation(fwd, Vector3.up);
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            input = inputFrame * input;

            if (input.magnitude > 0)
            {
                if (!_audioSource.isPlaying && transform.position.y <= _restY)
                {
                    _audioSource.clip = runningSound;
                    _audioSource.Play();
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    speed = _startSpeed * 2f;
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    speed = _startSpeed;
                }
            }

            var dt = Time.deltaTime;
            var desiredVelocity = input * speed;
            var deltaVel = desiredVelocity - _currentVleocity;
            _currentVleocity += Damper.Damp(deltaVel, velocityDamping, dt);

            _selfAnim.SetBool("IsRun", input.magnitude > 0);

            transform.position += _currentVleocity * dt;

            // Process jump
            if (_currentJumpSpeed != 0)
            {
                _currentJumpSpeed -= 10 * dt;
            }
            var p = transform.position;
            p.y += _currentJumpSpeed * dt;
            if (p.y < _restY)
            {
                p.y = _restY;
                _currentJumpSpeed = 0;
            }
            transform.position = p;

            if (Input.GetKeyDown(KeyCode.Space) && p.y <= _restY)
            {
                Jump();

            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.uiManager.ShowUI(Resume);
                gameManager.IsPause = true;
            }

            _mouseX += Input.GetAxis("Mouse X") * mouseXSpeed;
            _mouseY += Input.GetAxis("Mouse Y") * mouseYSpeed;

            Quaternion rootRotation = Quaternion.Euler(0, _mouseX, 0);
            transform.rotation = rootRotation;

            var mouseY = _mouseY;
            mouseY = Mathf.Clamp(mouseY, 2.2f, 4f);

            composer.m_TrackedObjectOffset.y = mouseY;
            _selfAnim.SetInteger("AimOffset", 1);
            var aimUpDown = (mouseY - 2.2f) / 2.5f;
            _selfAnim.SetLayerWeight(1, aimUpDown);
        }
        else
        {
            director.Play();
        }

    }

    private void Jump()
    {
        _currentJumpSpeed += 10 * jumpTime * 0.5f;
        _selfAnim.SetTrigger("Jump");
        _audioSource.clip = jumpingSound;
        _audioSource.Play();
    }

    private void Resume()
    {
        gameManager.IsPause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Boss"))
        {
            gameManager.IsPlayerDie = true;
            Time.timeScale = 0.05f;
            _selfAnim.SetTrigger("Die");
            shoot.Die();
            StartCoroutine(ReStartGame());
        }
    }

    private IEnumerator ReStartGame()
    {
        StartCoroutine(gameManager.ReStart(ResetGame));

        yield return null;
    }

    private void ResetGame()
    {
        gameManager.IsPause = false;
        gameManager.IsPlayerDie = false;
        gameManager.IsPowerup = false;
        gameManager.IsBossShow = false;
        _selfAnim.SetTrigger("Restart");
        Time.timeScale = 1;
        enemyManager.RepoolAllEnemies();
        enemyManager.ResetEnemies();
        shoot.DestroyAllDecals();
        gameManager.boss.SetActive(false);
        gameManager.ResetStar();
        director.Stop();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}

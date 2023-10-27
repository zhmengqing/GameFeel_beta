
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject starUp;
    public float speed;
    public GameObject boss;

    [HideInInspector]
    public bool IsPause = true;
    [HideInInspector]
    public bool IsPowerup = false;

    [HideInInspector]
    public bool IsBossShow = false;
    [HideInInspector]
    public bool IsPlayerDie = false;



    // Start is called before the first frame update
    void Start()
    {
        boss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPause)
        {
            Time.timeScale = 0;
            if(boss.activeInHierarchy)
            {
                boss.GetComponent<AudioSource>().enabled = false;
            }
        }
        else
        {
            if (!IsPlayerDie)
                Time.timeScale = 1;
            if (boss.activeInHierarchy)
            {
                boss.GetComponent<AudioSource>().enabled = true;
            }
        }


        if (!IsPowerup)
        {
            starUp.transform.Rotate((Vector3.up * speed));
        }
        else if (starUp != null) 
        {
            starUp.SetActive(false);
        }
        
        

    }

    public IEnumerator ReStart(Action action)
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        IsPause = true;
        uiManager.ShowStart(action);
    }

    public void ShowBoss()
    {
        IsBossShow = true;
        boss.SetActive(true);
    }

    public void ResetStar()
    {
        starUp.SetActive(true);
    }
}

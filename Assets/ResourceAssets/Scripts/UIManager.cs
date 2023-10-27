using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button resumeBtn;
    public Button exitBtn;
    public Canvas uiCanvas;

    private Action resumeAction;
    // Start is called before the first frame update
    void Start()
    {
        resumeBtn.onClick.AddListener(ResumeOnClick);
        exitBtn.onClick.AddListener(ExitOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowUI(Action action, bool isStart = false)
    {
        if (!isStart)
        {
            resumeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "继续";
        }
        resumeAction = action;
        uiCanvas.transform.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowStart(Action action)
    {
        resumeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "重新开始";
        uiCanvas.transform.gameObject.SetActive(true);
        resumeAction = action;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideUI()
    {
        uiCanvas.transform.gameObject.SetActive(false);
    }

    private void ResumeOnClick()
    {
        HideUI();
        resumeAction?.Invoke();
    }
    private void ExitOnClick()
    {
        Application.Quit();
    }
}

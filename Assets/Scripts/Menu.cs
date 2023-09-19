using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject resetBtn, resumeBtn, quitBtn;
    private bool showMenu = false;

    void Start()
    {
        option_button(false);
        
    }

    private void showCursor()
    {
        Cursor.visible = true;
            
        Cursor.lockState = CursorLockMode.None;
    }
    private void option_button(bool option)
    {
        if (option)
        {
            showCursor();
        }
        showMenu = option;
        resetBtn.gameObject.SetActive(option);
        resumeBtn.gameObject.SetActive(option);
        quitBtn.gameObject.SetActive(option);
    }
    private void hideCursor()
    {
        Cursor.visible = false;
            
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void close()
    {
        resetBtn.gameObject.SetActive(false);
        resumeBtn.gameObject.SetActive(false);
        quitBtn.gameObject.SetActive(false);
        hideCursor();
    }
    public void reset_()
    {
        SceneManager.LoadScene("Room");
        close();
    }

    public void resume_()
    {
        close();
    }

    public void quit_()
    {

        Application.Quit();
        close();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (showMenu)
            {
                option_button(false);
                resume_();
            }
            {
                option_button(true);
            }
        }
    }
}

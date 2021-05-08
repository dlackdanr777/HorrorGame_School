using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

    public void StartButton()
    {
        SceneManager.LoadScene("schoolScenes"); //스쿨씬을 불러온다
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("StartScene"); //스타트씬을 불러온다
    }

    public void ExitGame()
    {
        Application.Quit(); //게임을 나간다.
    }
}

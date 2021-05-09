using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Stage_State //현재 스테이지의 상황을 보여주는 변수
{
    Start,
    Play,
    GameOver,
}
public class GameManager : MonoBehaviour
{
    public bool is_Start = true;
    public static GameManager instance;
    public Text HealthText; //체력을 보여주는 텍스트
    public Image StaminaImage; //스네미나 이미지를 넣는 변수
    public Image StaminaBar; //스테미나를 보여주는 이미지
    public Text ScoreText; //스코어를 보여주는 텍스트
    public Stage_State Stage_State; //현재 스테이지의 상황을 보여주는 변수
    public int OwnKey = 0; //몇단계의 키를 보유중인가를 받아오는 변수
    public int Score = 0; //아이템을 먹으면 올라갈 변수
    public float Health = 100f; //플레이어의 체력을 받아오는 변수
    public float Stamina = 100f; //플레이어의 스테미나를 받아오는 변수
    public float Battery_Gauge = 100f; //플래시의 배터리 잔량을 표현하는 변수

    public Text youWin;
    public Text youDie;

    private int a;



    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        instance = this;
    }

    private void Start()
    {
        is_Start = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }
    private void FixedUpdate()
    {
        HealthText.text = Mathf.Floor(Health).ToString(); //플레이어의 체력을 ui창에 보여준다
        ScoreText.text = Score + "/8";
        StaminaBar.fillAmount = Stamina * 0.01f;

        if(Health <= 0f)
        {
            is_Start = false;
            youDie.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if(Score == 8)
        {
            is_Start = false;
            youWin.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

}

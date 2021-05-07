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
    public static GameManager instance;
    public Text HealthText; //체력을 보여주는 텍스트
    public Text ScoreText; //스코어를 보여주는 텍스트
    public Stage_State Stage_State; //현재 스테이지의 상황을 보여주는 변수
    public int OwnKey = 0; //몇단계의 키를 보유중인가를 받아오는 변수
    public int Score = 0; //아이템을 먹으면 올라갈 변수
    public float Health = 100f; //플레이어의 체력을 받아오는 변수
    public float Battery_Gauge = 100f; //플래시의 배터리 잔량을 표현하는 변수



    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HealthText.text = Mathf.Floor(Health).ToString(); //플레이어의 체력을 ui창에 보여준다
        ScoreText.text = Score + "/8";
    }
}

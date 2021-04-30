using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Health_State
{
    light,
}
public class Health_Controller : MonoBehaviour
{
    public Health_State Health_State;

    //다른 클래스를 참조하는 변수
    public GameObject Player; //플레이어를 받아오는 변수
    public GameObject Light_Button; //전등버튼을 받아오는 변수
    private Player_Controller Player_Controller;
    private LightController LightController;

    //회복관련 변수
    private bool Recovering = false;

    //쿨타임 관련 변수
    private bool ResetTrigger = false;
    private float ResetCoolTime; //리셋 쿨타임


    private void Start()
    {
        Player = GameObject.Find("Player"); //플레이어를 받아온다
        Player_Controller = Player.GetComponent<Player_Controller>(); //플레이어의 클래스를 받아온다.

        LightController = Light_Button.GetComponent<LightController>(); //전등의 클래스를 받아온다.
    }


    void FixedUpdate()
    {

        HealthFnc();
        DataReset();

        
    }

    void HealthFnc() //체력관련 함수
    {
        
        if (!Player_Controller.is_Under_Attack) //공격받는 중이 아니거나
        {
            if (Recovering && LightController.is_TurnOn) //체력 회복 범위에 있고 전등이 켜진상태라면?
            {
                if (Player_Controller.Health < 100) //체력이 100보다 적을경우
                {
                    Player_Controller.Health += Time.deltaTime * 0.5f; //체력을 초당 0.5씩 회복한다.
                    Debug.Log("체력회복");
                }
                else
                {
                    Player_Controller.Health = 100f;
                }
                Debug.Log("쉬는중");
            }
        }

    }



    private void OnTriggerStay(Collider other)
    {
        if(Health_State == Health_State.light) //전등일경우
        {
            if(other.gameObject.tag == "Player") //플레이어가 해당범위 안에 있을경우?
            {
                Recovering = true; //회복중 변수를 참으로 만든다.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Health_State == Health_State.light) //전등일경우
        {
            if (other.gameObject.tag == "Player") //플레이어가 해당범위 밖에 있을경우
            {
                Recovering = false; //회복중 변수를 거짓으로 만든다.
            }
        }

    }


    void DataReset() //컨트롤을 초기화하는 함수
    {
        if (Recovering && !ResetTrigger) // 컨트롤 가능하고 리셋트리거가 작동되지않았을때
        {
            ResetTrigger = true; //리셋트리거를 작동시킨다.
            ResetCoolTime = 0; //해당변수를 초기화한다.

        }

        else if (ResetTrigger) //리셋트리거가 작동 했을 때
        {

            ResetCoolTime += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime > 1f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                Recovering = false; //회복중을 끈다.
            }
        }
    }
}


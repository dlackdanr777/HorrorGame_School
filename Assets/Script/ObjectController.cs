using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


public class ObjectController : MonoBehaviour
{
    [HideInInspector]
    public bool PossibleState = false; //현재 문이 컨트롤 가능한 상태인지 입력받는 변수
    [HideInInspector]
    public bool is_Identify = false; //물체를 식별중인지 아닌지 입력받는 변수

    public AudioClip OpenSound;
    public AudioClip CloseSound;


    //플레이어 관련 변수
    private GameObject Player; //플레이어 캐릭터를 지정하는 변수
    private Player_Controller Player_Controller;

    //원래정보를 저장할 변수
    private Vector3 Save_Position; //위치를 저장한다.
    private Quaternion Save_Rotation; //회전값을 저장한다.
    private GameObject CopyObj;

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////

    private void Start()
    {
        Player = GameObject.Find("Player"); //플레이어를 찾아 넣어준다
        Player_Controller = Player.GetComponent<Player_Controller>(); //플레이어의 스크립트를 받아온다.
    }


    private void FixedUpdate()
    {
        IdentifyObj();

        CoolTimeSet();
        DataReset();
    }

    void IdentifyingObj()
    {
        if(is_Identify && (Player_Controller.Player_State != (int)Player_Controller.State.is_Identify)) //만약 물체가 식별당하고있을 경우
        {

        }
    }


    void IdentifyObj ()
    {

        if (is_Identify)
        {
            Debug.Log("1");
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("2");
                if (!SetTrigger)
                {
                    Debug.Log("3");
                    SetTrigger = true;
                    if (Player_Controller.Player_State == (int)Player_Controller.State.is_Identify) //만약 식별중일경우
                    {
                        Debug.Log("실행됨");
                        Player_Controller.Player_State = (int)Player_Controller.State.is_Stop; //식별중을 끈다.
                        is_Identify = false; //식별중을 끈다.

                        transform.position = Save_Position; //원래있던자리에 둔다
                    }
                }
            }

        }
       


        else if (PossibleState)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger)
                {
                    SetTrigger = true;
                    if(Player_Controller.Player_State != (int)Player_Controller.State.is_Identify) //만약 플레이어가 식별중이 아닐경우
                    {
                        Player_Controller.Player_State = (int)Player_Controller.State.is_Identify; //식별중으로 바꾼다.
                        is_Identify = true; //식별중으로 바꾼다.
                        Save_Position = transform.localPosition; //물체의 위치값을 저장한다.
                        Save_Rotation = transform.localRotation; //물체의 회전값을 저장한다.

                        Debug.Log("실행됨");

                        transform.position = Player_Controller.MainCamera.transform.position + Player_Controller.MainCamera.transform.forward; // 카메라앞에 물체를 둔다.
                    }

                    
                }
            }
        }

        
    }








    void CoolTimeSet()
    {
        if (SetTrigger) //쿨타임 설정
        {
            CoolTime += Time.deltaTime;
            if (CoolTime > SetCooltime)
            {
                CoolTime = 0;
                SetTrigger = false;
            }
        }
    }

    void DataReset() //컨트롤을 초기화하는 함수
    {
        if (PossibleState && !ResetTrigger) // 컨트롤 가능하고 리셋트리거가 작동되지않았을때
        {
            ResetTrigger = true; //리셋트리거를 작동시킨다.

        }

        else if (ResetTrigger) //리셋트리거가 작동 했을 때
        {

            ResetCoolTime += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime > 0.1f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                PossibleState = false; //컨트롤 가능한 상태를 끈다.
            }
        }
    }
}

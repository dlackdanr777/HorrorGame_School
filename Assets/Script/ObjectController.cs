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


    //플레이어 관련 변수
    private GameObject Player; //플레이어 캐릭터를 지정하는 변수
    private Player_Controller Player_Controller;



    //원래정보를 저장할 변수
    private Vector3 Save_Position; //위치를 저장한다.
    private Quaternion Save_Rotation; //회전값을 저장한다.
    private float DragSpeed = 15f;
    private float Scroll =0.5f;

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////
    private void Start()
    {
        Save_Position = transform.localPosition; //변수를 초기화한다
        Save_Rotation = transform.localRotation;//변수를 초기화한다
        Player = GameObject.Find("Player"); //플레이어를 찾아 넣어준다
        Player_Controller = Player.GetComponent<Player_Controller>(); //플레이어의 스크립트를 받아온다.

    }


    private void FixedUpdate()
    {
        IdentifyingObj();//식별중일때 실행되는 함수
        IdentifyObj(); //플레이어 레이캐스트와 오브젝트가 충돌했을때 실행되는 함수

        CoolTimeSet();
        DataReset();

    }

    void IdentifyingObj() //식별중일때 실행되는 함수
    {
        if(is_Identify && (Player_Controller.is_Identify)) //만약 물체가 식별당하고있을 경우
        {

            Vector3 velo = Vector3.zero;
            //오브젝트를 카메라 앞쪽으로 이동시킨다.
            transform.position = Vector3.SmoothDamp(transform.position, Player_Controller.MainCamera.transform.position + (Player_Controller.MainCamera.transform.forward * Scroll), ref velo, 0.05f);

            if (Input.GetMouseButton(0)) //마우스 왼쪽버튼을 누르고 있으면
            {
                transform.Rotate(0f, -Input.GetAxis("Mouse X") * DragSpeed, 0f, Space.World); //마우스x좌표로 드래그할경우 회전한다
                transform.Rotate( 0f, 0f, Input.GetAxis("Mouse Y") * DragSpeed, Space.World); //마우스y좌표로 드래그할경우 회전한다
            }

            Scroll += Input.GetAxis("Mouse ScrollWheel");

            if(Scroll > 1.5f)
            {
                Scroll = 1.5f;
            }
            else if(Scroll < 0.5f)
            {
                Scroll = 0.5f;
            }

        }
        else
        {
            Vector3 velo = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, Save_Position, ref velo, 0.05f); //원래 위치로 되돌린다.
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Save_Rotation, 0.25f); // 원래 각도로 되돌린다.
            Scroll = 1f; // 스크롤변수를 초기화한다.
        }

    }


    void IdentifyObj () //플레이어 레이캐스트와 오브젝트가 충돌했을때 실행되는 함수
    {

        if (is_Identify)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger)
                {
                    SetTrigger = true;
                    if (Player_Controller.is_Identify) //만약 식별중일경우
                    {

                        Player_Controller.is_Identify = false; //식별중을 끈다
                        is_Identify = false; //식별중을 끈다.

                        //transform.localPosition = Save_Position;//원래있던자리에 둔다
                        //transform.localRotation = Save_Rotation;
                        
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
                    if(!Player_Controller.is_Identify) //만약 플레이어가 식별중이 아닐경우
                    {
                        //플레이어 애니메이션 조정
                        if(Player_Controller.Player_State != (int)Player_Controller.State.is_Sit) //앉아있는 중이 아니라면
                        {
                            Player_Controller.Player_State = (int)Player_Controller.State.is_Stop; //서있는 것으로 바꾼다
                        }
                        else
                        {
                            Player_Controller.Player_State = (int)Player_Controller.State.is_Sit; //앉아있는 것으로 바꾼다.
                            Player_Controller.Animator.SetFloat("MoveSpeed", 0); //애니메이션에 있는 MoveSpeed변수에 전후 방향키 입력전달
                            Player_Controller.Animator.SetFloat("HorizontalSpeed", 0); //애니메이션에 있는 HorizontalSpeed변수에 좌우 방향키 입력전달

                        }

                        Player_Controller.is_Identify = true; //식별중으로 바꾼다.
                        is_Identify = true; //식별중으로 바꾼다.
                        Save_Position = transform.localPosition; //물체의 위치값을 저장한다.
                        Save_Rotation = transform.localRotation; //물체의 회전값을 저장한다.

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

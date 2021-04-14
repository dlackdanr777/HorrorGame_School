using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    enum State //플레이어의 상태를 열거
    {
        is_Walk, //0일경우 걷는중
        is_Sit, // 1일경우 앉아있는중
        is_Run, // 2일경우 뛰는중
    }


    //플레이어 능력치
    public float Speed = 6.0f; // 플레이어의 속도
    public float RunSpeed = 1.5f; // 플레이어가 달리는 속도(곱연산)
    public float JumpSpeed = 8.0f; // 플레이어의 점프 속도
    public float gravity = 20.0f; // 플레이어에게 작용하는 중력크기
    private int Player_State = 0; // 플레이어의 현재 상태

    private CharacterController Controller; //플레이어의 캐릭터콘트롤러 콜라이더
    private Vector3 Movedir; // 플레아어가 움직이는 방향

    private void Start()
    {
        Controller = GetComponent<CharacterController>(); // 플레이어가 가지고 있는 캐릭터 콘트롤러 콜라이더를 변수에 할당
        Movedir = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (Controller.isGrounded) // 만약 플레이어가 땅에 있으면?
        {

            Movedir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //움직임 세팅

            Movedir = transform.TransformDirection(Movedir); // 백터를 로컬좌표계에서 월드 좌표계 기준으로 변환


            if (Player_State == (int)State.is_Walk) // 만약 현재 상태가 걷는 중이라면?
            {
                Movedir *= Speed; // 스피드 값만큼 이동속도 증가 
            }

            else if(Player_State == (int)State.is_Run) // 만약 현재 상태가 달리는 중이라면?
            {
                Movedir *= Speed; // 스피드 값만큼 이동속도 증가
                Movedir.z *= RunSpeed; // 앞뒤로 달리는 속도를 런 스피드 값만큼 곱한다. 
            }

            else if(Player_State == (int)State.is_Sit) // 만약 현재 상태가 앉아있는 중이라면?
            {
                Movedir *= (Speed * 0.5f); // 스피드/2 값만큼 이동속도 증가
            }

            if (Input.GetButton("Jump")) // 점프키를 눌렀을 경우?
            {
                Movedir.y = JumpSpeed; //플레이어의 y좌표를 점프스피드값만큼 준다
            }

        }
        Now_State();

        Movedir.y -= gravity * Time.deltaTime; // 캐릭터 중력적용

        Controller.Move(Movedir * Time.deltaTime); // 캐릭터를 이동

       
    }


    void Now_State() // 키입력을 받아 현재상태를 변경하는 함수
    {
        if (Input.GetKeyDown(KeyCode.C)) // 만약 C버튼을 눌렀으면?
        {
            if (Player_State != (int)State.is_Sit) // 만약 플레이어의 상태가 앉아있는중이 아니라면?
                Player_State = (int)State.is_Sit; //플레이어의 상태를 앉아있는 중으로 변경
            else //만약 앉아있는 중이라면?
                Player_State = (int)State.is_Walk; //플레이어의 상태를 걷는 중으로 변경
        }

        else if (Input.GetKey(KeyCode.LeftShift) && Player_State != (int)State.is_Sit) // 만약 왼쪽 쉬프트키를 누르고 있는 중 AND 앉아있는 중이 아니라면?
        {
            Player_State = (int)State.is_Run; // 플레이어의 상태를 달리는 중으로 변경
        }
        
        else if(!Input.GetKey(KeyCode.LeftShift) && Player_State != (int)State.is_Sit) // 만약 왼쪽 쉬프트를 누르지 않고 있거나 앉아있는 중이 아니라면?
        {
            Player_State = (int)State.is_Walk; //플레이어의 상태를 걷는 중으로 변경
        }
    }
}

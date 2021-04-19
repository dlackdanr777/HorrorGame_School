using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    public enum State //플레이어의 상태를 열거
    {
        is_Stop, //0일경우 멈춰있는 중
        is_Sit = 1, // 1일경우 앉아있는중
        is_Walk = 10, //10일경우 걷는중
        is_Run = 11, // 11일경우 뛰는중

    }

    [Header ("플레이어 속성")]
    public float Speed = 6.0f; // 플레이어의 속도
    public float RunSpeed = 1.5f; // 플레이어가 달리는 속도(곱연산)
    public float JumpSpeed = 8.0f; // 플레이어의 점프 속도
    public float gravity = 20.0f; // 플레이어에게 작용하는 중력크기
    public int Player_State = 0; // 플레이어의 현재 상태
    public Flashlight_PRO Flash; // 플레이어가 사용하는 플래쉬


    [Header ("카메라 속성")]
    public Camera MainCamera; //플레이어 1인칭 카메라 변수
    public float LookSensitivity; //카메라 회전 속도
    public float CameraRotationLimit = 60; // 카메라의 상하 각도를 제한하는 값
    private float currentCameraRotationX = 5; //카메라 x축의 회전값

    [Header ("레이캐스트 속성")]
    private RaycastHit Hit;
    private GameObject HitObj; //충돌할 오브젝트를 저장할 변수
    public float MaxDistance = 5f; //레이캐스트의 거리
    public Text RayCastText; //레이캐스트가 충돌했을때 나타날 텍스트


    public GameObject Flashlight;


    private CharacterController Controller; //플레이어의 캐릭터콘트롤러 콜라이더
    private Animator Animator; //플레이어의 애니메이터
    private Vector3 Movedir; // 플레아어가 움직이는 방향

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private float CoolTime;
    private float SetCooltime = 0.6f; // 쿨타임을 0.6초로 설정
    /////////////////////////////////
    private void Start()
    {
        Controller = GetComponent<CharacterController>(); // 플레이어가 가지고 있는 캐릭터 콘트롤러 콜라이더를 변수에 할당
        Animator = GetComponent<Animator>();
        Movedir = Vector3.zero;
    }

    private void FixedUpdate()
    {
        PlayerMove(); // 플레이어의 전후좌우 움직임 함수
        PlayerRotation(); // 플레이어 좌우 움직임 함수
        CameraRotation(); //1인칭 카메라 상하 움직임 함수

        RayCastFunction();
        PlayerAnimation();

        CoolTimeSet();

    }



    private void PlayerMove() //플레이어의 상하좌우 움직임의 함수
    {
        if (Controller.isGrounded) // 만약 플레이어가 땅에 있으면?
        {

            Movedir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //움직임 세팅

            Movedir = transform.TransformDirection(Movedir); // 백터를 로컬좌표계에서 월드 좌표계 기준으로 변환


            if (Player_State == (int)State.is_Walk || Player_State == (int)State.is_Stop) // 만약 현재 상태가 걷거나 멈춤이라면?
            {
                Movedir *= Speed; // 스피드 값만큼 이동속도 증가 
            }

            else if (Player_State == (int)State.is_Run) // 만약 현재 상태가 달리는 중이라면?
            {
                Movedir *= Speed * RunSpeed; // 스피드 값 * 달리기 계수만큼 이동속도 증가
            }

            else if (Player_State == (int)State.is_Sit) // 만약 현재 상태가 앉아있는 중이라면?
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


    private void CameraRotation() // 카메라의 상하움직임을 구현한 함수
    {
        float _XRotation = Input.GetAxisRaw("Mouse Y"); // 마우스 움직임 값을 변수에 넣는다. -1~1
        float _CameraRotationX = _XRotation * LookSensitivity;

        currentCameraRotationX -= _CameraRotationX; //위에서 구한 마우스 움직임 각도를 빼준다.(더하면 카메라가 반전됨)
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CameraRotationLimit, CameraRotationLimit); //카메라 상하의 움직임을 일정 각도이상으로 안꺽이게 제한

        MainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //위에서 구한 값으로 카메라 각도를 변경
        if(Player_State == (int)State.is_Sit)
        {
            Flashlight.transform.localEulerAngles = new Vector3(30f, currentCameraRotationX - 60, currentCameraRotationX * 0.2f -40f); //위에서 구한 값으로 플래쉬의 각도도 변경하지만 앉아있을 경우 손전등이 아래를 향하기에 그 각도만큼 보정해준다.
        }
        else 
            Flashlight.transform.localEulerAngles = new Vector3(0f, currentCameraRotationX + 10f, currentCameraRotationX * 0.3f - 10f); //위에서 구한 값으로 플래쉬의 각도도 변경한다
    }


    void PlayerRotation() //플레이어의 좌우회전을 구현한 함수
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");// 마우스 좌우값을 받아 변수에 저장 (-1 ~ 1);
        Vector3 _PlayerRotationY = new Vector3(0f, _yRotation, 0f) * LookSensitivity; //좌우 값에 센서민감도를 곱한다
        transform.Rotate(_PlayerRotationY); // 위에서 구한 회전 각만큼 회전
    }


    void Now_State() // 키입력을 받아 현재상태를 변경하는 함수
    {
        if (Input.GetAxis("Vertical") == 0 && Player_State != 1) //만약 움직이지 않을 경우나 앉아있지 않을 경우?
        {
            Player_State = (int)State.is_Stop;
        }

        if (Input.GetKey(KeyCode.C)) // 만약 C버튼을 눌렀으면?
        {
            if (!SetTrigger) // 쿨타임 설정
            {
                SetTrigger = true;
                if (Player_State != (int)State.is_Sit) // 만약 플레이어의 상태가 앉아있는중이 아니라면?
                    Player_State = (int)State.is_Sit; //플레이어의 상태를 앉아있는 중으로 변경
                else //만약 앉아있는 중이라면?
                    Player_State = (int)State.is_Stop; //플레이어의 상태를 멈춤으로 변경
            }
            

            Debug.Log("1");
        }

        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0.5f && Player_State != (int)State.is_Sit) // 만약 왼쪽 쉬프트키를 누르고 있는 중 AND 앉아있는 중이 아니라면?
        {
            Player_State = (int)State.is_Run; // 플레이어의 상태를 달리는 중으로 변경
        }
        
        else if(!Input.GetKey(KeyCode.LeftShift) && Player_State != (int)State.is_Sit && (Input.GetAxis("Vertical") > 0.5f || Input.GetAxis("Vertical") < -0.5f )) // 만약 왼쪽 쉬프트를 누르지 않고 있거나 앉아있는 중이 아니라면?
        {
            Player_State = (int)State.is_Walk; //플레이어의 상태를 걷는 중으로 변경
        }

        if (Input.GetKey(KeyCode.F))
        {
            if (!SetTrigger) // 쿨타임 설정
            {
                SetTrigger = true;
                Flash.Switch(); //F키로 플래시 켯다 끄기기능
            }
        }

        if(Player_State == (int)State.is_Sit) // 만약 앉아있을 경우
        {
            Controller.height = 1.3f; // 캐릭터의 높이를 1.3로 낮춤
            Controller.center = new Vector3(0f, 0.7f, 0.1f); //콜라이더의 중심점을 0.7로 낮춤
            Controller.radius = 0.6f;
        }
        else
        {
            Controller.height = 1.7f; // 캐릭터의 높이를 1.7로 올림
            Controller.center = new Vector3(0f, 0.9f, 0f); //콜라이더의 중심점을 0.9로 올림
            Controller.radius = 0.5f;
        }


    }

    void PlayerAnimation()
    {
        Animator.SetFloat("MoveSpeed", Input.GetAxis("Vertical")); //애니메이션에 있는 MoveSpeed변수에 전후 방향키 입력전달
        Animator.SetFloat("HorizontalSpeed", Input.GetAxis("Horizontal")); //애니메이션에 있는 HorizontalSpeed변수에 좌우 방향키 입력전달
        Animator.SetInteger("PlayerState", Player_State); // 현재상태 변수 전달
        
    }


    void RayCastFunction() // 레이캐스트 관련 함수
    {
        
        Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * MaxDistance, Color.blue, 0.3f);
        if(Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out Hit, MaxDistance)) // 앞에 레이케스트를 쏜다.
        {
            if (Hit.transform.tag == "Door") // 현재 보고있는 것이 문일경우?
            {
                Debug.Log("문임");
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<DoorController>().PossibleState = true; // 사용가능한 문일경우 컨트롤가능한 상태로 변경한다.
                if (Hit.transform.GetComponent<DoorController>().is_open)
                {
                    RayCastText.text = "문닫기(E)";
                }
                else
                    RayCastText.text = "문열기(E)";
            }
            else if(Hit.transform.tag == "Window") // 현재 보고있는 것이 창문일경우?
            {
                Debug.Log("창문임");
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<WindowController>().PossibleState = true; // 사용가능한 창문일경우 컨트롤가능한 상태로 변경한다.
                if (Hit.transform.GetComponent<WindowController>().is_open)
                {
                    RayCastText.text = "창문닫기(E)";
                }
                else
                    RayCastText.text = "창문열기(E)";
            }

        }
        if(Hit.transform == null || Hit.transform != HitObj.transform) //충돌한 물체가 없거나 충돌물체가 바뀔경우?
        {
            if(HitObj != null) // 만약 충돌했던 물체가 있을경우?
            {
                if (HitObj.tag == "Door") //충돌했던 물체가 문일경우?
                {
                    HitObj.transform.GetComponent<DoorController>().PossibleState = false; //문의 컨트롤을 끈다.
                }
                else if(HitObj.tag == "Window")
                {
                    HitObj.transform.GetComponent<WindowController>().PossibleState = false; //문의 컨트롤을 끈다.
                }

                HitObj = null;
                RayCastText.text = " ";
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
}

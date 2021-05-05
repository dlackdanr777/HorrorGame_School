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
    public GameObject Flashlight;
    public Flashlight_PRO Flash; // 플레이어가 사용하는 플래쉬

    //체력관련 변수
    [HideInInspector]
    public bool is_Resting = false; //체력이 회복되는 상황인가를 받는 변수
    [HideInInspector]
    public bool is_Under_Attack = false; //공격받는 상황인가를 받는 변수

    //아이템 관련 변수
    [SerializeField]
    private Text NameText; //이름을 보여줄 텍스트
    [SerializeField]
    private Text ExplanationText; //설명을 보여줄 텍스트


    [Header ("카메라 속성")]
    public Camera MainCamera; //플레이어 1인칭 카메라 변수
    public float LookSensitivity; //카메라 회전 속도
    public float CameraRotationLimit = 60; // 카메라의 상하 각도를 제한하는 값
    private float currentCameraRotationX = 5; //카메라 x축의 회전값

    [Header ("레이캐스트 속성")]
    private RaycastHit Hit;
    private GameObject HitObj; //충돌할 오브젝트를 저장할 변수
    public float MaxDistance = 2f; //레이캐스트의 거리
    public Text RayCastText; //레이캐스트가 충돌했을때 나타날 텍스트



    private CharacterController Controller; //플레이어의 캐릭터콘트롤러 콜라이더
    [HideInInspector]
    public Animator Animator; //플레이어의 애니메이터
    private Vector3 Movedir; // 플레아어가 움직이는 방향
    [HideInInspector]
    public bool is_Identify; //오브젝트를 식별중인가 아닌가를 입력받는 변수
    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private float CoolTime;
    private float SetCooltime = 0.6f; // 쿨타임을 0.6초로 설정

    private int layerMask; //레이어 마스크를 지정한다.(레이케스트)
    /////////////////////////////////

    //카메라 무빙함수를 위한 변수들
    private float y; //카메라 위치를 조정할때 쓰이는 변수
    private float Time_I;

    private void Start()
    {
        Controller = GetComponent<CharacterController>(); // 플레이어가 가지고 있는 캐릭터 콘트롤러 콜라이더를 변수에 할당
        Animator = GetComponent<Animator>();
        Movedir = Vector3.zero;
        layerMask = 1 << LayerMask.NameToLayer("Interaction"); //상호작용 레이어를 지정한다.
        y = MainCamera.transform.position.y;
    }

    private void FixedUpdate()
    {
        if (!is_Identify) //플레이어가 식별중이 아닐경우
        {
            PlayerMove(); // 플레이어의 전후좌우 움직임 함수
            PlayerRotation(); // 플레이어 좌우 움직임 함수
            CameraRotation(); //1인칭 카메라 상하 움직임 함수
            PlayerAnimation();
        }

        FPSCamera();
        RayCastFunction();
        CoolTimeSet();
        Animator.SetInteger("PlayerState", Player_State); // 애니메이터에 현재상태 변수 전달

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

        StartCoroutine("Flash_Move");
    }

    IEnumerator Flash_Move()
    {
        yield return new WaitForSeconds(0.3f);
        Flashlight.transform.localRotation = Quaternion.Slerp(Flashlight.transform.localRotation, Quaternion.Euler(0f, -90f, -currentCameraRotationX), 20f * Time.deltaTime);
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
                {
                    Player_State = (int)State.is_Sit; //플레이어의 상태를 앉아있는 중으로 변경
                }

                else //만약 앉아있는 중이라면?
                {
                    Player_State = (int)State.is_Stop; //플레이어의 상태를 멈춤으로 변경
                }
                   
            }

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
            Controller.height = 1f; // 캐릭터의 높이를 1.3로 낮춤
            Controller.center = new Vector3(0f, 0.5f, 0.3f); //콜라이더의 중심점을 0.7로 낮춤
            Controller.radius = 0.35f;


        }
        else
        {
            Controller.height = 1.6f; // 캐릭터의 높이를 1.6로 올림
            Controller.center = new Vector3(0f, 0.85f, 0.2f); //콜라이더의 중심점을 0.85로 올림
            Controller.radius = 0.3f;
    
        }

    }

    void PlayerAnimation()
    {
        Animator.SetFloat("MoveSpeed", Input.GetAxis("Vertical")); //애니메이션에 있는 MoveSpeed변수에 전후 방향키 입력전달
        Animator.SetFloat("HorizontalSpeed", Input.GetAxis("Horizontal")); //애니메이션에 있는 HorizontalSpeed변수에 좌우 방향키 입력전달

    }




    void FPSCamera() //1인칭 카메라 위치를 계산하는 함수
    {
        if (Player_State == (int)State.is_Sit)
        {

            if(1 < (MainCamera.transform.position.y - transform.position.y))
            {
                MainCamera.transform.position = new Vector3(this.transform.position.x, MainCamera.transform.position.y - Time.deltaTime, this.transform.position.z);
            }

        }
        else
        {
            if(1.5f > (MainCamera.transform.position.y - transform.position.y))
            {
                MainCamera.transform.position = new Vector3(this.transform.position.x, MainCamera.transform.position.y + Time.deltaTime, this.transform.position.z);
            }

        }
    }


    void RayCastFunction() // 레이캐스트 관련 함수
    {
        Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * MaxDistance, Color.blue, 0.3f);
        if(Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out Hit, MaxDistance,layerMask)) // 앞에 레이케스트를 쏜다.
        {
            if (Hit.transform.tag == "Door") // 현재 보고있는 것이 문일경우?
            {
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                if(Hit.transform.GetComponent<DoorController>() != null) //만약 도어컨트롤러가 있는 오브젝트일경우
                {
                    Hit.transform.GetComponent<DoorController>().PossibleState = true; // 사용가능한 문일경우 컨트롤가능한 상태로 변경한다.
                    if (Hit.transform.GetComponent<DoorController>().is_Lock)
                    {
                        if(GameManager.instance.OwnKey == (int)Hit.transform.GetComponent<DoorController>().KeyStage) //만약 현재 보유중인 키가 문에 맞는 열쇠이거나 그 이상의 열쇠를 가지고 있으면?
                        {
                            RayCastText.text = "잠금해제(E)";
                        }
                        else
                        {
                            RayCastText.text = "잠김(E)";
                        }
                        
                    }
                    else if (Hit.transform.GetComponent<DoorController>().is_open)
                    {
                        RayCastText.text = "닫기(E)";
                    }
                    else
                        RayCastText.text = "열기(E)";
                }
                else if(Hit.transform.GetComponent<LockerController>() != null) // 만약 로커 오브젝트일경우
                {
                    Hit.transform.GetComponent<LockerController>().PossibleState = true; // 사용가능한 로커일경우 컨트롤가능한 상태로 변경한다.
                    if (Hit.transform.GetComponent<LockerController>().is_Lock)
                    {
                        RayCastText.text = "잠김(E)";
                    }
                    else if (Hit.transform.GetComponent<LockerController>().is_open)
                    {
                        RayCastText.text = "닫기(E)";
                    }
                    else
                        RayCastText.text = "열기(E)";
                }

                
                
            }


            else if(Hit.transform.tag == "Window") // 현재 보고있는 것이 창문일경우?
            {
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<WindowController>().PossibleState = true; // 사용가능한 창문일경우 컨트롤가능한 상태로 변경한다.
                if (Hit.transform.GetComponent<WindowController>().is_open || Hit.transform.GetComponent<WindowController>().Window2.GetComponent<WindowController>().is_open)
                {
                    RayCastText.text = "닫기(E)";
                }
                else
                    RayCastText.text = "열기(E)";
            }

            else if(Hit.transform.tag == "LightButton") //현재 보고있는 것이 조명 버튼일 경우
            {
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<LightController>().PossibleState = true; // 사용가능한 창문일경우 컨트롤가능한 상태로 변경한다.
                if (Hit.transform.GetComponent<LightController>().is_TurnOn)
                {
                    RayCastText.text = "조명끄기(E)";
                }
                else
                {
                    RayCastText.text = "조명켜기(E)";
                }
            }


            else if (Hit.transform.tag == "ImportantObj") //현재 보고 있는 것이 중요한 오브젝트 일경우
            {
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<ObjectController>().PossibleState = true;
                if (!is_Identify)
                {
                    RayCastText.text = "보기(E)";
                    NameText.text = null;
                    ExplanationText.text = null;
                }
                else if(is_Identify)
                {
                    if (Hit.transform.GetComponent<ObjectController>().Obj_State != Obj_State.Obj) //만약 일반 오브젝트가 아니였을 경우
                    {
                        RayCastText.text = "획득(R) \n\n내려놓기(E)";

                    }
                    else
                    {
                        RayCastText.text = "내려놓기(E)";
                    }
                        
                    NameText.text = Hit.transform.GetComponent<ObjectController>().Name;
                    ExplanationText.text = Hit.transform.GetComponent<ObjectController>().Explanation;
                }
                
            }

            else if(Hit.transform.tag == "Piano") //현재 보고 있는 것이 피아노 일경우
            {
                Debug.Log("실행");
                HitObj = Hit.transform.gameObject; // 충돌한 물체의 정보를 저장함
                Hit.transform.GetComponent<BeethovenController>().PossibleState = true;
                if (Hit.transform.GetComponent<BeethovenController>().fncStart)
                {
                    RayCastText.text = "피아노 닫기(E)";
                }
                else
                {
                    RayCastText.text = " ";
                }
            }


            else //만약 아무것도 해당되지 않는다면?
            {
                HitObj = null;
                RayCastText.text = " ";
                NameText.text = null;
                ExplanationText.text = null;
            }

            
        }


        if(Hit.transform == null) //충돌한 물체가 없을 경우?
        {
            if(HitObj != null) // 만약 충돌했던 물체가 있을경우?
            {
                if (HitObj.tag == "Door") //충돌했던 물체가 문일경우?
                {
                    if(HitObj.transform.GetComponent<DoorController>() != null)
                    {
                        HitObj.transform.GetComponent<DoorController>().PossibleState = false; //문의 컨트롤을 끈다.
                    }
                    else if(HitObj.transform.GetComponent<LockerController>() != null)
                    {
                        HitObj.transform.GetComponent<LockerController>().PossibleState = false; //문의 컨트롤을 끈다.
                    }
                    

                }
                else if(HitObj.tag == "Window")
                {
                    HitObj.transform.GetComponent<WindowController>().PossibleState = false; //문의 컨트롤을 끈다.
                }
                else if(HitObj.tag == "LightButton")
                {
                    HitObj.transform.GetComponent<LightController>().PossibleState = false; //문의 컨트롤을 끈다.
                }
                else if(HitObj.tag == "ImportantObj")
                {
                    HitObj.transform.GetComponent<ObjectController>().PossibleState = false;
                }

            }
            HitObj = null;
            RayCastText.text = " ";
            NameText.text = null;
            ExplanationText.text = null;
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

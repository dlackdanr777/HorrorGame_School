using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public enum m_State
{
    is_Stop,
    is_Walk,
    is_Boundary,
    Tracking,
}
public class Monster_Controller : MonoBehaviour
{
    public m_State m_State; //몬스터의 상태를 받는 변수


    private GameObject Player; //플레이어의 오브젝트를 받아오는 변수
    private Player_Controller Player_Controller;
    private NavMeshAgent Nav; //해당 오브젝트의 네비메쉬를 받아오는 변수
    private BoxCollider Box; //자신의 박스콜라이더를 받을 변수
    private Vector3 Player_location; //플레이어의 위치를 받는 변수
    private Vector3 Set_location; //자신의 위치를 저장할 변수
    private Animator Ani;


    //시야 관련 변수
    private float m_angle = 80f; //시야각
    private float m_distance = 5f; //시야길이

    //충돌관련 변수
    private bool Did_Collide = false; //플레이어와 충돌했나 안했나를 받는 변수
    private bool Out_Collide = false; //충돌범위에서 벗어낫나 아닌가를 받는 변수
    private bool Player_Detection = false; //플레이어가 시야 범위 안에 있으면 참을 받는 변수
    private bool is_Tracking = false; //추적중일때 참을 받는 변수
    private bool TrackingFailed = false; //추적에 실패했을때 참을 받는 변수

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private bool ResetTrigger2 = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    private float ResetCoolTime2 = 0;
    private float time = 0;
    /// ///////////////////////////////////

    RaycastHit Hit;
    private void Start()
    {
        Player = GameObject.Find("Player");
        Player_Controller = Player.GetComponent<Player_Controller>();

        Nav = GetComponent<NavMeshAgent>();
        Box = GetComponent<BoxCollider>();

        Ani = GetComponent<Animator>();
        Set_location = transform.position;
    }

    private void FixedUpdate()
    {
        
        TrackingPlayer();
        Sight();
        HearAsound();
        DataReset();

        Ani.SetFloat("Speed", Mathf.Abs(Nav.velocity.x + Nav.velocity.z));
        Ani.SetInteger("State", (int)m_State);

        if(m_State == m_State.Tracking)
        {
            Nav.speed = 4f;
        }
        else if (m_State <= m_State.is_Boundary)
        {
            Nav.speed = 1f;
        }

        NavMeshPath path = new NavMeshPath();
        if (Mathf.Abs(Nav.velocity.x + Nav.velocity.z) < 0.1f) //몬스터가 움직이지 않을경우
        {
            time += Time.deltaTime;
            if(time > 5f)
            {
                Debug.Log("못감");
                m_State = m_State.is_Walk;
                Did_Collide = false;
                Player_Detection = false;
                Nav.destination = Set_location;
                time = 0;
            }

        }

    }

    void TrackingPlayer() //플레이어 추적관련 함수
    {
        if (!Out_Collide) //충돌범위에서 안벗어났으면?
        {
            Player_location = Player.transform.position; //플레이어의 현재 위치를 저장한다.
        }



        if (Player_Detection) //몬스터 시야에 들어오면?
        {
            is_Tracking = true;
            m_State = m_State.Tracking; //상태를 추적중으로 바꾼다.
            Nav.destination = Player.transform.position; //플레이어위치를 갱신시킨다.
            Nav.Resume(); //네비메쉬를 킨다
        }

        else if (Did_Collide) //플레이어에 충돌중이면
        {
            Nav.Resume(); //네비메쉬를 킨다
            if (!Out_Collide && !is_Tracking) //만약 충돌범위 안이거나 시야에서 벗어나있는 상태면?
            {
                Nav.destination = Player_location; //플레이어위치를 갱신시킨다.
                m_State = m_State.is_Boundary; //상태를 경계중으로 바꾼다
            }
            else if (!Out_Collide && is_Tracking)
            {
                Nav.destination = Player_location; //플레이어위치를 갱신시킨다.
                m_State = m_State.Tracking; //상태를 추적중으로 유지한다
                Nav.Resume(); //네비메쉬를 킨다
            }
        }
        else
        {
            Nav.destination = Set_location;
            m_State = m_State.is_Walk;
            Did_Collide = false;
            Player_Detection = false;
        }

    }


    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }


    void HearAsound()
    {
        //만약 플레이어가 앉아있거나 멈춰있으면
        if (Player_Controller.Player_State == (int)Player_Controller.State.is_Stop || Player_Controller.Player_State == (int)Player_Controller.State.is_Sit)
        {
            Box.size = new Vector3(2, 2.5f, 2); //충돌 범위를 줄인다.

        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Walk) //플레이어가 걷는 중이라면
        {
            Box.size = new Vector3(15, 2.5f, 15); //충돌 범위를 조정한다
        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Run) //플레이어가 뛰는 중이라면
        {

            Box.size = new Vector3(30, 8f, 30); //충돌 범위를 조정한다
        }
    }

    void Sight() //몬스터의 시야 관련 함수
    {
        Vector3 _leftBoundary = BoundaryAngle(-m_angle * 0.5f); //왼쪽 눈의 시야각을 설정
        Vector3 _rightBoundary = BoundaryAngle(m_angle * 0.5f);//오른쪽 눈의 시야각을 설정

        Debug.DrawRay(transform.position + (transform.up * 1f) + transform.forward, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + (transform.up * 1f) + transform.forward, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, m_distance); //근처에 있는 오브젝트들의 콜라이더를 받아온다

        if (Player_Detection) //만약 시야범위 내라면?
        {
            Did_Collide = true; //충돌 변수도 참으로 만든다.
        }


        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.tag == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < m_angle * 0.5f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + (transform.up * 1f) + transform.forward, _direction, out hit, m_distance)) //플레이어의 위치로 레이를 쏜다
                    {
                        if (hit.transform.tag == "Player") //플레이어가 닿으면?
                        {
                            Player_Detection = true; //시야포착을 참으로 만든다.
                            Debug.DrawRay(transform.position + transform.up * 1f + transform.forward, _direction, Color.blue);
                        }
                    }
                    else //시야에 닿지않으면?
                    {
                        Player_Detection = false;
                    }
                }
                else //시야각에서 벗어나있으면?
                {
                    Player_Detection = false;
                }
            }
        }
    }


    void DataReset() //컨트롤을 초기화하는 함수
    {
        if (!Out_Collide)
        {
            ResetCoolTime = 0;
            ResetCoolTime2 = 0;
        }

  
        if (Did_Collide && !ResetTrigger) // 플레이어에 충돌중이고 리셋트리거가 작동되지않았을때
        {
            ResetTrigger = true; //리셋트리거를 작동시킨다.

        }

        else if (ResetTrigger) //리셋트리거가 작동 했을 때
        {


            ResetCoolTime += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime > 20f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                Did_Collide = false; //충돌 상태를 끈다.
            }
        }

        if(Player_Detection && !ResetTrigger2) //플레이어가 시야안에있고 리셋트리거가 작동되지 않았을때
        {

            ResetTrigger2 = true; //리셋트리거를 작동시킨다.
        }

        else if (ResetTrigger2) //리셋트리거가 작동 했을 때
        {

            ResetCoolTime2 += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime2 > 20f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger2 = false; //리셋트리거를 끈다
                ResetCoolTime2 = 0f; //시간을 초기화
                is_Tracking = false; //추적중을 끈다
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") //트리거에 플레이어가  충돌중이라면
        {
            Did_Collide = true; //충돌변수에 참을 준다.
            Out_Collide = false; //충돌범위안이면 거짓을 준다
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") //트리거에서 플레이어가 벗어났다면
        {
            Out_Collide = true; //충돌범위에서 벗어나면 이 변수에 참을준다.
        }
    }

}

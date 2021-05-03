using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    cant_move, //이동할 수 없는 물체에 준다.
    can_move,   //이동할 수 있는 물체에 준다.
}

public class StatueGhostController : MonoBehaviour
{
    public State State; //움직일수있는 물체를 판별하는 변수

    private bool can_turn = true; //회전할 수 있나 없나를 받는 변수
    private bool Did_Collide = false; //플레이어와 충돌했나 안했나를 받는 변수
    private bool Out_Collide = false; //충돌범위에서 벗어낫나 아닌가를 받는 변수
    private bool Player_Detection = false; //플레이어가 시야 범위 안에 있으면 참을 받는 변수
    private bool is_Start = false; //언제 움직일 것인가를 받는 변수
    private GameObject Player; //플레이어의 오브젝트를 받아오는 변수
    private Player_Controller Player_Controller;
    private NavMeshAgent Nav; //해당 오브젝트의 네비메쉬를 받아오는 변수
    private BoxCollider Box; //자신의 박스콜라이더를 받을 변수
    [HideInInspector]
    public bool PossibleState = false; //현재 컨트롤 가능한 상태인지 입력받는 변수
    private Vector3 SetPosition;//오브젝트의 원래 위치를 받는다.

    //시야 관련 변수
    private float m_angle = 120f; //시야각
    private float m_distance = 5f; //시야길이

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////


    private void Start()
    {
        Player = GameObject.Find("Player"); //플레이어를 찾아 넣는다.
        Player_Controller = Player.GetComponent<Player_Controller>();
        Box = GetComponent<BoxCollider>();
        if (State == State.can_move)
        {
            SetPosition = transform.position;
            Nav = GetComponent<NavMeshAgent>();
            Nav.destination = transform.position;
            Nav.Stop();

        }
        is_Start = true;
    }

    private void FixedUpdate()
    {
        if (is_Start) //시작변수가 참이면
        {
            if (Did_Collide) //플레이어에 충돌중이고  
            {
                if (can_turn) //카메라범위에서 벗어났으면?
                {
                    transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z)); //플레이어를 바라보게 회전한다.

                    if (State == State.can_move) //만약 움직일수 있는 물체면
                    {
                        Nav.Resume(); //네비메쉬를 킨다
                        if (!Out_Collide || Player_Detection) //만약 충돌범위 안이거나 시야범위 안이면?
                        {
                            Nav.destination = Player.transform.position; //플레이어위치를 갱신시킨다.
                        }

                    }

                }
                else //카메라에 있을 경우
                {
                    if (State == State.can_move) //움직일수 있는 상태일때
                    {
                        Nav.Stop(); //네비를 끈다
                    }

                }


            }

            else // 플레이어가 충돌범위에서 벗어났을때
            {
                if (can_turn) //카메라에서 벗어났을경우
                {
                    if (State == State.can_move) //움직일수 있는 물체이면
                    {
                        Nav.destination = SetPosition; //원래 위치로 돌아간다.
                        Nav.Resume();
                    }

                }
                else // 카메라안에 있을 경우
                {
                    if (State == State.can_move) // 움직일 수 있는 경우라면
                    {
                        Nav.Stop(); //멈춘다
                    }
                }

            }
        }
        else //시작변수가 거짓이면
        {

        }

        //만약 플레이어가 앉아있거나 멈춰있으면
        if (Player_Controller.Player_State == (int)Player_Controller.State.is_Stop || Player_Controller.Player_State == (int)Player_Controller.State.is_Sit)
        {
            Box.size = new Vector3(4, 2.5f, 4); //충돌 범위를 줄인다.

        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Walk) //플레이어가 걷는 중이라면
        {
            Box.size = new Vector3(8, 2.5f, 8); //충돌 범위를 조정한다
        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Run) //플레이어가 뛰는 중이라면
        {

            Box.size = new Vector3(15, 8f, 15); //충돌 범위를 조정한다
        }

        DataReset();
        Sight();
    }



    private void OnBecameInvisible()
    {
        can_turn = true;
    }

    private void OnBecameVisible()
    {
        can_turn = false;
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
    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }


    void Sight() //몬스터의 시야 관련 함수
    {
        Vector3 _leftBoundary = BoundaryAngle(-m_angle * 0.5f); //왼쪽 눈의 시야각을 설정
        Vector3 _rightBoundary = BoundaryAngle(m_angle * 0.5f);//오른쪽 눈의 시야각을 설정

        Debug.DrawRay(transform.position + (transform.up * 1.5f), _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + (transform.up * 1.5f), _rightBoundary, Color.red);

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
                    if (Physics.Raycast(transform.position + (transform.up * 1.5f), _direction, out hit, m_distance))
                    {
                        if (hit.transform.tag == "Player") //플레이어가 닿으면?
                        {
                            Player_Detection = true; //시야포착을 참으로 만든다.
                            Debug.DrawRay(transform.position + transform.up * 1.5f, _direction, Color.blue);
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
        if (Did_Collide && !ResetTrigger) // 플레이어에 충돌중이고 리셋트리거가 작동되지않았을때
        {
            ResetTrigger = true; //리셋트리거를 작동시킨다.

        }

        else if (ResetTrigger) //리셋트리거가 작동 했을 때
        {

            ResetCoolTime += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime > 5f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                Did_Collide = false; //충돌 상태를 끈다.
            }
        }
    }
}


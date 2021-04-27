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
    private float m_angle = 30f; //시야각
    private float m_distance = 5f; //시야길이
    private LayerMask m_layerMask = 0;

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////


    RaycastHit Hit;
    private void Start()
    {
        Player = GameObject.Find("Player"); //플레이어를 찾아 넣는다.
        Player_Controller = Player.GetComponent<Player_Controller>();
        Box = GetComponent<BoxCollider>();
        if(State == State.can_move)
        {
            SetPosition = transform.position;
            Nav = GetComponent<NavMeshAgent>();
            Nav.destination = transform.position;
            Nav.Stop();
        
        }
        is_Start = true;
        m_layerMask = 1 << LayerMask.NameToLayer("Player"); //플레이어 레이어를 지정한다
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

                    if (State == State.can_move) //만약 움직일수 있는 물체거나 충돌범위 안이면?
                    {
                        Nav.Resume();
                        if(!Out_Collide)
                        {
                            Nav.destination = Player.transform.position; //플레이어를 추적한다.
                            Debug.Log("추적중");
                        }

                    }

                }
                else //카메라에 있을 경우
                {
                    Nav.Stop();
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
            Box.size = new Vector3(5, 2.5f, 5); //충돌 범위를 줄인다.

        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Walk) //플레이어가 걷는 중이라면
        {
            Box.size = new Vector3(10, 2.5f, 10); //충돌 범위를 조정한다
        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Run) //플레이어가 뛰는 중이라면
        {

            Box.size = new Vector3(20, 8f, 20); //충돌 범위를 조정한다
        }

        DataReset();
        Sight();
        Debug.Log(Player_Detection);
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
        if(other.gameObject.tag == "Player") //트리거에 플레이어가  충돌중이라면
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


    void Sight() //몬스터의 시야 관련 함수
    {

        Vector3 t_direction = (Player.transform.position - transform.position).normalized; //거리를 구한다.
        float t_angle = Vector3.Angle(t_direction, transform.forward); //플레이어와 몬스터의 각도차를 구한다

        if (Physics.BoxCast(transform.position + new Vector3(0, 1.5f, 0), transform.localScale, transform.forward, out Hit, transform.rotation, m_distance)) //레이를 쏜다
        {
            if (Hit.transform.tag == "Player")
            {
                Player_Detection = true;
            }
            else if((Hit.transform.tag != "Player"))
            {
                Player_Detection = false;
                Debug.Log("1");
            }
            else
            {
                Player_Detection = false;
                Debug.Log("2");
            }
        }
        else
        {
            Player_Detection = false;
            Debug.Log("3");
        }
        
        

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(0, 1.5f, 0), transform.forward * 10);
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 1.5f, 0) + transform.forward * 10, transform.localScale);
    }

    void DataReset() //컨트롤을 초기화하는 함수
    {
        if (Did_Collide && !ResetTrigger) // 플레이어에 충돌중이고 리셋트리거가 작동되지않았을때
        {
            ResetTrigger = true; //리셋트리거를 작동시킨다.
            Debug.Log("작동");

        }

        else if (ResetTrigger) //리셋트리거가 작동 했을 때
        {

            ResetCoolTime += Time.deltaTime; //숫자를 센다
            if (ResetCoolTime > 5f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                Did_Collide = false; //충돌 상태를 끈다.
                Debug.Log("꺼짐");
            }
        }
    }

}

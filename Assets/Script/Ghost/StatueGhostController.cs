using System.Collections;
using System.Collections.Generic;
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
    private bool Did_Collide = false;
    private GameObject Player; //플레이어의 오브젝트를 받아오는 변수
    private NavMeshAgent Nav; //해당 오브젝트의 네비메쉬를 받아오는 변수

    [HideInInspector]
    public bool PossibleState = false; //현재 컨트롤 가능한 상태인지 입력받는 변수

    private Vector3 SetPosition;//오브젝트의 원래 위치를 받는다.
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
        if(State == State.can_move)
        {
            SetPosition = transform.position;
            Nav = GetComponent<NavMeshAgent>();
            Nav.destination = transform.position;
            Nav.Stop();
        }

    }

    private void FixedUpdate()
    {
        Debug.Log(can_turn);
        if (Did_Collide) //플레이어에 충돌중이고  
        {
            if (can_turn) //카메라범위에서 벗어났으면?
            {
                transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z)); //플레이어를 바라보게 회전한다.

                if (State == State.can_move) //만약 움직일수 있는 물체면
                {
                    Debug.Log("추적");
                    Nav.destination = Player.transform.position; //플레이어를 추적한다.
                    Nav.Resume();
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



        DataReset();
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
            Debug.Log("충돌중");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") //트리거에서 플레이어가 벗어났다면
        {
            Did_Collide = false; //충돌변수에 거짓을 준다.
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
            if (ResetCoolTime > 1f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                Did_Collide = false; //충돌 상태를 끈다.
            }
        }
    }

}

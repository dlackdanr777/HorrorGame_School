using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [HideInInspector]
    public bool is_open = false; //문이 열렸는지 아닌지를 입력받는 변수
    [HideInInspector]
    public bool PossibleState = false; //현재 문이 컨트롤 가능한 상태인지 입력받는 변수

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private float CoolTime;
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////

    float x;
    private void Start()
    {
    }

    private void FixedUpdate()
    {
        DoorControll();
        OpenTheDoor();


        CoolTimeSet();
    }

    void OpenTheDoor()
    {

        if (is_open) // 만약 문이 열렸다면?
        {
            if (x < 0.1f)
            {
                x += Time.deltaTime * 0.5f;
            }
            else if (x < 0.3f)
            {
                x += Time.deltaTime;
            }
            else if (x < 0.9f)
            {
                x += Time.deltaTime * 1.5f;
            }
            else if (x > 0.9f)
            {
                x = 0.9f;
            }
            // 문이 열릴수록 속도가 증가하다 0.9를 넘으면 0.9로 고정
        }
        else if (!is_open) // 만약 문이 닫혔다면?
        {
            if (x > 0.8f)
            {
                x -= Time.deltaTime * 0.5f;
            }
            else if (x > 0.6f)
            {
                x -= Time.deltaTime;
            }
            else if (x > 0)
            {
                x -= Time.deltaTime * 1.5f;
            }
            else if (x < 0)
            {
                x = 0f;
            }
        } // 문이 닫힐 수록 속도가 증가하다 0아래로 내려가면 0으로 고정한다.

        transform.localPosition = new Vector3(x, 0f, 0f); // 오브젝트의 위치가 0,0,0이 될수있도록 보정한 값에 위에서 구한 x를 추가한다.
    }

    void DoorControll()
    {
        if (PossibleState)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger)
                {
                    SetTrigger = true;
                    is_open = !is_open;
                    Debug.Log(is_open);
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
}

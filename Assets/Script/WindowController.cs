using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{

    [HideInInspector]
    public bool is_open = false; //문이 열렸는지 아닌지를 입력받는 변수
    [HideInInspector]
    public bool PossibleState = false; //현재 문이 컨트롤 가능한 상태인지 입력받는 변수

    public bool is_right; //오른쪽 창문인지 아닌지를 받는 변수
    private WindowController Window2Controller; // 반대쪽 창문의 컴포넌트를 받아온다
    public GameObject Window2; // 반대쪽 창문의 오브젝트를 받는 변수

    [Range(0.5f, 1.0f)]
    public float OpenDistance = 0.9f; //문이 열릴 거리를 받는 함수
    
    [Header("소리 관련 변수")]
    private AudioSource Audio;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////

    float x;
    private void Start()
    {
        Audio = GetComponent<AudioSource>();
        Window2Controller = Window2.GetComponent<WindowController>();
        Audio.volume = 0.4f; //문 사운드볼륨을 0.4로 지정
    }

    private void FixedUpdate()
    {
        DoorControll();
        OpenTheDoor();


        CoolTimeSet(); //문의 쿨타임
        DataReset();
    }

    void OpenTheDoor()
    {

        if (is_open) // 만약 문이 열렸다면?
        {
            if (x < OpenDistance / 9)
            {
                x += Time.deltaTime * 0.5f;
            }
            else if (x < OpenDistance / 3)
            {
                x += Time.deltaTime * 1.2f;
            }
            else if (x < OpenDistance)
            {
                x += Time.deltaTime * 1.5f;
            }
            else if (x > OpenDistance)
            {
                x = OpenDistance;
            }
            // 문이 열릴수록 속도가 증가하다 0.9를 넘으면 0.9로 고정
        }
        else if (!is_open) // 만약 문이 닫혔다면?
        {
            if (x > OpenDistance - 0.1)
            {
                x -= Time.deltaTime * 0.5f;
            }
            else if (x > OpenDistance - 0.3)
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

        if (is_right) //오른쪽 창문일경우
            transform.localPosition = new Vector3(x, 0f, 0f); // 오브젝트의 위치가 0,0,0이 될수있도록 보정한 값에 위에서 구한 x를 추가한다.
        else //아닐경우
            transform.localPosition = new Vector3(-x, 0f, 0f);
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
                    Window2Controller.SetTrigger = true; //반대 문도 쿨타임을 적용시킨다.

                    if (is_open) // 만약 열린 문이라면?
                    {
                        Audio.clip = CloseSound; // 닫히는 문 사운드를 재생
                        Audio.Play();
                        is_open = !is_open;
                    }
                    else
                    {
                        if (Window2Controller.is_open)
                        {
                            Audio.clip = CloseSound; // 닫히는 문 사운드를 재생
                            Audio.Play();
                            Window2Controller.is_open = false;
                        }
                        else
                        {
                            Audio.clip = OpenSound; // 닫히는 문 사운드를 재생
                            Audio.Play();
                            is_open = !is_open;
                        }
                        
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

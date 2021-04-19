using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [HideInInspector]
    public bool is_open = false; //문이 열렸는지 아닌지를 입력받는 변수
    [HideInInspector]
    public bool PossibleState = false; //현재 문이 컨트롤 가능한 상태인지 입력받는 변수
    
    public bool is_Lock = false; //문이 잠겼는지 아닌지를 입력받는 변수
    public bool is_HingedDoor = false; //여닫이문인지 아닌지를 입력받는 변수

    //여닫이문 관련 변수
    private float doorOpenAngle = -90f; //열었을때 각도
    private float doorCloseAngle = 0f; //닫혔을때 각도
    private float smoot = 2.5f;


    [Header ("소리 관련 변수")]
    private AudioSource Audio;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip SoundWhenLocked;

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

        Audio.volume = 0.4f; //문 사운드볼륨을 0.5로 지정
    }

    private void FixedUpdate()
    {
        DoorControll();
        OpenTheDoor();


        CoolTimeSet();
    }

    void OpenTheDoor()
    {
        if (is_HingedDoor) // 여닫이문 일경우?
        {
            if (is_open) //여닫이문이 열렸다면?
            {
                Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoot * Time.deltaTime);
            }
            else //닫혔다면?
            {
                Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smoot * Time.deltaTime);

            }
        }
        else //미닫이 문 일경우?
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

                    if (is_Lock) // 문이 잠겨있을 경우
                    {
                        Audio.clip = SoundWhenLocked; // 잠긴 문 사운드를 재생
                        Audio.Play();
                    }
                    else // 문이 잠겨있지 않을경우
                    {


                        if (is_open) // 만약 열린 문이라면?
                        {
                            Audio.clip = CloseSound; // 닫히는 문 사운드를 재생
                            Audio.Play();
                        }
                        else
                        {
                            Audio.clip = OpenSound; // 닫히는 문 사운드를 재생
                            Audio.Play();
                        }
                        is_open = !is_open;

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
            Debug.Log(ResetCoolTime);
            if (ResetCoolTime > 0.2f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                PossibleState = false; //컨트롤 가능한 상태를 끈다.
            }
        }
    }
}

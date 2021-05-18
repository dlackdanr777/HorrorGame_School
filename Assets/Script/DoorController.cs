using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DoorState
{
    SlidingDoor,
    OneDoor,
    TwoDoor,   
}

public enum KeyStage //문이 어느 열쇠에 열릴지를 받는 변수
{
    stage0,
    stage1,
    stage2,
    stage3,
    Library,
    ArtRoom,
    DontOpen,
}

public class DoorController : MonoBehaviour
{


    [HideInInspector]
    public bool is_open = false; //문이 열렸는지 아닌지를 입력받는 변수
    [HideInInspector]
    public bool PossibleState = false; //현재 문이 컨트롤 가능한 상태인지 입력받는 변수

    
    public bool is_Lock = false; //문이 잠겼는지 아닌지를 입력받는 변수
    public bool is_Right = false; //오른쪽 문인지 아닌지 받는 변수(원도어일때만 작동)
    //여닫이문 관련 변수
    private float doorOpenAngle = 90f; //열었을때 각도
    private float doorCloseAngle = 0f; //닫혔을때 각도
    private float smoot = 2.5f;

    [Header("문 관련 변수")]
    public DoorState State;
    public KeyStage KeyStage;


    [Space]
    private AudioSource Audio;
    public AudioClip OpenSound;
    public AudioClip CloseSound;
    public AudioClip SoundWhenLocked;
    public AudioClip UnLockSound;

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////

    private GameObject Player;
    private Quaternion targetRotation; // 여닫이문의 변수
    float x;
    private void Start()
    {
        Player = GameObject.Find("Player");
        Audio = GetComponent<AudioSource>();
        Audio.volume = 1f; //문 사운드볼륨을 0.5로 지정

        if(State == DoorState.OneDoor) //여닫이 문일경우
        {
            Audio.pitch = 0.72f;
        }

        StartCoroutine("GhostDoor");//일정 확률로 문이 닫히게 하는 함수 실행
    }

    private void FixedUpdate()
    {
        DoorControll();
        OpenTheDoor();


        CoolTimeSet();
        DataReset();
    }



    IEnumerator GhostDoor() //일정 확률로 문이 닫히게 하는 함수
    {
        while (true)
        {
            if (is_open)
            {
               
                float a = Random.Range(0.0f, 100f);
                if (a <= 0.1f) //만약 0~100까지 난수에서 0.1이하의 수가 나올경우
                {
                    is_open = false; //문을 닫는다.
                    Audio.clip = CloseSound; // 닫히는 문 사운드를 재생
                    Audio.Play();
                }
            }

            yield return new WaitForSeconds(1f); //1초마다 재생
        }
    }

    void OpenTheDoor()
    {
        if (State == DoorState.SlidingDoor) // 미닫이문 일경우?
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
        else if (State == DoorState.OneDoor) //여닫이 문 일경우?
        {
            if (is_open) //여닫이문이 열렸다면?
            {
                if (is_Right) //오른쪽 문이였다면?
                {
                    targetRotation = Quaternion.Euler(0, doorOpenAngle, 0);
                }
                else
                {
                    targetRotation = Quaternion.Euler(0, -doorOpenAngle, 0);
                }


                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoot * Time.deltaTime);

            }
            else //닫혔다면?
            {
                Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smoot * Time.deltaTime);

            }

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
                    Player.GetComponent<Player_Controller>().StartCoroutine(Player.GetComponent<Player_Controller>().Noise_Generation());
                    SetTrigger = true;
                    if (is_Lock && GameManager.instance.OwnKey != (int)KeyStage) // 문이 잠겨있을 경우나 가지고 있는 키가 열수없는 문이라면
                    {
                        Audio.clip = SoundWhenLocked; // 잠긴 문 사운드를 재생
                        Audio.Play();
                    }
                    else if (is_Lock && GameManager.instance.OwnKey == (int)KeyStage) // 문이 잠겨있을 경우나 가지고 있는 키가 열수있는 문이라면
                    {
                        Audio.clip = UnLockSound; // 문을 열쇠로 여는 사운드를 재생
                        Audio.Play();
                        is_Lock = false; //잠금을 해제한다.
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
            if (ResetCoolTime > 0.1f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                PossibleState = false; //컨트롤 가능한 상태를 끈다.
            }
        }
    }
}

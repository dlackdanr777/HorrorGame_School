using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeethovenController : MonoBehaviour
{
    public AudioClip music; //음악을 받아오는 변수
    public AudioClip Endmusic; //함수가 끝날때 재생될 변수
    private AudioSource Speaker; //스피커의 오디오를 불러오는 변수

    private GameObject Player; //플레이어를 받는 변수
    private Player_Controller Player_Controller;
    [HideInInspector]
    public bool fncStart = false;
    private float timer = 0;

    [HideInInspector]
    public bool PossibleState = false; //현재 컨트롤 가능한 상태인지 입력받는 변수

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////


    private void Start()
    {
        Speaker = GetComponent<AudioSource>();
        Speaker.clip = music;
        Player = GameObject.Find("Player"); //플레이어 오브젝트를 찾아 넣는다.
        Player_Controller = Player.GetComponent<Player_Controller>();
        RunFuc();
    }

    private void FixedUpdate()
    {
        DecreaseHealth();
        StopFuc();
        CoolTimeSet();
        DataReset();
    }

    void RunFuc() //베토벤 귀신을 실행시킨다.
    {
        Speaker.Play();
        fncStart = true; //변수를 참으로 만들어 플레이어의 체력을 깍게 만든다.
        Player_Controller.is_Under_Attack = true; //플레이어의 상태를 공격받는 중으로 바꾼다.
    }

    



    void StopFuc()
    {
        
        if (PossibleState)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger)
                {
                    if (fncStart)
                    {
                        SetTrigger = true;
                        Speaker.Stop(); // 음악을 정지한다.
                        Speaker.clip = Endmusic; //음악을 변경하고 재생한다
                        Speaker.Play();
                        fncStart = false;//변수를 참으로 만들어 플레이어의 체력을 깍는것을 멈춘다.
                        Player_Controller.is_Under_Attack = false; //플레이어의 상태를 공격받는 중이 아닌것으로 바꾼다
                    }

                }

            }
        }

    }



    void DecreaseHealth() //체력을 깍게 만드는 함수
    {
        if (fncStart)
        {
            timer += Time.deltaTime;
            if (timer > 10f) //타이머가 10초를 넘었을경우
            {
                if (GameManager.instance.Health <= 0)
                {
                    GameManager.instance.Health = 0; //0이하가 되면 0으로 고정
                    Speaker.Stop(); // 음악을 정지한다.
                    fncStart = false;//변수를 참으로 만들어 플레이어의 체력을 깍는것을 멈춘다.
                    Player_Controller.is_Under_Attack = false; //플레이어의 상태를 공격받는 중이 아닌것으로 바꾼다

                }
                else
                {
                    GameManager.instance.Health -= Time.deltaTime; //초만큼 체력을 깍는다.
                    
                }


               
            }
        }
        else
        {
            timer = 0;
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
                Player_Controller.is_Under_Attack = false; //플레이어의 상태를 공격받는 중이 아닌 것으로 변경한다
            }
        }
    }
}

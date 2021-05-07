using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAlarmController : MonoBehaviour
{
    [SerializeField]
    private AudioSource Audio; //사운드를 받을 변수
    [SerializeField]
    private AudioClip Clip; //경보벨의 사운드를 받을 변수

    [HideInInspector]
    public bool is_TurnOn; //경보기가 켜져있을때 참을 받는 변수

    [HideInInspector]
    public bool PossibleState = false; //현재 컨트롤 가능한 상태인지 입력받는 변수

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정
    private float time = 0;
    /// ///////////////////////////////////



    private void Start()
    {
        Audio = GetComponent<AudioSource>();
        StartCoroutine("SoundOff");
    }
    private void FixedUpdate()
    {
        if (!is_TurnOn)
        {
            Audio.Stop();
        }


        SoundOn();

        CoolTimeSet();
        DataReset();
    }




    IEnumerator SoundOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (is_TurnOn) //경보벨이 켜져있을 경우
            {
                time += 0.1f;

                if (time > 30f) //30초가 지났을 경우
                {
                    is_TurnOn = false; //소리를 끈다.
                }
            }
            else
            {
                time = 0;
            }
        }
    }

    void SoundOn()
    {
        if (PossibleState)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger)
                {
                    SetTrigger = true;
                    if (!is_TurnOn)
                    {
                        is_TurnOn = true;
                        Audio.clip = Clip;
                        Audio.Play();
                    }
                    else
                    {
                        is_TurnOn = false;
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


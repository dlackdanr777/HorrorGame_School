using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [HideInInspector]
    public bool is_TurnOn = false;
    [HideInInspector]
    public bool PossibleState = false; //현재 조명이 컨트롤 가능한 상태인지 입력받는 변수

    [Header ("형광등 관련 변수")]
    public bool Noton = false; //작동 안되는 형광등의 경우 킨다.
    public GameObject[] FluorescentLamp = new GameObject[6]; //형광등 불빛6개를 받아옴
    public Material LampMaterial; //형광등 점멸효과를 위해 마테리얼을 받아옴
    private AudioSource Sound;
    [Header ("사운드 관련 변수")]
    public AudioClip TurnOnSound;
    public AudioClip TurnOffSound;

    private bool SetTrigger = false;
    private bool ResetTrigger = false;
    private float CoolTime;
    private float ResetCoolTime; //리셋 쿨타임
    private float SetCooltime = 1f; // 쿨타임을 1초로 설정


    private void Start()
    {
        Sound = GetComponent<AudioSource>();
        Sound.volume = 0.5f;
        if(LampMaterial != null) //마테리얼변수에 값이 담겨있을 경우만 실행
        {
            LampMaterial.SetColor("_EmissionColor", new Color(0, 0, 0));
        }
        
    }


    private void FixedUpdate()
    {
        TurnOnTheLight();

        CoolTimeSet();
        DataReset();

    }

    void TurnOnTheLight()
    {
        if (PossibleState) // 컨트롤 가능한 상태일때
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!SetTrigger) //쿨타임 온
                {
                    SetTrigger = true;
                    if (!Noton) //켜지는 상태일경우
                    {
                        if (is_TurnOn)
                        {
                            for (int i = 0; i < FluorescentLamp.Length; i++)
                            {
                                FluorescentLamp[i].SetActive(false);
                            }
                            LampMaterial.SetColor("_EmissionColor", new Color(0, 0, 0) * 10f);
                            Sound.clip = TurnOffSound;
                            Sound.Play();


                        }
                        else
                        {
                            for (int i = 0; i < FluorescentLamp.Length; i++)
                            {
                                FluorescentLamp[i].SetActive(true);
                            }
                            LampMaterial.SetColor("_EmissionColor", new Color(255, 255, 255) * 10f);
                            Sound.clip = TurnOnSound;
                            Sound.Play();
                        }
                        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
                        is_TurnOn = !is_TurnOn;
                    }
                    else // 안켜지는 상태일 경우
                    {
                        Debug.Log("2");
                        if (is_TurnOn) //버튼 딸칵거리는 소리만 나게 한다.
                        {
                            Sound.clip = TurnOffSound;
                            Sound.Play();


                        }
                        else //버튼 딸칵거리는 소리만 나게 한다.
                        {
                            Sound.clip = TurnOnSound;
                            Sound.Play();
                        }
                        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
                        is_TurnOn = !is_TurnOn;
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
            if (ResetCoolTime > 0.2f) //지정한 쿨타임 시간이 지나면
            {
                ResetTrigger = false; //리셋트리거를 끈다
                ResetCoolTime = 0f; //시간을 초기화
                PossibleState = false; //컨트롤 가능한 상태를 끈다.
            }
        }
    }
}

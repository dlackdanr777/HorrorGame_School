using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopKiller : MonoBehaviour
{

    private bool is_Watching = false; //플레이어가 포스터를 보고있을때 참이되는 함수

    [SerializeField]
    private GameObject Audio_Obj; //소리를 낼 오브젝트를 받아오는 변수
    private AudioSource Audio; //소리를 낼 오브젝트에서 사운드 소스를 받아올 변수
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
        Audio = Audio_Obj.GetComponent<AudioSource>();
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && is_Watching) //만약 플레이어가 범위내에 있고 포스터를 보고있는 중이라면?
        {
            time += Time.deltaTime;
            Debug.Log(time);

            if (time > 5f) //만약 5초이상 보고있으면?
            {
                Debug.Log("재생");
                time = 0;
                GetComponent<BoxCollider>().enabled = false;
                Audio.Play(); //소리를 재생한다.
                this.enabled = false; //스크립트를 끈다.

            }
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") //만약 플레이어가 범위밖으로 나갔으면?
        {
            time = 0;
        }
    }

    private void OnBecameInvisible()
    {
        is_Watching = false;
    }

    private void OnBecameVisible()
    {
        is_Watching = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeethovenController : MonoBehaviour
{
    public GameObject[] Speaker = new GameObject[10]; //스피커를 받아오는 변수
    public AudioClip music; //음악을 받아오는 변수
    public AudioSource[] Speaker_ = new AudioSource[10]; //스피커의 오디오를 불러오는 변수

    private GameObject Player; //플레이어를 받는 변수

    private bool fncStart = false;
    float i = 0;

    private void Start()
    {
        for(int i = 0; i < Speaker.Length; i++)
        {
            Speaker_[i] = Speaker[i].GetComponent<AudioSource>(); //스피커의 오디오소스를 스피커_에 넣는다.
            Speaker_[i].clip = music; //스피커에 음악을 실행시킬 준비를 한다.
        }
        Player = GameObject.Find("Player"); //플레이어 오브젝트를 찾아 넣는다.
        RunFuc();
    }

    void RunFuc() //베토벤 귀신을 실행시킨다.
    {
        for (int i = 0; i < Speaker.Length; i++)
        {
            Speaker_[i].Play(); //음악을 실행시킨다.
        }
        
        while( i < 5f)
        {
            i = Time.deltaTime;
            Debug.Log(i);
        }
        i = 0f;
        fncStart = true; //변수를 참으로 만들어 플레이어의 체력을 깍게 만든다.
    }

    void StopFuc()
    {
        for (int i = 0; i < Speaker.Length; i++)
        {
            Speaker_[i].Stop();
        }

    }
}

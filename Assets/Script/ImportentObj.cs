using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportentObj : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Poster; //포스터를 받아올 변수
    [SerializeField]
    private GameObject[] Battery; //배터리들을 받아올 변수
    [SerializeField]
    private GameObject[] Key1; //1학년 키를 받아올 변수
    [SerializeField]
    private GameObject[] Key2; //2학년 키를 받아올 변수

    [SerializeField]
    private GameObject[] ArtRoomKey; //미술실 키를 받아올 변수

    [SerializeField]
    private GameObject[] LibraryKey; //도서관 키를 받아올 변수

    [SerializeField]
    private GameObject[] Monster; //몬스터를 받아올 변수

    [SerializeField]
    private GameObject[] Statue; //석상들을 받아올 변수


    [SerializeField]
    private int PosterGenerationCount = 8; //포스터를 몇개생성할지 받는 변수
    [SerializeField]
    private int BatteryGenerationCount = 15; //배터리를 몇개생성할지 받는 변수
    private int randArray;

    private int count = 0;
    private void Start()
    {
        for(int i = 0; i < Poster.Length; i++)
        {
            Poster[i].SetActive(false);//포스터를 전부 비활성화시킨다.
        }
        for (int i = 0; i < Battery.Length; i++)
        {
            Battery[i].SetActive(false);//배터리를 전부 비활성화시킨다.
        }
        for (int i = 0; i < Key1.Length; i++)
        {
            Key1[i].SetActive(false);//키1를 전부 비활성화시킨다.
        }
        for (int i = 0; i < Key2.Length; i++)
        {
            Key2[i].SetActive(false);//키2를 전부 비활성화시킨다.
        }
        for (int i = 0; i < LibraryKey.Length; i++)
        {
            LibraryKey[i].SetActive(false);// 도서관 키를 전부 비활성화시킨다.
        }
        for (int i = 0; i < ArtRoomKey.Length; i++)
        {
            ArtRoomKey[i].SetActive(false);//미술실 키를 전부 비활성화시킨다.
        }


        while (count < PosterGenerationCount) //카운트가 변수 이하일경우 진행
        {
            count = 0; //카운트 초기화
            randArray = Random.Range(0, Poster.Length); //난수를 생성
            Poster[randArray].SetActive(true); //난수로 생성된 번호로 포스터를 킨다.
            Debug.Log("포스터켜기");
            for (int i = 0; i < Poster.Length; i++)
            {
                if (Poster[i].activeSelf) //만약 해당포스터가 켜져있을경우
                {
                    ++count; //카운트를 하나 늘린다.

                }
            }

        }
        count = 0; //카운트 초기화
        while (count < BatteryGenerationCount) //카운트가 변수 이하일경우 진행
        {
            count = 0; //카운트 초기화
            randArray = Random.Range(0, Battery.Length); //난수를 생성
            Battery[randArray].SetActive(true); //난수로 생성된 번호로 배터리를 킨다.
            Debug.Log("배터리 켜기");
            for (int i = 0; i < Battery.Length; i++)
            {
                if (Battery[i].activeSelf) //만약 해당포스터가 켜져있을경우
                {
                    ++count; //카운트를 하나 늘린다.

                }
            }

        }

        Key1[Random.Range(0, Key1.Length)].SetActive(true); //키1을 랜덤으로 활성화시킨다.
        Key2[Random.Range(0, Key2.Length)].SetActive(true); //키2을 랜덤으로 활성화시킨다.
        LibraryKey[Random.Range(0, LibraryKey.Length)].SetActive(true); //키2을 랜덤으로 활성화시킨다.
        ArtRoomKey[Random.Range(0, ArtRoomKey.Length)].SetActive(true); //키2을 랜덤으로 활성화시킨다.
        //Monster[Random.Range(0, Monster.Length)].SetActive(true);
    }

    private void Update()
    {
        if(GameManager.instance.Score >= 4) //만약 스코어가 4이상이 되었을경우?
        {
            Statue[0].SetActive(true);
            Statue[1].SetActive(true);
            Statue[2].SetActive(true);
        }

        if (GameManager.instance.Score >= 6) //만약 스코어가 4이상이 되었을경우?
        {
            Statue[3].SetActive(true);
            Statue[4].SetActive(true);
            Statue[5].SetActive(true);
            Statue[6].SetActive(true);
            Monster[1].SetActive(true);

        }
    }
}

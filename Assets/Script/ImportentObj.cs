using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class ImportentObj : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Poster; //포스터를 받아올 변수
    private GameObject[] Battery; //배터리들을 받아올 변수

    private int randArray;
    private int[] boxArray = new int[8];

    private int count = 0;
    private void Start()
    {
        for(int i = 0; i < Poster.Length; i++)
        {
            Poster[i].SetActive(false);//포스터를 전부 비활성화시킨다.
        }
        while(count < 8) //카운트가 8이하일경우 진행
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
    }
}

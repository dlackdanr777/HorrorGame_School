using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSound : MonoBehaviour
{
    private Player_Controller Player; //플레이어의 스크립트를 받아옴
    private AudioSource Audio; //플레이어 발의 컴포넌트를 받아온다

    private void Start()
    {
        Player = transform.root.gameObject.GetComponent<Player_Controller>();
        Audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Player.Player_State == (int)Player_Controller.State.is_Sit) // 플레이어가 앉아있을 경우
        {
            Audio.volume = 0.2f;
        }
        else if(Player.Player_State == (int)Player_Controller.State.is_Walk || Player.Player_State == (int)Player_Controller.State.is_Stop) // 플레이어가 걷거나 서있을 경우?
        {
            Audio.volume = 0.5f;
        }
        
        else if(Player.Player_State == (int)Player_Controller.State.is_Run)
        {
            Audio.volume = 1f;
        }

        if (other.gameObject.tag == "Floor")
        {
            Audio.Play();
            Debug.Log("발소리");
        }
    }
}

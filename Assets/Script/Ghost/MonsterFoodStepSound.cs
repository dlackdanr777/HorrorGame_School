using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFoodStepSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] FootStepSound = new AudioClip[5]; //발소리를 저장할 변수
    private AudioSource AudioSource;
    private GameObject Monster; //자기자신을 불러올 변수
    private MonsterController MonsterController; //자기자신에게 받아올 스크립트를 불러올 변수
    private NavMeshAgent m_Nav;

    private void Start()
    {
        Monster = transform.root.gameObject; //최상위오브젝트를 받아와 넣는다.
        MonsterController = Monster.GetComponent<MonsterController>();
        AudioSource = GetComponent<AudioSource>();
        m_Nav = Monster.GetComponent<NavMeshAgent>();
        StartCoroutine("AudioPlay");
    }


    IEnumerator AudioPlay()
    {
        while (true)
        {
            if (Mathf.Abs(m_Nav.velocity.x + m_Nav.velocity.z) < 0.1f) //몬스터가 움직이지 않을경우
            {
                yield return new WaitForSeconds(0.1f);
                AudioSource.Stop(); //사운드를 정지한다.
            }

            else if (MonsterController.m_state == MonsterController.box_state) //전상태와 현재상태가 같으면?
            {
                switch (MonsterController.m_state) //몬스터의 상태를 받는다.
                {

                    case m_state.is_Stop: //정지상태일경우
                        yield return new WaitForSeconds(0.1f);
                        AudioSource.Stop(); //사운드를 정지한다.
                        break;

                    case m_state.is_Walk: //걷는상태일경우
                        yield return new WaitForSeconds(0.7f);
                        AudioStart(0.5f, Random.Range(0, FootStepSound.Length));
                        break;

                    case m_state.is_Boundary: //경계상태일경우
                        yield return new WaitForSeconds(0.7f);
                        AudioStart(0.5f, Random.Range(0, FootStepSound.Length));
                        break;

                    case m_state.Tracking: //추격중일경우
                        yield return new WaitForSeconds(0.5f);
                        AudioStart(1f, Random.Range(0, FootStepSound.Length));
                        break;
                }

            }

            else if (MonsterController.m_state != MonsterController.box_state) // 현재상태와 전 상태가 다를경우
            {
                switch (MonsterController.m_state) //몬스터의 상태를 받는다.
                {
                    case m_state.is_Stop: //정지상태일경우
                        yield return new WaitForSeconds(0.1f);
                        AudioSource.Stop(); //사운드를 정지한다.
                        break;

                    case m_state.is_Walk: //걷는상태일경우
                        yield return new WaitForSeconds(0.7f);
                        AudioStart(0.5f, Random.Range(0, FootStepSound.Length));
                        break;

                    case m_state.is_Boundary: //경계상태일경우
                        yield return new WaitForSeconds(2.55f);
                        AudioStart(0.5f, Random.Range(0, FootStepSound.Length));
                        break;

                    case m_state.Tracking: //추격중일경우
                        yield return new WaitForSeconds(4.05f);
                        AudioStart(1f, Random.Range(0, FootStepSound.Length));
                        break;
                }
            }
        }
       
    }

    void AudioStart(float volume, int i)
    {
        AudioSource.volume = volume;
        AudioSource.clip = FootStepSound[i];
        AudioSource.Play();
    }


}

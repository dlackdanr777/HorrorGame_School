using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum m_state
{
    is_Stop,
    is_Walk,
    is_Boundary,
    Tracking,
}
public class MonsterController : MonoBehaviour
{
    public m_state m_state; //몬스터의 상태를 받는 변수
    public m_state box_state; //몬스터의 상태를 임시로 저장하는 변수


    private GameObject Player; //플레이어의 오브젝트를 받아오는 변수
    private Player_Controller Player_Controller;
    private NavMeshAgent Nav; //해당 오브젝트의 네비메쉬를 받아오는 변수
    private BoxCollider Box; //자신의 박스콜라이더를 받을 변수
    private Vector3 Player_location; //플레이어의 위치를 받는 변수
    private Vector3 Set_location; //자신의 위치를 저장할 변수

    [SerializeField]
    private GameObject[] Random_location = new GameObject[4]; //랜덤으로 이동할 위치를 변경할 변수
    private Animator Ani; //자신의 애니메이터 콜라이더를 저장할 변수

    //소리관련 변수
    private AudioSource Audio; //자신의 오디오소스를 저장할 변수
    [SerializeField]
    private AudioClip[] AudioClip = new AudioClip[5]; //소리를 저장할 변수
    [SerializeField]


    //시야 관련 변수
    private float m_angle = 80f; //시야각
    private float m_distance = 5f; //시야길이

    //충돌관련 변수
    private bool Did_Collide = false; //플레이어와 충돌했나 안했나를 받는 변수
    private bool Out_Collide = false; //충돌범위에서 벗어낫나 아닌가를 받는 변수
    private bool Player_Detection = false; //플레이어가 시야 범위 안에 있으면 참을 받는 변수

    /////////////////쿨타임 관련 변수
    private float time = 0;
    /// ///////////////////////////////////
    /// 
    private void Start()
    {
        Player = GameObject.Find("Player");
        Player_Controller = Player.GetComponent<Player_Controller>();

        Nav = GetComponent<NavMeshAgent>();
        Box = GetComponent<BoxCollider>();

        Ani = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();

        Set_location = transform.position;
        Audio_Change(AudioClip[0], 1, true);
        StartCoroutine("State_Update");
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.is_Start)
        {
            Change_State();
            Sight();
            HearAsound();
        }

        Ani.SetFloat("Speed", Mathf.Abs(Nav.velocity.x + Nav.velocity.z));

        if(m_state == m_state.Tracking) //만약 추적중일때
        {
            if(Vector3.Distance(transform.position, Player.transform.position) < 1f) //근접하게되면
            {
                GameManager.instance.Health = 0f;
            }
        }
    }

    void Change_State()
    {
        if (!Out_Collide) //소리범위에 감지되면?
        {
            Player_location = Player.transform.position; //플레이어의 위치를 저장시킨다.
        }

        if (Did_Collide) //소리감지범위에 플레이어가 감지됬으면?
        {
            if(m_state == m_state.is_Stop || m_state == m_state.is_Walk) //멈춰있거나 걷는 상태일경우?
            {
                m_state = m_state.is_Boundary; //상태를 추적중으로 바꾼다.
            }

        }
        
        if(Player_Detection) //몬스터의 시야범위내에 플레이어가 들어오면?
        {
            m_state = m_state.Tracking; //상태를 추격중으로 바꾼다.
        }


        if (Mathf.Abs(Nav.velocity.x + Nav.velocity.z) < 0.1f) //몬스터가 움직이지 않을경우
        {
            time += Time.deltaTime;
            if (time > 8f)
            {
                Debug.Log("멈춤");
                Did_Collide = false;
                Player_Detection = false;
                time = 0;
                Set_location = Random_location[Random.Range(0, Random_location.Length)].transform.position;//랜덤의 위치로 이동시키게 변수에 입력한다.
                m_state = m_state.is_Walk;
            }

        }
        else
        {
            time = 0;
        }

    }


    IEnumerator State_Update()
    {
        while (true) //무한반복 시킨다.
        {
            if (m_state == box_state) //전상태와 현재상태가 같으면?
            {
                switch (m_state)
                {
                    case m_state.is_Stop: //만약 서있는 중이라면
                        Nav.Stop();
                        break;

                    case m_state.is_Walk: //만약 걷는중이라면
                        Nav.destination = Set_location; //지정된 위치로 이동하게한다.
                        Nav_Start(1f);
                        break;

                    case m_state.is_Boundary: //추적중이라면
                        Nav.destination = Player_location; //플레이어 위치를 추적한다.
                        Nav_Start(1f);
                        break;

                    case m_state.Tracking: //추격중이라면
                        Nav.destination = Player_location; //플레이어 위치를 추적한다.
                        Nav_Start(4.5f);
                        break;
                }

            }

            else if (m_state != box_state) // 현재상태와 전 상태가 다를경우
            {
                box_state = m_state; //변수에 임시로 저장한다.
                switch (m_state)
                {
                    case m_state.is_Stop: //만약 서있는 중이라면
                        Ani.SetInteger("State", 0);
                        Nav_Stop();
                        Audio_Change(AudioClip[0], 1, true);
                        break;

                    case m_state.is_Walk: //만약 걷는중이라면
                        Ani.SetInteger("State", 0);
                        Nav_Start(1.5f);
                        Nav.destination = Set_location; //지정된 위치로 이동하게한다.
                        Audio_Change(AudioClip[0], 1, true);
                        break;

                    case m_state.is_Boundary: //추적중이라면
                        Ani.SetInteger("State", 0);
                        Nav_Stop();
                        Audio_Change(AudioClip[1], 1, false); //오디오를 변경한다.
                        yield return new WaitForSeconds(2.0f); //2초를 기다린다.
                        Audio_Change(AudioClip[0], 1, true); //오디오를 변경한다.
                        Nav_Start(1.5f);//속도를 1로 지정하고 네비메쉬를 킨다
                        Nav.destination = Player_location; //플레이어 마지막 위치로 이동하게 한다
                        break;

                    case m_state.Tracking: //추격중이라면
                        Nav_Stop();
                        yield return new WaitForSeconds(0.5f);
                        Ani.SetInteger("State", 1);
                        yield return new WaitForSeconds(0.5f);
                        Audio_Change(AudioClip[2], 1, false);
                        yield return new WaitForSeconds(2.5f);
                        Audio_Change(AudioClip[0], 2, true);
                        Ani.SetInteger("State", 0);
                        Nav_Start(4); //속도를 4로 지정하고 네비메쉬를 킨다
                        Nav.destination = Player_location; //플레이어 마지막 위치로 이동하게 한다
                        break;
                }

            }

            yield return new WaitForSeconds(0.5f); //1초를 기다린다.

        }
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }


    void HearAsound()
    {
        //만약 플레이어가 앉아있거나 멈춰있으면
        if (Player_Controller.Player_State == (int)Player_Controller.State.is_Stop || Player_Controller.Player_State == (int)Player_Controller.State.is_Sit)
        {
            Box.size = new Vector3(3, 2.5f, 3); //충돌 범위를 줄인다.

        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Walk) //플레이어가 걷는 중이라면
        {
            Box.size = new Vector3(20, 2.5f, 20); //충돌 범위를 조정한다
        }
        else if (Player_Controller.Player_State == (int)Player_Controller.State.is_Run) //플레이어가 뛰는 중이라면
        {

            Box.size = new Vector3(35, 8f, 35); //충돌 범위를 조정한다
        }
    }

    void Sight() //몬스터의 시야 관련 함수
    {
        Vector3 _leftBoundary = BoundaryAngle(-m_angle * 0.5f); //왼쪽 눈의 시야각을 설정
        Vector3 _rightBoundary = BoundaryAngle(m_angle * 0.5f);//오른쪽 눈의 시야각을 설정

        Debug.DrawRay(transform.position + (transform.up * 1f) + transform.forward, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + (transform.up * 1f) + transform.forward, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, m_distance); //근처에 있는 오브젝트들의 콜라이더를 받아온다

        if (Player_Detection) //만약 시야범위 내라면?
        {
            Did_Collide = true; //충돌 변수도 참으로 만든다.
        }


        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.tag == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < m_angle * 0.5f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + (transform.up * 1f) + transform.forward, _direction, out hit, m_distance)) //플레이어의 위치로 레이를 쏜다
                    {
                        if (hit.transform.tag == "Player") //플레이어가 닿으면?
                        {
                            Player_Detection = true; //시야포착을 참으로 만든다.
                            Debug.DrawRay(transform.position + transform.up * 1f + transform.forward, _direction, Color.blue);
                        }
                    }
                    else //시야에 닿지않으면?
                    {
                        Player_Detection = false;
                    }
                }
                else //시야각에서 벗어나있으면?
                {
                    Player_Detection = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") //트리거에 플레이어가  충돌중이라면
        {
            Did_Collide = true; //충돌변수에 참을 준다.
            Out_Collide = false; //충돌범위안이면 거짓을 준다
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") //트리거에서 플레이어가 벗어났다면
        {
            Out_Collide = true; //충돌범위에서 벗어나면 이 변수에 참을준다.
        }
    }


    void Nav_Start(float Speed)//네비메쉬를 시작할때 필요한 함수
    {
        Nav.Resume();
        Nav.speed = Speed;
    }

    void Nav_Stop() //네비메쉬를 정지시킬때 필요한 함수
    {
        Nav.Stop(); //일단 정지시킨다.
        Nav.speed = 0f;
    }

    void Audio_Change(AudioClip Clip, float Pitch, bool loop) //오디오를 바꿀때 쓰는 함수
    {
        Audio.Stop();
        Audio.clip = Clip;
        Audio.loop = loop;
        Audio.pitch = Pitch;
        Audio.Play();
    }
}

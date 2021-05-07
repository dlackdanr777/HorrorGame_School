using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Obj_State[] Slot_State; //슬롯에 무슨 종류의 아이템이 있는지 받는 변수
    public bool[] Choice_Slot; //선택중인 슬롯에게 참을 주는 변수
    public bool[] fullCheck; //해당 아이템슬롯에 아이템이 있는지 아닌지를 받는 변수
    public Image[] slots; //아이템슬롯을 넣는 변수
    public Text[] slot_Text; //아이템의 이름을 보여줄 텍스트
    public Sprite[] Item_Image; //아이템의 이미지를 넣는 변수

    /////////////////쿨타임 관련 변수
    private bool SetTrigger = false;
    private float CoolTime;
    private float SetCooltime = 0.5f; // 쿨타임을 1초로 설정
    /// ///////////////////////////////////


    private void Update()
    {
        NumKey();
        ChoiceSlotState();
        slotfnc();
        CoolTimeSet();
    }



    void ChoiceSlotState() //선택한 슬롯의 아이템에 따라 설정을 변경하는 함수
    {
       
        for(int i = 0; i < slots.Length; i++)
        {
            if (Choice_Slot[i]) //만약 i번째 배열이 선택되었을경우
            {
                switch (Slot_State[i]) //i번째 슬롯의 아이템 정보를 가져온다.
                {
                    case Obj_State.Obj: //만약 아무효과없는 아이템이거나 빈 공간일경우
                        GameManager.instance.OwnKey = 0; //키변수를 0으로 만든다.
                        slot_Text[i].text = " "; //해당슬롯의 텍스트를 채운다
                        break;
                    case Obj_State.Key1: //만약 마스터키1 일경우
                        GameManager.instance.OwnKey = 1; //키변수를1으로 만든다.
                        slot_Text[i].text = "1학년 열쇠"; //해당슬롯의 텍스트를 채운다
                        break;
                    case Obj_State.Key2: //만약 마스터키2일경우
                        GameManager.instance.OwnKey = 2; //키변수를 2으로 만든다.
                        slot_Text[i].text = "2학년 열쇠"; //해당슬롯의 텍스트를 채운다
                        break;
                    case Obj_State.Key3: //만약 마스터키3일경우
                        GameManager.instance.OwnKey = 0; //키변수를 3으로 만든다.
                        slot_Text[i].text = "3학년 열쇠"; //해당슬롯의 텍스트를 채운다
                        break;
                    case Obj_State.LibraryKey: //만약 도서관 키일경우
                        GameManager.instance.OwnKey = 4; //키변수를 4로만든다.
                        slot_Text[i].text = "도서관 열쇠"; //해당슬롯의 텍스트를 채운다
                        break;
                    case Obj_State.ArtRoomKey: //만약 미술실 키일경우
                        GameManager.instance.OwnKey = 5; //키변수를 5로만든다.
                        slot_Text[i].text = "미술실 열쇠"; //해당슬롯의 텍스트를 채운다
                        break;

                }
            }

        }
    }

    void slotfnc() //아이템슬롯을 관리하는 함수
    {
        for (int i = 0; i < slots.Length; i++)
        {
            switch (Slot_State[i]) //i번째 슬롯의 아이템 정보를 가져온다.
            {
                case Obj_State.Obj: //만약 아무효과없는 아이템이거나 빈 공간일경우
                    slots[i].color = new Color(255, 255, 255, 0);//알파값을 0으로 만든다.
                    slot_Text[i].text = " "; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.Key1: //만약 마스터키1 일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "1학년 열쇠"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.Key2: //만약 마스터키2일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "2학년 열쇠"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.Key3: //만약 마스터키3일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "3학년 열쇠"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.LibraryKey: //만약 도서관 키일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "도서관 열쇠"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.ArtRoomKey: //만약 미술실 키일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "미술실 열쇠"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.Battery: //만약 배터리일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "배터리"; //해당슬롯의 텍스트를 채운다
                    break;
                case Obj_State.Medkit: //만약 약일경우
                    slots[i].color = new Color(255, 255, 255, 255);//알파값을 255으로 만든다.
                    slot_Text[i].text = "치료제"; //해당슬롯의 텍스트를 채운다
                    break;
            }
        }


    }



    void NumKey()
    {
        if (Input.GetKey(KeyCode.Alpha1))//숫자키 1을 눌렀을때
        {
            slotSet(0);
        }

        if (Input.GetKey(KeyCode.Alpha2)) //숫자키 2를눌렀을때
        {
            slotSet(1);
        }

        if (Input.GetKey(KeyCode.Alpha3)) //숫자키 3를눌렀을때
        {
            slotSet(2);

        }

        if (Input.GetKey(KeyCode.Alpha4)) //숫자키 4를눌렀을때
        {
            slotSet(3);
        }

        if (Input.GetKey(KeyCode.Alpha5)) //숫자키 5를눌렀을때
        {
            slotSet(4);
        }

        if (Input.GetKey(KeyCode.Alpha6)) //숫자키 6를눌렀을때
        {
            slotSet(5);
        }

        if (Input.GetKey(KeyCode.Alpha7)) //숫자키 7를눌렀을때
        {
            slotSet(6);
        }

        if (Input.GetKey(KeyCode.Alpha8)) //숫자키 8를눌렀을때
        {
            slotSet(7);
        }
    }

    void slotSet(int a)
    {
        if (!SetTrigger)
        {
            SetTrigger = true;
            if (Slot_State[a] == Obj_State.Battery && Choice_Slot[a]) //만약 배터리이고 선택중일 경우엔?
            {
                fullCheck[a] = false; //해당 슬롯의 사용여부를 거짓으로 만든다.
                Slot_State[a] = Obj_State.Obj; //해당 슬롯을 빈공간으로 만든다.
                slots[a].sprite = Item_Image[0]; //해당 슬롯의 이미지를 없앤다.
                GameManager.instance.Battery_Gauge = 100f; //배터리 게이지를 100%로 만든다.
            }
            if (Slot_State[a] == Obj_State.Medkit && Choice_Slot[a]) //만약 약이고 선택중일 경우엔?
            {
                fullCheck[a] = false; //해당 슬롯의 사용여부를 거짓으로 만든다.
                Slot_State[a] = Obj_State.Obj; //해당 슬롯을 빈공간으로 만든다.
                slots[a].sprite = Item_Image[0]; //해당 슬롯의 이미지를 없앤다.
                GameManager.instance.Health = 100f; //채력 게이지를 100%로 만든다.
            }

            slots[a].transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255); //알파값을 최대로 높힌다.
            Choice_Slot[a] = true; //선택중인 슬롯에게 참을 준다.
            for (int i = 0; i < slots.Length; i++)
            {
                if (i != a)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0); //알파값을 낮춘다.
                    Choice_Slot[i] = false; //선택중인 슬롯이외에 거짓값을 준다.
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.SceneManagement;

public class HallManager : MonoBehaviour {

    public InputField roomName;

    public InputField roomContent;

    public Button[] buttons;

    public GameObject roomInfo;

	void Start () {
        StartCoroutine(GetUnitDatas(GameData.url + "/data"));
	}
	
	void Update () {
		
	}

    public void CreateBtn()
    {
        string url = GameData.url + "/room/create";
        WWWForm form = new WWWForm();
        form.AddField("roomName", roomName.text);
        form.AddField("roomContent", roomContent.text);
        StartCoroutine(CreateRoom(url, form));
    }

    public void JoinBtn(int roomId)
    {
        GameData.roomId = roomId;
        SceneManager.LoadScene("Room");
    }

    public void FlushBtn()
    {
        StartCoroutine(FindRooms(GameData.url + "/page/roomList"));
    }

    IEnumerator CreateRoom(string url, WWWForm form)
    {
        WWW data = new WWW(url, form);
        yield return data;
        if(data.error == null)
        {
            JsonReader reader = new JsonReader(data.text);
            JsonData jsonData = JsonMapper.ToObject(reader);
            GameData.roomId = (int)jsonData["roomId"];
            SceneManager.LoadScene("Room");
        }
    }

    IEnumerator FindRooms(string url)
    {
        WWW data = new WWW(url);
        yield return data;
        if(data.error == null)
        {
            JsonReader reader = new JsonReader(data.text);
            JsonData jsonData = JsonMapper.ToObject(reader);
            if ((int)jsonData["count"] > 0)
            {
                JsonData list = jsonData["data"];
                //buttons[0].transform.Find("Text").GetComponent<Text>().text = (string)list[0]["roomName"];
                buttons[0].GetComponent<BtnRoomData>().roomName = (string)list[0]["roomName"];
                buttons[0].GetComponent<BtnRoomData>().roomContent = (string)list[0]["roomContent"];
                buttons[0].GetComponent<BtnRoomData>().roomId = (int)list[0]["roomId"];
                buttons[0].onClick.AddListener(
                    delegate ()
                    {
                        roomInfo.transform.Find("Text").GetComponent<Text>().text = buttons[0].GetComponent<BtnRoomData>().roomName;
                        roomInfo.transform.Find("Text (1)").GetComponent<Text>().text = buttons[0].GetComponent<BtnRoomData>().roomContent;
                        roomInfo.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                            delegate ()
                            {
                                JoinBtn(buttons[0].GetComponent<BtnRoomData>().roomId);
                            }
                            );
                    }
                );
            }
            else
            {
                //buttons[0].transform.Find("Text").GetComponent<Text>().text = "空";
            }
        }
        else
        {
            Debug.Log(data.error);
        }
    }
    //获取单位数据
    IEnumerator GetUnitDatas(string url)
    {
        WWW data = new WWW(url);
        yield return data;
        if(data.error == null)
        {
            JsonReader reader = new JsonReader(data.text);
            JsonData jsonData = JsonMapper.ToObject(reader);
            JsonData list = jsonData["data"];
            //GameData.unitDatas = new List<UnitData>();
            for (int i = 0; i < list.Count; i++)
            {
                UnitData unitData = new UnitData
                {
                    Id = (int)list[i]["id"],
                    Type = (string)list[i]["type"],
                    Name = (string)list[i]["name"],
                    HP = (int)list[i]["hp"],
                    Atteck = (int)list[i]["atteck"],
                    Cost = (int)list[i]["cost"],
                    Level = (int)list[i]["level"],
                    Info = (string)list[i]["info"]
                };
                GameData.unitDatas.Add(unitData);
            }
        }
    }
}

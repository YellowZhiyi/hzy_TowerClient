using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.WebSocket;
using LitJson;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {

    public GameObject panel;//准备面板

    public GameObject gPanel;//游戏面板




	void Start () {
        panel.transform.Find("Button").GetComponent<Button>().interactable = false;
        gPanel.transform.GetComponent<GameManger>().enabled = false;
        gPanel.transform.GetComponent<GameFightData>().enabled = false;
        gPanel.SetActive(false);
    }
	
	void Update () {
		
	}

    public void DisplayPanel(string roomName,string p1Name,string p2Name)
    {
        panel.transform.Find("room").GetComponent<Text>().text = roomName;
        panel.transform.Find("p1").GetComponent<Text>().text = p1Name;
        panel.transform.Find("p2").GetComponent<Text>().text = p2Name;
        if(p1Name != "空" && p2Name != "空")
        {
            panel.transform.Find("Button").GetComponent<Button>().interactable = true;
            panel.transform.Find("p2info").gameObject.SetActive(true);
            panel.transform.Find("talking").gameObject.SetActive(true);
        }
    }

    private int _ready = 0;
    public void DisplayReady(string who,string msg)
    {
        if(who == "p1")
        {
            panel.transform.Find("ready").GetComponent<Toggle>().isOn = true;
            _ready++;
        }
        else
        {
            panel.transform.Find("ready (1)").GetComponent<Toggle>().isOn = true;
            _ready++;
        }
        //双方都准备好了
        if(_ready == 2)
        {
            panel.SetActive(false);
            gPanel.SetActive(true);
            gPanel.transform.GetComponent<GameFightData>().enabled = true;
            gPanel.transform.GetComponent<GameManger>().enabled = true;
            //请求玩家数据
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
            {
                { "Type", "PlayerStartInfo" }
            };
            GameData.webSocket.Send(JsonMapper.ToJson(result));
            
        }
    }

    public void ReadyBtn()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
        {
            { "Type", "Ready" }
        };
        GameData.webSocket.Send(JsonMapper.ToJson(result));
        panel.transform.Find("Button").gameObject.SetActive(false);
    }


}

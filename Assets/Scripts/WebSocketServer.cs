using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using System;
using LitJson;

public class WebSocketServer : MonoBehaviour {

    public RoomManager _roomManager;

    public GameManger _gameManger;

    public FightManager _fightManager;

	void Start () {
        string url = GameData.wUrl + GameData.p1Id + "_" + GameData.roomId;
        Init(url);
        GameData.webSocket.Open();
	}
	

    private void Init(string url)
    {
        GameData.webSocket = new WebSocket(new Uri(url));
        GameData.webSocket.OnOpen += OnOpen;
        GameData.webSocket.OnMessage += OnMessage;
        GameData.webSocket.OnError += OnError;
        GameData.webSocket.OnClosed += OnClosed;
    }

    private void AntiInit()
    {
        GameData.webSocket.OnOpen = null;
        GameData.webSocket.OnMessage = null;
        GameData.webSocket.OnError = null;
        GameData.webSocket.OnClosed = null;
        GameData.webSocket = null;
    }

    void OnOpen(WebSocket ws)
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
        {
            { "Type", "RoomInfo" }
        };
        ws.Send(JsonMapper.ToJson(result));
    }

    void OnMessage(WebSocket ws,string msg)
    {
        Debug.Log(msg);
        if(msg != null && msg != "" && msg != "{}")
        {
            JsonReader reader = new JsonReader(msg);
            JsonData jsonData = JsonMapper.ToObject(reader);

            switch ((string)jsonData["Type"])
            {
                case "房间信息":
                    _roomManager.DisplayPanel((string)jsonData["roomName"], (string)jsonData["p1Name"], (string)jsonData["p2Name"]);
                    break;
                case "准备":
                    if ((string)jsonData["Ready"] == "p1")
                    {
                        _roomManager.DisplayReady("p1", (string)jsonData["msg"]);
                    }
                    else
                    {
                        _roomManager.DisplayReady("p2", (string)jsonData["msg"]);
                    }
                    break;
                case "玩家初始信息":
                    GameFightData.p1HP = (int)jsonData["start.HP"];
                    GameFightData.p2HP = GameFightData.p1HP;
                    GameFightData.HP = GameFightData.p1HP;
                    GameFightData.p1money = (int)jsonData["start.money"];
                    GameFightData.p1efficiency = (int)jsonData["start.efficiency"];
                    _gameManger.UpdateUIMsg(1);
                    //先手回合判断
                    if ((int)jsonData["stepPlayerId"] == GameData.p1Id)
                    {
                        _gameManger.my = true;
                        _gameManger.DisplayUI(true);
                    }
                    else
                    {
                        _gameManger.my = false;
                        _gameManger.DisplayUI(false);
                    }
                    break;

                case "建造":
                    //如果是我方建的
                    //Debug.Log((int)jsonData["playerId"] + "---" + GameData.p1Id);
                    if ((int)jsonData["playerId"] == GameData.p1Id)
                    {
                        _gameManger.CreateUnit("my", (int)jsonData["posX"], (int)jsonData["posY"], (int)jsonData["unitId"]);
                        GameFightData.p1money = (int)jsonData["currentMoney"];
                    }
                    else
                    {
                        _gameManger.CreateUnit("you", (int)jsonData["posX"], (int)jsonData["posY"], (int)jsonData["unitId"]);
                    }
                    break;
                case "回合结束":
                    //如果是我方回合结束,所以不是我方回合了
                    Debug.Log("玩家"+GameData.p1Id + "回合结束");
                    if ((int)jsonData["playerId"] == GameData.p1Id)
                    {
                        _gameManger.my = false;
                        _gameManger.DisplayUI(false);
                    }
                    else
                    {
                        _gameManger.my = true;
                        _gameManger.DisplayUI(true);
                    }
                    break;
                case "战斗回合"://战斗场景发出时，使双方客户端各自操作和结算
                    //屏蔽所有UI按钮
                    _gameManger.my = false;
                    _gameManger.DisplayUI(false);
                    if ((int)jsonData["playerId"] == GameData.p1Id)
                    {
                        _fightManager.StartATK(true);
                        _fightManager.StartATK(false);
                        //战斗执行完再发送结算数据
                    }
                    else
                    {
                        _fightManager.StartATK(false);
                        _fightManager.StartATK(true);
                    }
                    _fightManager.SendGameScenesData();
                    break;
                case "同步战斗数据"://同步数据，服务端做同步后，进行一个转发广播操作
                    _gameManger.SynorizedGameScene(msg);
                    break;
                case "失败":
                    _gameManger.DisplayGameResult((int)jsonData["playerId"]);
                    break;
            }

        }

    }

    void OnError(WebSocket ws,Exception ex)
    {
        Debug.Log("网络错误！");
    }

    void OnClosed(WebSocket ws, UInt16 code, string msg)
    {
        Debug.Log("玩家离开房间！");
    }
}

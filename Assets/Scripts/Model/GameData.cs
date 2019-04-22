using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.WebSocket;

public class GameData : MonoBehaviour {

	void Start () {
        DontDestroyOnLoad(gameObject);
        GetComponent<AudioSource>().Play();
	}
    public static string url = "http://localhost:8050";
    //public static string url = "http://129.204.124.171:8050";

    public static string wUrl = "ws://localhost:8050/websocket/";
    //public static string wUrl = "ws://129.204.124.171:8050/websocket/";

    public static int p1Id;

    public static string p1Name;

    public static int roomId;

    public static WebSocket webSocket;

    //场景数据
    public static List<UnitData> unitDatas = new List<UnitData>();

    


}

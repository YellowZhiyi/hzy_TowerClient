using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnRoomData : MonoBehaviour {

    public int roomId;

    public string roomName;

    public string roomContent;

    public GameObject roomInfo;

    public void Btn()
    {
        roomInfo.transform.Find("Text").GetComponent<Text>().text = roomName;
        roomInfo.transform.Find("Text (1)").GetComponent<Text>().text = roomContent;
    }
}

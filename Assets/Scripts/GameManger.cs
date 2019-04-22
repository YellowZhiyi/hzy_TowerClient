using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.SceneManagement;
//游戏逻辑管理器
public class GameManger : MonoBehaviour {

    public GameObject UI;

    public GameObject stepmask;

    public Image imagePrefabs;

    public Slider p2HpSlider;

    public Toggle[] buildToggles;

    public GameObject buildInfo;

    public Text addMoney;

    public GameObject unit;

    public bool my;

    //选择的建筑数据
    //public UnitData currentBuildUnit;
    public int currentUnitId = 0;

    public BuildPosArray[] p1map;
    public BuildPosArray[] p2map;

    public GameObject resultPanel;
	
	void Start () {
        buildInfo.gameObject.SetActive(false);

        resultPanel.SetActive(false);

        //初始化建造菜单
        InitBuildMenu();

        //初始化科技等级按钮显示
        GameFightData.level = 1;
        OnLevelIsX(GameFightData.level);

        //初始化建造格子
        for (int i = 0; i < p1map.Length; ++i)
        {
            for(int j = 0; j < p1map[i].pos.Length; ++j)
            {
                GameObject temp = p1map[i].pos[j].gameObject;
                p1map[i].pos[j].gameObject.GetComponent<Button>().onClick.AddListener(
                    delegate ()
                    {
                        SendCreateUnit(temp.GetComponent<BuildPos>().xy.x, temp.GetComponent<BuildPos>().xy.y);
                    }
                );           
            }
        }

	}
	
	
	void Update () {
        UpdateUIMsg(GameFightData.p2HP);
        if(GameFightData.p1HP <= 0)
        {
            GameFightData.p1HP = 1;
            SendGameResult();
        }
        OnLevelIsX(GameFightData.level);
    }

    //发送建造单位的数据
    public void SendCreateUnit(int x,int y)
    {
       
        if(p1map[x].pos[y].build && currentUnitId != 0)
        {
            UnitData data = GameData.unitDatas[currentUnitId-1].Clone() as UnitData;
            if(GameFightData.p1money >= data.Cost)
            {
                //Debug.Log("GameFightData.p1money = " + data.Cost);
                addMoney.text = "-" + data.Cost;
                GameFightData.p1money -= data.Cost;
                Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
                {
                    { "Type", "Build" },
                    { "posX", x },//格子id
                    { "posY", y },//格子id
                    { "unitId", currentUnitId },//单位id
                    { "currentMoney",GameFightData.p1money}//剩余金钱(服务端校验)
                };
                GameData.webSocket.Send(JsonMapper.ToJson(result));
            }
        }
    }

    //拿到服务端返回值后执行
    //建造单位
    //@Parm p：my/you，posId：格子id
    public void CreateUnit(string p, int x,int y,int unitId)
    {
        GameFightData.p1UnitSum++;
        Image image = Instantiate(imagePrefabs);
        
        image.name = "unit_" + x + y;
        //
        UnitData data = GameData.unitDatas[unitId-1].Clone() as UnitData;
        if(p == "my")
        {
            if (data.Name == "兵营")
            {
                GameFightData.level = 2;
            }
            if (data.Name == "作战实验室")
            {
                GameFightData.level = 3;
            }
            if (data.Name == "发电厂")
            {
                GameFightData.eff = GameFightData.p1efficiency;
            }
            if (data.Type == "建筑" || data.Name == "核弹发射井")
            {
                //Debug.Log(data.Type + "+++++++++");
                BanToggleFromId(data.Id, true);
            }
        }
        image.GetComponent<Unit>().p = p; 
        image.GetComponent<Unit>().gameManger = this;
        image.GetComponent<Unit>().unitSkill = new UnitSkill(p1map,p2map);
        image.GetComponent<Unit>().unitData = data;
        image.GetComponent<Unit>().xy = new PostionXY()
        {
            x = x,
            y = y
        };
        image.transform.parent = transform;
        BuildPos temp;
        if(p == "my")
        {
            image.GetComponent<Image>().sprite = Resources.Load("my/" + unitId, typeof(Sprite)) as Sprite;
            temp = p1map[x].pos[y];
            p1map[x].pos[y].unit = image.GetComponent<Unit>();
            p1map[x].pos[y].unitImage = image;
            p1map[x].pos[y].build = false;
        }
        else
        {
            image.GetComponent<Image>().sprite = Resources.Load("you/" + unitId, typeof(Sprite)) as Sprite;
            temp = p2map[x].pos[y];
            p2map[x].pos[y].unit = image.GetComponent<Unit>();
            p2map[x].pos[y].unitImage = image;
            p2map[x].pos[y].build = false;
        }
        image.transform.position = temp.gameObject.transform.position;
        image.transform.localScale = new Vector3(1, 1, 1);
        image.gameObject.transform.parent = unit.transform;
    }

    //回合结束按钮
    public void StepBtn()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
        {
            {"Type","Step" }
        };
        GameData.webSocket.Send(JsonMapper.ToJson(result));
    }

    //初始化建造菜单
    //初始化单位数据时调用
    public void InitBuildMenu()
    {
        for(int i = 0; i < GameData.unitDatas.Count; i++)
        {
            UnitData data = GameData.unitDatas[i];
            string msg = data.Name + "\n" + data.Cost + "资源";
            buildToggles[i].transform.Find("Label").GetComponent<Text>().text = msg;
            Vector3 vector3 = new Vector3()
            {
                x = buildToggles[i].transform.position.x,
                y = buildToggles[i].transform.position.y,
                z = buildToggles[i].transform.position.z
            };
            buildToggles[i].GetComponent<Toggle>().onValueChanged.AddListener(
                delegate (bool isOn)
                {
                    ChoiceBuildUnitToggle(data.Id, isOn, vector3);
                }
            );
            buildToggles[i].GetComponent<ToggleUnit>().level = data.Level;
        }
    }

    //选择建造单位选项
    private void ChoiceBuildUnitToggle(int unitId, bool isOn, Vector3 vector)
    {
        if (isOn)
        {
            UnitData data = GameData.unitDatas[unitId-1].Clone() as UnitData;
            currentUnitId = unitId;
            BuildUnitInfo(data, unitId-1);
        }
    }
    //显示建造详细信息
    private void BuildUnitInfo(UnitData unitData, int id)
    {
        string info = unitData.Info + "\n"
                    + "生命：" + unitData.HP + "\n"
                    + "攻击：" + unitData.Atteck + "\n"
                    + "类型：" + unitData.Type + "\n"
                    + "科技：" + unitData.Level;
        buildInfo.transform.Find("info").transform.Find("Text").GetComponent<Text>().text = info;
        //Debug.Log(vector + "+++++++");
        Vector3 vector = buildToggles[id].transform.position;
        buildInfo.transform.position = buildToggles[id].transform.position;
        buildInfo.gameObject.SetActive(true);
    }

    public void BuildInfoBtn()
    {
        buildInfo.gameObject.SetActive(false);
    }

    //控制建造菜单显示
    public void DisplayBuildMenu(bool my)
    {
        for(int i = 0; i < buildToggles.Length; i++)
        {
            if (my)
            {
                addMoney.text = "";
                buildToggles[i].gameObject.SetActive(true);
            }
            else
            {
                currentUnitId = 0;
                buildToggles[i].gameObject.SetActive(false);
            }
        }
    }
    //控制UI显示（回合切换）
    public void DisplayUI(bool my)
    {
        //隐藏遮罩
        stepmask.gameObject.SetActive(!my);
        //显示回合结束按钮
        UI.transform.Find("step").gameObject.SetActive(my);
        //显示建造菜单
        DisplayBuildMenu(my);
       
    }
   

    //更新UI信息，每次操作后调用
    //@Parm p2玩家剩余百分比
    public void UpdateUIMsg(float p2HPvalue)
    {
        p2HpSlider.value = p2HPvalue*1.0f / GameFightData.HP;
        UI.transform.Find("money").GetComponent<Text>().text = "资源："+ GameFightData.p1money;
        UI.transform.Find("HP").GetComponent<Text>().text = "生命：" + GameFightData.p1HP;
        UI.transform.Find("eff").GetComponent<Text>().text = "资源获取效率：" + GameFightData.eff;
    }

    //同步场景数据
    public void SynorizedGameScene(string msg)
    {
        addMoney.text = "";
        JsonReader reader = new JsonReader(msg);
        JsonData json = JsonMapper.ToObject(reader);
        JsonData jsonData = json["data"];
        Debug.Log("playerId = " + GameData.p1Id + "\n jsonPlayerId = " + (int)json["playerId"]);
        //同步双方生命值
        GameFightData.p1HP = (int)json["playerId"] == GameData.p1Id ? (int)jsonData["p1HP"]: (int)jsonData["p2HP"];
        GameFightData.p2HP = (int)json["playerId"] == GameData.p1Id ? (int)jsonData["p2HP"] : (int)jsonData["p1HP"];
        UpdateUIMsg(GameFightData.p2HP);

        //更新战斗数据
        GameFightData.stepSum++;
        addMoney.text = "+" + GameFightData.eff;
        GameFightData.p1money += GameFightData.eff;

        JsonData jsonUnit = jsonData["data"];
        // 更新场景数据
        JsonData p1Unit = (int)json["playerId"] == GameData.p1Id ? jsonUnit["p1"] : jsonUnit["p2"];
        JsonData p2Unit = (int)json["playerId"] == GameData.p1Id ? jsonUnit["p2"] : jsonUnit["p1"];

        

        //发送战斗回合结束的信息
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
        {
            {"Type","FightStep" }
        };
        GameData.webSocket.Send(JsonMapper.ToJson(result));
    }
    
    //重建
    private void ReBuild(int x,int y,int unitId,string p)
    {
        if(p == "my")
        {
            //先删除
            p1map[x].pos[y].unit.Destroy();
        }
        else
        {
            p2map[x].pos[y].unit.Destroy();
        }
        CreateUnit(p, x, y, unitId);
    }

    //只有失败的人才发消息
    private void SendGameResult()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>
        {
            {"Type","Defeated" }
        };
        GameData.webSocket.Send(JsonMapper.ToJson(result));
    }

    //显示结算信息
    public void DisplayGameResult(int id)
    {
        resultPanel.SetActive(true);

        string resultMsg = id == GameData.p1Id ? "您战败了！！！" : "您获胜了！！！";

        resultPanel.transform.Find("title").GetComponent<Text>().text = resultMsg;
        resultPanel.transform.Find("Text").GetComponent<Text>().text = "生产单位数："+GameFightData.p1UnitSum;
        resultPanel.transform.Find("Text1").GetComponent<Text>().text = "消灭单位数："+GameFightData.p2UnitSum;
        resultPanel.transform.Find("Text2").GetComponent<Text>().text = "战斗回合数："+GameFightData.stepSum;
        resultPanel.transform.Find("Text3").GetComponent<Text>().text = "战斗时长："+GameFightData.timeSum / 60 + "分钟";
    }

    //返回大厅按钮
    public void ReturnHall()
    {
        GameData.webSocket.Close();
        
        SceneManager.LoadScene("Hall");
    }


    //显示各级科技按钮
    public void OnLevelIsX(int level)
    {
        for (int i = 0; i < buildToggles.Length; i++)
        {
            ToggleUnit toggleUnit = buildToggles[i].GetComponent<ToggleUnit>();
            if (toggleUnit.level > level)
            {
                buildToggles[i].GetComponent<Toggle>().interactable = false;
            }
            else
            {
                if(!toggleUnit.has)
                    buildToggles[i].GetComponent<Toggle>().interactable = true;
            }
        }
    }
    //禁掉/开放某些按钮（建筑物和核弹只能建一个）
    //ban true禁止 false开放
    public void BanToggleFromId(int unitId, bool ban)
    {
        buildToggles[unitId - 1].GetComponent<Toggle>().interactable = !ban;
        buildToggles[unitId - 1].GetComponent<ToggleUnit>().has = ban;
        //Debug.Log(buildToggles[unitId - 1].GetComponent<ToggleUnit>().has + "+++++++++");
    }


}


[System.Serializable]
public class BuildPosArray
{
    public BuildPos[] pos;

}

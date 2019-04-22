using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LitJson;
//战斗管理器
//双方回合结束时启动
public class FightManager : MonoBehaviour {

    public GameManger _gameManger;

    private BuildPosArray[] p1map;
    private BuildPosArray[] p2map;

    public float timer = 0;
    public float timer2 = 0;
    public float sleeprate = 1.5f;
    private bool step = false;
    private bool my = false;

    public int synStep = 0;//是否同步数据


    private void Start()
    {
        p1map = _gameManger.p1map;
        p2map = _gameManger.p2map;
    }

    private void Update()
    {
       
    }

    //开始攻击
    //各在自己的客户端执行，同步执行结果
    public void StartATK(bool my)
    {
        //my = true p1打p2 = false 反过来
        BuildPosArray[] temp;
        if (my)
        {
            temp = p1map;
        }
        else
        {
            temp = p2map;
        }
        for(int i = 0; i < temp.Length; i++)
        {
            for(int j = 0; j < temp[i].pos.Length; j++)
            {
               if (!temp[i].pos[j].build)
                {
                   if(temp[i].pos[j].unit.unitData.Type != "建筑")
                   {
                        Debug.Log(temp[i].pos[j].gameObject.name + "\n攻击");
                        temp[i].pos[j].unit.ATK(my, i);
                    }
               }
            }
        }
       
    }

    //同步场景数据
    public void SendGameScenesData()
    {
        Debug.Log("同步数据中...");
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result.Add("Type", "Fight");
        //同步玩家生命值
        result.Add("p1HP", GameFightData.p1HP);
        result.Add("p2HP", GameFightData.p2HP);
        //同步战斗数据
        //result.Add("p1Money")

        //同步地图场景数据
        GameSceneData gameSceneData = new GameSceneData();
        for(int i = 0; i < p1map.Length; i++)
        {
            for(int j = 0; j < p1map[i].pos.Length; j++)
            {
                GameUnitData gameUnitData = new GameUnitData()
                {
                    unitId = !p1map[i].pos[j].build ? p1map[i].pos[j].unit.unitData.Id : 0,//id==0代表没有单位
                    HP = !p1map[i].pos[j].build ? p1map[i].pos[j].unit.unitData.HP : 0
                };
                gameSceneData.p1[i][j] = gameUnitData;
            }
        }
        for (int i = 0; i < p2map.Length; i++)
        {
            for (int j = 0; j < p2map[i].pos.Length; j++)
            {
                GameUnitData gameUnitData = new GameUnitData()
                {
                    unitId = !p2map[i].pos[j].build ? p2map[i].pos[j].unit.unitData.Id : 0,//id==0代表没有单位
                    HP = !p2map[i].pos[j].build ? p2map[i].pos[j].unit.unitData.HP : 0
                };
                gameSceneData.p2[i][j] = gameUnitData;
            }
        }
        result.Add("data", gameSceneData);
        GameData.webSocket.Send(JsonMapper.ToJson(result));
    }
    
}   

public class GameSceneData
{
    public GameUnitData[][] p1;//p1数据
    public GameUnitData[][] p2;//p2数据
    public GameSceneData()
    {
        p1 = new GameUnitData[3][];
        for(int i = 0; i < 3; i++)
        {
            p1[i] = new GameUnitData[3];
            for(int j = 0; j < 3; j++)
            {
                p1[i][j] = new GameUnitData();
            }
        }
        p2 = new GameUnitData[3][];
        for (int i = 0; i < 3; i++)
        {
            p2[i] = new GameUnitData[3];
            for (int j = 0; j < 3; j++)
            {
                p2[i][j] = new GameUnitData();
            }
        }
    }
}

public class GameUnitData
{
    public int unitId;//单位id
    public int HP;//当前血量
    public GameUnitData()
    {

    }
    public GameUnitData(GameUnitData gameUnitData)
    {
        this.unitId = gameUnitData.unitId;
        this.HP = gameUnitData.HP;
    }
}

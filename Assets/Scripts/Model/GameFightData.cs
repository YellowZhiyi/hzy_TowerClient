using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//统计战斗数据
public class GameFightData : MonoBehaviour {

    //战斗数据（每次进入房间时都需要清零）
    public static int moneySum;//只增不减，统计花费的资源数（生产的资源+获取的资源<从同步数据中拿>）
    public static int p1UnitSum;//建造的单位数
    public static int p2UnitSum;//消灭的单位数，数据每回合从同步数据中拿
    public static int stepSum;//每回合同步数据时+1
    public static float timeSum;//战斗总时长

    //玩家数据
    public static int HP;

    public static int p1HP; //玩家生命
    public static int p1money; //玩家初始资源
    public static int p1efficiency; //玩家初始效率
    public static int eff;
    public static int p2HP; //敌方玩家生命

    //场景数据
    public static int level = 1;//当前科技等级

    void Start () {
        moneySum = p1money;
        p1UnitSum = 0;
        p2UnitSum = 0;
        stepSum = 0;
        timeSum = 0;
        level = 0;
        eff = 0;
	}
	
	void Update () {
        timeSum += Time.deltaTime;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//绑定在每个单位上的类
public class Unit : MonoBehaviour {

    public Slider slider;//血条

    public UnitData unitData;//单位数据

    public PostionXY xy;//xy坐标轴

    public int startHP;//满血条

    public UnitSkill unitSkill;

    public Image image;//背景

    public GameManger gameManger;

    public GameObject anim;

    public string p;

	void Start () {
        startHP = unitData.HP; 
    }
	
	void Update () {
        slider.value = unitData.HP*1.0f / startHP;
        
    }

    public void ATK(bool my, int x)//x 即在哪一行
    {
        if (unitData.Type != "建筑")
        {
            switch (unitData.Name)
            {
                case "天启坦克":
                    unitSkill.TianQiTank(unitData.Atteck, x, my);
                    break;
                case "磁能坦克":
                    unitSkill.CiNengTank(unitData.Atteck, x, my);
                    break;
                case "V3火箭车":
                    unitSkill.V3Tank(unitData.Atteck, x, my);
                    break;
                case "哨戒炮":
                    unitSkill.ShaojieTower(unitData.Atteck, x, my);
                    break;
                case "磁暴电塔":
                    unitSkill.CiBaoTower(unitData.Atteck, x, my);
                    break;
                case "核弹发射井":
                    unitSkill.HeDanTower(unitData.Atteck);
                    DEF(unitData.HP);
                    break;
            }
        }
    }

    public void DEF(int damage)
    {
        GameObject gameObject = Instantiate(anim);
        gameObject.transform.position = transform.position;
        gameObject.transform.parent = transform.parent;

        unitData.HP -= damage;
    }

    public GameObject Create(GameObject unitPrefab)
    {
        return Instantiate(unitPrefab);
    }

    public void Destroy()
    {
        if (p == "my")
        {
            if (unitData.Name == "作战实验室") GameFightData.level = 2;
            if (unitData.Name == "兵营") GameFightData.level = 1;
            if (unitData.Name == "发电厂") GameFightData.eff = 0;
            if (unitData.Type == "建筑" || unitData.Name == "核弹发射井")
            {
                gameManger.BanToggleFromId(unitData.Id, false);
            }
        }
        else
        {
            GameFightData.p2UnitSum++;
            if(unitData.Name != "核弹发射井")
            {
                gameManger.addMoney.text = "+"+ (unitData.Cost / 2);
                GameFightData.p1money += unitData.Cost / 2;
            }
        }

        Destroy(this.gameObject);
    }

}

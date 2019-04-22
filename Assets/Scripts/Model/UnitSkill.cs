using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

//技能数据类
public class UnitSkill{

    private BuildPosArray[] p1map;
    private BuildPosArray[] p2map;

    public UnitSkill(BuildPosArray[] p1map, BuildPosArray[] p2map)
    {
        this.p1map = p1map;
        this.p2map = p2map;
    }


    //天启坦克：对攻击目标造成150%伤害，如果目标生命低于30%造成斩杀效果
    //@Parm damage：基础伤害，x：目标第几行， my：谁发起的攻击
    public void TianQiTank(int damage,int x,bool my)
    {
        BuildPosArray[] temp;
        if (my)
        {
            temp = p2map;
        }
        else
        {
            temp = p1map;
        }

        for(int i = 2;i >= 0; i--)
        {
            if (!temp[x].pos[i].build) // 不能建造说明上面有单位
            {
                
                Unit unit = temp[x].pos[i].unit;
                int tDamage = 0;
                if(unit.unitData.HP <= unit.startHP * 0.3)
                {
                    tDamage = unit.unitData.HP;
                }
                else
                {
                    tDamage = damage;
                }
                unit.DEF(tDamage);
                return;
            }
        }
        //直接攻击玩家
        if (my)
        {
            GameFightData.p2HP -= damage;
        }
        else
        {
            GameFightData.p1HP -= damage;
        }
        
    }
    //磁能坦克：对其左右单位造成75伤害
    //@Parm damage：基础伤害，posId：目标位置， p：谁发起的攻击
    public void CiNengTank(int damage,int x,bool my)
    {
        BuildPosArray[] temp;
        if (my)
        {
            temp = p2map;
        }
        else
        {
            temp = p1map;
        }

        for (int i = 2; i >= 0; i--)
        {
            if (!temp[x].pos[i].build) // 不能建造说明上面有单位
            {
                Unit unit = temp[x].pos[i].unit;

                int tDamage = damage;
                unit.DEF(tDamage);
                if (x == 0)
                {
                    if (!temp[1].pos[i].build)
                    {
                        unit = temp[1].pos[i].unit;
                        tDamage = Convert.ToInt32(damage * 0.75);
                        unit.DEF(tDamage);
                    }                  
                }
                else if(x == 1)
                {
                    if (!temp[0].pos[i].build)
                    {
                        unit = temp[0].pos[i].unit;
                        tDamage = Convert.ToInt32(damage * 0.75);
                        unit.DEF(tDamage);
                    }
                    if (!temp[2].pos[i].build)
                    {
                        unit = temp[2].pos[i].unit;
                        tDamage = Convert.ToInt32(damage * 0.75);
                        unit.DEF(tDamage);
                    }
                                  
                }
                else
                {
                    if (!temp[1].pos[i].build)
                    {
                        unit = temp[1].pos[i].unit;
                        tDamage = Convert.ToInt32(damage * 0.75);
                        unit.DEF(tDamage);
                    }
                            
                }
                return;    
            }
        }
        //直接攻击玩家
        GameFightData.p2HP -= damage;
    }

    //V3火箭车：对整行单位造成100伤害
    public void V3Tank(int damage, int x, bool my)
    {
        BuildPosArray[] temp;
        if (my)
        {
            temp = p2map;
        }
        else
        {
            temp = p1map;
        }
        bool att = false;
        for (int i = 2; i >= 0; i--)
        {
            if (!temp[x].pos[i].build)
            {
                Unit unit = temp[x].pos[i].unit;
                int tDamage = damage;
                unit.DEF(tDamage);
                att = true;
            }
        }
        if (!att)
        {
            GameFightData.p2HP -= damage;
        }  
    }
    //哨戒炮：攻击第一个单位
    public void ShaojieTower(int damage, int x, bool my)
    {
        BuildPosArray[] temp;
        if (my)
        {
            temp = p2map;
        }
        else
        {
            temp = p1map;
        }
        for (int i = 2; i >= 0; i--)
        {
            if (!temp[x].pos[i].build)
            {
                Unit unit = temp[x].pos[i].unit;
                int tDamage = damage;
                unit.DEF(tDamage);
                return;
            }
        }
        GameFightData.p2HP -= damage;
    }
    //磁暴电塔：对装甲部队造成暴击
    public void CiBaoTower(int damage,int x,bool my)
    {
        BuildPosArray[] temp;
        if (my)
        {
            temp = p2map;
        }
        else
        {
            temp = p1map;
        }
        for (int i = 2; i >= 0; i--)
        {
            if (!temp[x].pos[i].build)
            {
                Unit unit = temp[x].pos[i].unit;
                int tDamage = damage;
                if (unit.unitData.Type == "士兵")
                    unit.DEF(Convert.ToInt32(damage * 1.5));
                else
                    unit.DEF(tDamage);
                return;
            }
        }
        GameFightData.p2HP -= damage;
    }
    //核弹，直接对玩家造成攻击
    public void HeDanTower(int damage)
    {
        GameFightData.p2HP -= damage;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//每个单位的数据类
[System.Serializable]
public class UnitData {

    private int _id;

    private string _type;

    private string _name;

    private int _HP;

    private int _atteck;

    private int _cost;

    private int _level;

    private string _info;

    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public string Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public int HP
    {
        get
        {
            return _HP;
        }

        set
        {
            _HP = value;
        }
    }

    public int Atteck
    {
        get
        {
            return _atteck;
        }

        set
        {
            _atteck = value;
        }
    }

    public int Cost
    {
        get
        {
            return _cost;
        }

        set
        {
            _cost = value;
        }
    }

    public int Level
    {
        get
        {
            return _level;
        }

        set
        {
            _level = value;
        }
    }

    public string Info
    {
        get
        {
            return _info;
        }

        set
        {
            _info = value;
        }
    }

    public override string ToString()
    {
        return "id = " + Id +
               "type = " + Type +
               "name = " + Name +
               "hp = " + HP +
               "atteck = " + Atteck +
               "cost = " + Cost +
               "level = " + Level +
               "info = " + Info;
    }

    public UnitData()
    {

    }

    public UnitData(int id, string type, string name, int HP, int atteck, int cost, int level, string info)
    {
        _id = id;
        _type = type;
        _name = name;
        _HP = HP;
        _atteck = atteck;
        _cost = cost;
        _level = level;
        _info = info;
    }

    public object Clone()
    {
        UnitData unitData = new UnitData(this.Id,this.Type,this.Name,this.HP,this.Atteck,this.Cost,this.Level,this.Info);
        return unitData;
    }
}

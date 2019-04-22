using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//绑定在坐标上判断该位置是否已经有了单位
[System.Serializable]
public class BuildPos : MonoBehaviour {

    public Unit unit;//建造的物体

    public Image unitImage;

    public bool build = true;

    public PostionXY xy;

    private void Update()
    {
        if(!build)
        {
            if (unit.unitData.HP <= 0 || unit == null || unitImage == null)
            {
                unit.Destroy();
                unit = null;
                build = true;
                unitImage = null;
            }
        }
    }

    


}

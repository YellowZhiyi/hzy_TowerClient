using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class JsonUtil {

	public static string ToJson(Dictionary<string, Object> data)
    {
        JsonData jsonData = new JsonData();
        jsonData.SetJsonType(JsonType.Object);
        foreach(KeyValuePair<string,Object> kv in data)
        {
            jsonData[kv.Key] = kv.Value.ToString();
        }
        return jsonData.ToString();
    }
}

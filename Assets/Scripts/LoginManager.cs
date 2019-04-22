using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour {

    public InputField username;

    public InputField password;

    private void Start()
    {
        transform.Find("warning").gameObject.SetActive(false);
    }

    public void Btn1()
    {
        string url = GameData.url + "/player/login";
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        StartCoroutine(Login(url, form));
    }


    IEnumerator Login(string url,WWWForm form)
    {
        WWW data = new WWW(url, form);
        yield return data;
        if(data.error == null)
        {
            JsonReader reader = new JsonReader(data.text);
            JsonData jsonData = JsonMapper.ToObject(reader);
            if((string)jsonData["type"] == "ERROR")
            {
                transform.Find("warning").gameObject.SetActive(true);
            }
            else
            {
                GameData.p1Id = (int)jsonData["playerId"];
                GameData.p1Name = (string)jsonData["playerName"];
                SceneManager.LoadScene("Hall");
            } 
        }
    }

    public void Btn()
    {
        transform.Find("warning").gameObject.SetActive(false);
    }

}

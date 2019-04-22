using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour {

    public float timer = 0;
	// Use this for initialization
	void Start () {
        timer = 0;
        transform.localScale = new Vector3(100, 100, 100);
        //Debug.Log("++++++++++++++++");
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer >= 1.0f)
        {
            Destroy(this.gameObject);
        }
	}
}

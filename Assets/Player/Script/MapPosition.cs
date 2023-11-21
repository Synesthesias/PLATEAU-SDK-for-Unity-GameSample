using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    Transform Player;
    Transform OverLook;
    int HintNum;
    void Start()
    {
        Player = GameObject.Find("PlayerArmature").transform;
        OverLook = this.transform;

        // ヒントオブジェクトの真上にマップ用の青いキューブを配置する
        foreach(Transform child in GameObject.Find("Hint").transform)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = child.name + "Point";
            cube.transform.position = new Vector3(child.position.x,770,child.position.z);
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // プレイヤーの座標に合わせてカメラの位置を変更する
        OverLook.position = new Vector3(Player.position.x,0,Player.position.z);
    }
}

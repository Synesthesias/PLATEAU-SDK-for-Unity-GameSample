using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLATEAU.Samples
{
    public class Contact : MonoBehaviour
    {
        private UIManage UIManageScript;
        void Start()
        {
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if(hit.gameObject.tag == "Hint")
            {
                //UIManageスクリプトのヒント関数を発動
                UIManageScript.Hint(hit.gameObject.name);
                //アイテムを削除
                Destroy(hit.gameObject);
            }

        }
    }
}
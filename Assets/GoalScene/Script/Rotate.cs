using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLATEAU.Samples
{
    public class Rotate : MonoBehaviour
    {
        private Vector3 angle;
        private UIManage UIManage;

        void Start()
        {
            angle = this.gameObject.transform.localEulerAngles;
            UIManage = GameObject.Find("UIManager").GetComponent<UIManage>();
        }

        void Update()
        {
            if(UIManage.SceneName == "GoalCamera")
            {
                RotateCamera();
            }
            
        }
        private void RotateCamera()
        {
            angle.y += Input.GetAxis("Mouse X");

            angle.x -= Input.GetAxis("Mouse Y");

            this.gameObject.transform.localEulerAngles = angle;
        }
    }
}
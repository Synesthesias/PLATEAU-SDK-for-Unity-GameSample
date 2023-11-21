// 正解データがある大元
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

namespace PLATEAU.Samples
{
    public class GameManage : MonoBehaviour,InputGame.IGameInputActions
    {
        private UIManage UIManageScript;
        public SampleAttribute correctGMLdata;
        private bool isInitialiseFinish = false;
        private GameObject goalBuilding;
        private Bounds goalBounds;
        private Vector3 goalPos;
        private System.Random rnd;
        private InputGame inputActions;
        private GameObject[] HintLst;
        private bool isSetGMLdata;
        KeyValuePair<string, PLATEAU.Samples.SampleCityObject> rndBuilding;
        void Awake()
        {
            inputActions = new InputGame();
        }
        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void OnDestroy()
        {
            inputActions.Dispose();
        }

        void Start()
        {
            inputActions.GameInput.AddCallbacks(this);
            rnd = new System.Random();
            //SceneManagerからShow.csにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            //Hintのリストを作る
            HintLst = GameObject.FindGameObjectsWithTag("Hint");
            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            StartCoroutine(WatiForInitialise());

            //20秒後にCalDistanceを実行
            // Invoke(nameof(CalDistance), 20f);
        }

        /// <summary>
        /// ランダムな位置にゴールを設置する
        /// </summary>
        private void SelectGoal()
        {
            while(!isSetGMLdata)
            {
                //ランダムに建物を指定
                rndBuilding = UIManageScript.gmls["53394525_bldg_6697_1_op.gml"].CityObjects.ElementAt(rnd.Next(0, UIManageScript.gmls["53394525_bldg_6697_1_op.gml"].CityObjects.Count));
                //ゴールのGMLデータ
                correctGMLdata = rndBuilding.Value.Attribute;
                isSetGMLdata = CheckGMLdata(correctGMLdata);
            }

            //正解の建物の情報を取得
            goalBuilding = GameObject.Find(rndBuilding.Key);
            goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            //選ばれた建物の位置情報を取得
            goalPos = new Vector3(goalBounds.center.x,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z);
            
            //ゴールカメラの位置を変更
            GameObject.Find("GoalSceneCamera").transform.position = goalPos;
        }


        /// <summary>
        /// 必要なGMLデータがそろっているか判定する
        /// </summary>
        private bool CheckGMLdata(SampleAttribute buildingData)
        {
            bool isSetData = false;
            
            foreach(GameObject hint in HintLst)
            {
                isSetData = false;
                foreach(var t in buildingData.GetKeyValues())
                {
                    if(t.Key.Path.Contains(hint.name))
                    {
                        isSetData = true;
                        break;
                    }
                }
                if(!isSetData)
                {
                    break;
                }
            }
            return isSetData;
        }

        /// <summary>
        /// ゴールとプレイヤーの距離を計測する
        /// </summary>
        private void CalDistance()
        {
            float userPosX = GameObject.Find("PlayerArmature").transform.position.x;
            float userPosZ = GameObject.Find("PlayerArmature").transform.position.z;
            float distance = Vector2.Distance(new Vector2(userPosX,userPosZ),new Vector2(goalPos.x,goalPos.z));

            Debug.Log("ゴールとの距離 : " + distance);
        }


        /// <summary>
        /// Plateauのデータの取得が終わった後の処理
        /// </summary> 
        IEnumerator WatiForInitialise()
        {
            // yield return ->　ある関数が終わるまで待つ
            yield return new WaitUntil(() => IsInitialiseFinished());
        }
        private bool IsInitialiseFinished()
        {
            if(UIManageScript.isInitialiseFinish)
            {
                SelectGoal();
                isInitialiseFinish = true;
            }
            return isInitialiseFinish;
        }


        /// <summary>
        /// 入力に対する処理(OnFinish : Keyboard F)
        /// </summary> 
        public void OnFinish(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                CalDistance();
            }
        }
    }
}

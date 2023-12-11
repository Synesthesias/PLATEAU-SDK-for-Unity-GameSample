// 正解データがある大元
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

namespace PLATEAU.Samples
{
    public class GameManage : MonoBehaviour, InputGameManage.IInputGameActions
    {
        [SerializeField, Tooltip("高さアイテム")] private GameObject measuredheightItem;
        [SerializeField, Tooltip("用途アイテム")] private GameObject UsageItem;
        [SerializeField, Tooltip("ターゲットフラッグ")] private GameObject targetFlag;
        [SerializeField, Tooltip("ゾンビ")] private GameObject Zombie;
        private InputGameManage inputActions;
        private UIManage UIManageScript;
        private TimeManage TimeManageScript;
        public SampleAttribute correctGMLdata;
        private GameObject goalBuilding;
        private Bounds goalBounds;
        private Vector3 goalPos;
        private System.Random rnd;
        private GameObject[] HintLst;

        public float sonarCount;
        public float distance;

        private int zombieNum;
        private bool isSetGMLdata;
        private int goalNum;
        KeyValuePair<string, PLATEAU.Samples.SampleCityObject> rndBuilding;
        private List<string> buildingDirName; 

        public struct GoalInfo
        {
            public Vector3 goalPosition;
            public string measuredheight;
            public string Usage;
            public string saboveground;
        }


        public Dictionary<string,GoalInfo> GoalAttributeDict;
        private void Awake()
        {
            inputActions = new InputGameManage();
        }

        // InputSystemに関する関数
        // -------------------------------------------------------------------------------------------------------------
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
            rnd = new System.Random();
            inputActions.InputGame.AddCallbacks(this);
            //SceneManagerからShow.csにアクセスする
            UIManageScript = GameObject.Find("UIManager").GetComponent<UIManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
            //Hintのリストを作る
            HintLst = GameObject.FindGameObjectsWithTag("HintText");
            buildingDirName = new List<string>();
            GoalAttributeDict = new Dictionary<string,GoalInfo>();

            goalNum = 3;
            sonarCount = 5;
            zombieNum = 50;

            for(int i=0; i < zombieNum;i++)
            {
                GenerateZombie();
            }
        }

        private string GetAttribute(string attributeName,SampleAttribute attribeteData)
        {
            string value = "";
            foreach(var attribute in attribeteData.GetKeyValues())
            {
                if(attribute.Key.Path.Contains(attributeName))
                {
                    value = attribute.Value;
                }
            }
            return value;
        }
        /// <summary>
        /// 正解の建物として必要な要件は満たしているか
        /// </summary>
        private bool CheckGMLdata(SampleAttribute buildingData,string buildingName)
        {
            bool isSetData = false;
            bool isOverbaseHeight = false;
            bool isOversaboveground = false;
            bool isSameBuilding = false;
            string hintValue;
            string buildingHeight;
            string buildingsaboveground;
            // 必要なGMLデータがそろっているか判定
            foreach(GameObject hint in HintLst)
            {
                isSetData = false;
                hintValue = GetAttribute(hint.name,buildingData);
                if(!(hintValue == ""))
                {
                    isSetData = true;
                }
                if(!isSetData)
                {
                    break;
                }
            }
                // foreach(var t in buildingData.GetKeyValues())
                // {
                //     if(t.Key.Path.Contains(hint.name))
                //     {
                //         isSetData = true;
                //         if(hint.name == "measuredheight")
                //         {
                //             buildingHeight = t.Value;
                //         }
                //         break;
                //     }
                // }

            // 建物の高さは10m以上か
            buildingHeight = GetAttribute("measuredheight",buildingData);
            if(buildingHeight == "")
            {
                buildingHeight = "-1";
            }
            if(float.Parse(buildingHeight) > 10)
            {
                isOverbaseHeight = true;
            }

            // 建物の階層は1以上か
            buildingsaboveground = GetAttribute("saboveground",buildingData);
            if(buildingsaboveground == "")
            {
                buildingsaboveground = "-1";
            }
            if(float.Parse(buildingsaboveground) > 0)
            {
                isOversaboveground = true;
            }

            // 同じ名前の建物でないか
            if(GoalAttributeDict.ContainsKey(buildingName))
            {
                isSameBuilding = true;
            }

            return isSetData && isOverbaseHeight && !isSameBuilding && isOversaboveground;
        }


        /// <summary>
        /// ランダムな位置に1個ゴールを設置する
        /// </summary>
        private void SelectGoal()
        {
            isSetGMLdata = false;
            while(!isSetGMLdata)
            {
                var tmpdirName = buildingDirName[Random.Range(0,buildingDirName.Count)];
                //ランダムに建物を指定
                rndBuilding = UIManageScript.gmls[tmpdirName].CityObjects.ElementAt(rnd.Next(0, UIManageScript.gmls[tmpdirName].CityObjects.Count));
                //ゴールの属性情報
                correctGMLdata = rndBuilding.Value.Attribute;
                isSetGMLdata = CheckGMLdata(correctGMLdata,rndBuilding.Key);
            }
            goalBuilding = GameObject.Find(rndBuilding.Key);
            goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);

            GoalInfo gmlData = new GoalInfo { goalPosition = goalPos, measuredheight = GetAttribute("measuredheight",correctGMLdata), Usage = GetAttribute("Usage",correctGMLdata), saboveground = GetAttribute("saboveground",correctGMLdata)};
            GoalAttributeDict.Add(rndBuilding.Key,gmlData);

            GenerateTargetFlag(goalPos);
        }

        /// <summary>
        /// ランダムな位置に複数個ゴールを設置する
        /// </summary>
        public void SelectGoals()
        {
            //citygmlからbldgに関するフォルダ情報を得る
            foreach(KeyValuePair<string, SampleGml> dir in UIManageScript.gmls)
            {
                if(dir.Key.Contains("bldg"))
                {
                    buildingDirName.Add(dir.Key);
                }
            }
            
            for(int i=0;i<goalNum;i++)
            {
                SelectGoal();
            }
            // foreach(var i in GoalAttributeDict)
            // {
            //     Debug.Log(i.Key);
            // }

            //正解の建物の情報を取得
            // goalBuilding = GameObject.Find(rndBuilding.Key);
            // goalBounds = goalBuilding.GetComponent<MeshCollider>().sharedMesh.bounds;
            //選ばれた建物の位置情報を取得
            // goalPos = new Vector3(goalBounds.center.x+320f,goalBounds.center.y+goalBounds.size.y,goalBounds.center.z+380f);
            
            //Helperの位置を変更
            //★デバッグ終了後元に戻す
            // GameObject.Find("Helper").transform.position = goalPos;
        }

//  -------------------------------------------------------------------------------

        /// <summary>
        /// ゴールとプレイヤーの距離を計測する
        /// </summary>

        private string FindNearestGoal()
        {
            string nearestBuildingName = "";
            float nearestDistance = float.MaxValue;
            Vector3 playerPos = GameObject.Find("PlayerArmature").transform.position;

            
            foreach(var goalAttribute in GoalAttributeDict)
            {
                distance = Vector2.Distance(new Vector2(goalAttribute.Value.goalPosition.x,goalAttribute.Value.goalPosition.z),new Vector2(playerPos.x,playerPos.z));
                if(distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBuildingName = goalAttribute.Key;
                }
            }

            return nearestBuildingName;
        }

        /// <summary>
        /// アイテムを拾った時の処理
        /// </summary>
        public void DisplayHint(string itemName)
        {
            string nearestBuildingName;
            string hint;

            //表示させる建物の情報を決める
            nearestBuildingName = FindNearestGoal();
            if(itemName == "measuredheight")
            {
                hint = GoalAttributeDict[nearestBuildingName].measuredheight;
            }
            else if(itemName == "Usage")
            {
                hint = GoalAttributeDict[nearestBuildingName].Usage;
            }
            else
            {
                hint = GoalAttributeDict[nearestBuildingName].saboveground;
            }
            UIManageScript.DisplayAnswerGML(itemName,hint,nearestBuildingName);

            //フィルター関連の表示
            TimeManageScript.ColorBuilding(itemName,hint);
        }

        // 生成する処理
        // -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを生成する
        /// </summary>
        public void GenerateHintItem()
        {
            //★GameViewの子として生成
            GameObject hintItem = Instantiate(measuredheightItem, transform.root.gameObject.transform) as GameObject;
            hintItem.name = "measuredheight";
            float itemPosX = Random.Range(0f,550f);
            float itemPosZ= Random.Range(0,700f);
            hintItem.transform.position = new Vector3(itemPosX,300,itemPosZ);

            hintItem = Instantiate(UsageItem, transform.root.gameObject.transform) as GameObject;
            hintItem.name = "Usage";
            itemPosX = Random.Range(0f,550f);
            itemPosZ= Random.Range(0f,700f);
            hintItem.transform.position = new Vector3(itemPosX,300,itemPosZ);
        }

        public void GenerateZombie()
        {
            GameObject zombie = Instantiate(Zombie, transform.root.gameObject.transform) as GameObject;
            zombie.name = "zombie";
            float itemPosX = Random.Range(-400f,400f);
            float itemPosZ= Random.Range(-200f,200f);
            zombie.transform.position = new Vector3(itemPosX,300,itemPosZ);
        }
        private void GenerateTargetFlag(Vector3 flagPosition)
        {
            GameObject flag = Instantiate(targetFlag,transform.root.gameObject.transform) as GameObject;
            flag.name = "targetflag";
            flag.transform.position = flagPosition;
        }

        // InputSystemの入力に対する処理(OnSonar : F)
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sonarを使う時の処理
        /// </summary> 
        public void OnSonar(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(sonarCount > 0)
                {
                    string nearestBuildingName = FindNearestGoal();
                    sonarCount -= 1;
                }   
                UIManageScript.DisplayDistance();
            }
        }
    }
}

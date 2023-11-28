using PLATEAU.CityInfo;
using PLATEAU.Util.Async;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

namespace PLATEAU.Samples
{
    public class UIManage : MonoBehaviour, InputScene.ISelectSceneActions
    {
        [SerializeField, Tooltip("初期化中UI")] private UIDocument initializingUi;
        [SerializeField, Tooltip("ヒントUI")] private UIDocument HintUi;
        [SerializeField, Tooltip("GMLデータUI")] private UIDocument GmlUi;
        [SerializeField, Tooltip("色分け（高さ）の色テーブル")] private Color[] heightColorTable;
        [SerializeField, Tooltip("色分け（使用用途）の色テーブル")] private Color[] usageColorTable;
        [SerializeField, Tooltip("選択中のオブジェクトの色")] private Color selectedColor;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera goalCamera;


        private PLATEAUInstancedCityModel[] instancedCityModels;
        private SampleCityObject selectCityObject;
        private SampleCityObject selectedCityObject;
        private InputScene inputActions;
        private ColorCodeType colorCodeType;
        private GameObject[] HintTexts;
        private GameObject[] FilterContents;
        private GameManage GameManageScript;
        private TimeManage TimeManageScript;
        private GameObject displaySelectGML;
        public readonly Dictionary<string, SampleGml> gmls = new Dictionary<string, SampleGml>();
        private string GMLText;
        public string SceneName; 
        private bool isSetColorCodeType;


        public bool isInitialiseFinish = false;
        private Label filteringLabel;
        private Label distanceLabel;
        private Label correctBuildingLabel;
        private Label selectBuildingLabel;
        private string filteringLabelText;
        private string distanceLabelText;
        private string correctBuildingLabelText;
        private string selectBuildingLabelText;



        private void Awake()
        {
            inputActions = new InputScene();

            //Plateauのデータを取得
            InitializeAsync().ContinueWithErrorCatch();  
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
        // -------------------------------------------------------------------------------------------------------------
        void Start()
        {
            inputActions.SelectScene.AddCallbacks(this);

            //コルーチン開始(Plateauのデータの取得が終わった後の処理を実行)
            StartCoroutine(WatiForInitialise());

            //初期化
            selectedCityObject = null;
            SceneName = "MainCamera";
            mainCamera.enabled = true;
            goalCamera.enabled = false;
                // GameManagerの関数や変数を参照できる
            GameManageScript = GameObject.Find("GameManager").GetComponent<GameManage>();
            TimeManageScript = GameObject.Find("TimeManager").GetComponent<TimeManage>();
                // 共通タグのオブジェクトを一つの配列にまとめる
            HintTexts = GameObject.FindGameObjectsWithTag("HintText");


                //UI
            HintUi.gameObject.SetActive(false);
            GmlUi.gameObject.SetActive(false);


        }

        // Plateauのデータに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Plateauのデータを取得する
        /// </summary> 
        private async Task InitializeAsync()
        {
            // 初期化UIを起動
            initializingUi.gameObject.SetActive(true);
            // Plateauのデータを取得
            instancedCityModels = FindObjectsOfType<PLATEAUInstancedCityModel>();
            if (instancedCityModels == null || instancedCityModels.Length == 0)
            {
                return;
            }
            foreach(var instancedCityModel in instancedCityModels)
            {
                var rootDirName = instancedCityModel.name;

                for (int i = 0; i < instancedCityModel.transform.childCount; ++i)
                {
                    var go = instancedCityModel.transform.GetChild(i).gameObject;
                    // サンプルではdemを除外します。
                    if (go.name.Contains("dem")) continue;
                    var cityModel = await PLATEAUCityGmlProxy.LoadAsync(go, rootDirName);
                    if (cityModel == null) continue;
                    var gml = new SampleGml(cityModel, go);
                    gmls.Add(go.name, gml);
                }
            }
            // 初期化UIを消去
            isInitialiseFinish = true;
        }

        // カメラに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// カメラのシーンを切り替える
        /// </summary> 
        private void ChangeCameraScene()
        {
            if(mainCamera.enabled)
            {
                SceneName = "GoalCamera";
                mainCamera.enabled = false;
                goalCamera.enabled = true;
            }
            else
            {
                SceneName = "MainCamera";
                mainCamera.enabled = true;
                goalCamera.enabled = false;
            }
        }

        // UIに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// UIを切り替える
        /// </summary>
        /// 
        private void ChangeUIDisplay()
        {
            if(HintUi.gameObject.activeSelf)
            {
                HintUi.gameObject.SetActive(false);
                GmlUi.gameObject.SetActive(true);
                correctBuildingLabel = GmlUi.rootVisualElement.Q<Label>("CorrectBuildingLabel");
                selectBuildingLabel = GmlUi.rootVisualElement.Q<Label>("SelectBuildingLabel");
                correctBuildingLabel.text = correctBuildingLabelText;
                selectBuildingLabel.text = selectBuildingLabelText;
                // correctBuildingLabel.text = "wwww";
            }
            else
            {
                GmlUi.gameObject.SetActive(false);
                HintUi.gameObject.SetActive(true);
                filteringLabel = HintUi.rootVisualElement.Q<Label>("FilterLabel");
                distanceLabel = HintUi.rootVisualElement.Q<Label>("DistanceLabel");
                filteringLabel.text = filteringLabelText;
                distanceLabel.text = distanceLabelText;
            }
        }

        // ヒントに関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイテムを取得した時の処理(Contact.csに参照されている)
        /// </summary>
        public void DisplayAnswerGML(string itemName)
        {
            //正解の建物のGMLデータを表示させる
            foreach(GameObject text in HintTexts)
            {
                // ヒントを表示させるTextオブジェクトを指定
                if(text.name.Contains(itemName))
                {
                    foreach(var t in GameManageScript.correctGMLdata.GetKeyValues())
                    {
                        // 表示させるGMLデータを指定
                        if(t.Key.Path.Contains(itemName))
                        {
                            if(!correctBuildingLabelText.Contains(itemName))
                            {
                                correctBuildingLabelText += itemName + "\n" + t.Value + "\n";
                                correctBuildingLabel.text = correctBuildingLabelText;
                            }
                        }
                    }    
                }
            }
        }

        private string SetFilterText(string itemName)
        {
            string filterText = "";
            if(itemName == "measuredheight")
            {
                float height = 0f;
                foreach(var t in GameManageScript.correctGMLdata.GetKeyValues())
                {
                    if(t.Key.Path.Contains(itemName))
                    {
                        height = float.Parse(t.Value);
                    }
                }    
                if (height <= 12f)
                {
                    filterText = "~ 12m";
                }
                else if (height > 12f && height <= 31f)
                {
                    filterText = "13m ~ 31m";
                }
                else if (height > 31f && height <= 60f)
                {
                    filterText = "32m ~ 60m";
                }
                else if (height > 60f && height <= 120f)
                {
                    filterText = "61m ~ 120m";
                }
                else if (height > 120f && height <= 180f)
                {
                    filterText = "121m ~ 180m";
                }
                else if (height > 180f)
                {
                    filterText = "181m ~";
                }
            }
            if(itemName == "Usage")
            {
                foreach(var t in GameManageScript.correctGMLdata.GetKeyValues())
                {
                    if(t.Key.Path.Contains(itemName))
                    {
                        filterText = t.Value;
                    }
                }
            }
            return filterText;
        }


        /// <summary>
        /// 機能しているフィルターを表示させる 
        /// </summary> 
        public void ChangeColoring(string itemName)
        {
            // TextMeshProUGUI  displayMesh = GameObject.Find("FilterLabel").GetComponent<TextMeshProUGUI>();

            // 全ての建物の色を元に戻す
            colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), "None");
            ColorCode(colorCodeType);
            //フィルターに引っかかった建物の色を変える
            colorCodeType = (ColorCodeType)Enum.Parse(typeof(ColorCodeType), itemName);
            ColorCode(colorCodeType);


            string filterText = SetFilterText(itemName); 
            // ownHintLstの中のIndexで指定された候補を返す
            if(itemName == "None")
            {
                filteringLabelText = $"フィルターなし";
            }
            else
            {
                filteringLabelText = itemName + "\n" + filterText;
            }
            filteringLabel.text = filteringLabelText;
        }


        // 建物の選択に関する関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// すべての建物の色を変える
        /// </summary>
        private void ColorCode(ColorCodeType type)
        {
            //GISSample.csのColorCode関数(上の方)を参考
            foreach (var keyValue in gmls)
            {
                Color[] colorTable = null;
                switch (type)
                {
                    case ColorCodeType.measuredheight:
                        colorTable = heightColorTable;
                        break;
                    case ColorCodeType.Usage:
                        colorTable = usageColorTable;
                        break;
                    default:
                        break;
                }
                keyValue.Value.ColorCode(type, colorTable);
            }
        }

        /// <summary>
        /// 目の前にある建物を返す
        /// </summary>
        private Transform Lookforward()
        {
            // 初期化
            float nearestDistance = float.MaxValue;
            Transform nearestTransform = null;
                //カメラの方向を取得
            var camera = Camera.main;
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // 方向(ray)と交わったオブジェクトの中から一番PlayerArmatureと距離の近いオブジェクトを返す
            foreach (var hit in Physics.RaycastAll(ray))
            {
                if (hit.distance <= nearestDistance && hit.transform.name != "PlayerArmature" && !hit.transform.name.Contains("dem"))
                {
                    nearestDistance = hit.distance;
                    nearestTransform = hit.transform;
                }
            }
            return nearestTransform;
        }

        /// <summary>
        /// gmlsから目的の建物の属性情報を選ぶ(属性情報 == Attribute)
        /// </summary>
        public SampleAttribute GetAttribute(string gmlFileName, string cityObjectID)
        {
            // gmls(gmlFileName, gml)  + gmlFileName  ->  gml
            if (gmls.TryGetValue(gmlFileName, out SampleGml gml))
            {
                // gml(ID,cityObject) + ID  -> cityObject
                if (gml.CityObjects.TryGetValue(cityObjectID, out SampleCityObject city))
                {
                    // cityobject -> attribute
                    return city.Attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// 建物を選択した時の処理
        /// </summary>
        private void SelectCityObject()
        {
            // 目の前の建物のTransform情報を得る(GameObject -> Transform)
            var trans = Lookforward();
            if(trans == null || trans.parent.parent == null)
            {
                selectedCityObject = null;
                return;
            }

            // 建物の色を変える
            selectCityObject = gmls[trans.parent.parent.name].CityObjects[trans.name];
                //選択された状態の建物の色を元に戻す
            if(selectedCityObject != null)
            {
                selectedCityObject.SetMaterialColor(Color.white);
            }
            // 選択した建物の色を変更する
            if(selectCityObject != selectedCityObject)
            {
                selectCityObject.SetMaterialColor(selectedColor);
                selectedCityObject =gmls[trans.parent.parent.name].CityObjects[trans.name];
            }

            //対象の建物のGMLデータを表示させる
            GMLText = "";
            var selectbuildingAttribute = GetAttribute(trans.parent.parent.name, trans.name);
            var AttributeKeyValues = selectbuildingAttribute.GetKeyValues();

            foreach(var AttributeKeyValue in AttributeKeyValues)
            {
                foreach(GameObject HintContent in HintTexts)
                {
                    if(AttributeKeyValue.Key.Path.Contains(HintContent.name))
                    {
                        GMLText += HintContent.name + "\n" + AttributeKeyValue.Value + "\n";
                    }
                }
            }
            selectBuildingLabelText = GMLText;
            selectBuildingLabel.text = selectBuildingLabelText;
        }

        // 実行時間に依存する処理
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ゴールまでの距離を表示させる
        /// </summary> 
        public void DisplayDistance()
        {
            distanceLabelText = "ソナー残数 : " + GameManageScript.sonarCount + "\n" + "距離 : " + GameManageScript.distance.ToString();
            distanceLabel.text = distanceLabelText;
        }

        
        // Plateauのデータ取得の終了待ちの関数
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ある関数が終わるまで待つ
        /// </summary> 
        IEnumerator WatiForInitialise()
        {
            // yield return ->　ある関数が終わるまで待つ
            yield return new WaitUntil(() => IsInitialiseFinished());
        }
        /// <summary>
        /// Plateauのデータの取得が終わった後の処理
        /// </summary> 
        private bool IsInitialiseFinished()
        {
            if(isInitialiseFinish)
            {
                initializingUi.gameObject.SetActive(false);
                GmlUi.gameObject.SetActive(true);
                correctBuildingLabel = GmlUi.rootVisualElement.Q<Label>("CorrectBuildingLabel");
                selectBuildingLabel = GmlUi.rootVisualElement.Q<Label>("SelectBuildingLabel");
                correctBuildingLabelText = "";
                distanceLabelText = "ソナー残数 : 5";
            }
            return isInitialiseFinish;
        }


        // InputSystemの入力に対する処理(OnChangeCameraScene : Keyboard Q, OnChangeFilterScene : Keyboard C, OnSelectObject : Mouse Left)
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// カメラの切り替えの要求処理
        /// </summary> 
        public void OnChangeCameraScene(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                ChangeCameraScene();
            }

        }
        /// <summary>
        /// フィルターの切り替えの要求処理
        /// </summary> 
        public void OnChangeFilterScene(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                // if(ownHintLst.Count == filterIndex + 1)
                // {
                //     filterIndex = 0;
                // }
                // else
                // {
                //     filterIndex += 1;
                // }

                // ChangeFilter_Hints();
                ChangeUIDisplay();
            }
        }
        /// <summary>
        /// カメラの切り替えの要求処理
        /// </summary> 
        public void OnSelectObject(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(isInitialiseFinish)
                {
                    SelectCityObject();
                }
            }
        }
    }
}
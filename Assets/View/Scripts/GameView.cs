﻿using PLATEAU.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;


public class GameView : ViewBase
{
    [SerializeField] private GameManage gameManage;
    [SerializeField, Tooltip("ゲームオーバーUI")] private UIDocument gameOverUI;
    [SerializeField, Tooltip("ゲームフィニッシュUI")] private UIDocument gameFinishUI;
    private bool IsClicked; //ボタンが押されたか
    private Button toTitleButton;
    public bool isGameFinish = false;  //ゲームクリアフラグ
    public bool isGameOver = false;   //ゲームオーバーフラグ

    private GameObject cursor;

    private CursorManager cursorManage;
    // サウンドエフェクト
    [SerializeField] private AudioClip gameFinishAudioClip;
    private AudioSource gameFinishSound;
    [SerializeField] private AudioClip gameOverAudioClip;
    private AudioSource gameOverSound;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        cursor = GameObject.Find("Cursor");
        cursorManage = cursor.GetComponent<CursorManager>();
    }

    void Start()
    {
        //ゲーム開始
        //transform.position = new Vector3(0, 0, 0);
        //スタート時はUIを非表示にする
        gameOverUI.enabled = false;
        gameFinishUI.enabled = false;
        gameManage.StartGame();
        cursorManage.OnInvisible();

        IsClicked = false;

        //サウンドエフェクト
        // アイテム取得時
        gameFinishSound = gameObject.AddComponent<AudioSource>();
        gameFinishSound.clip = gameFinishAudioClip;
        gameFinishSound.loop = false;
        // 救出時
        gameOverSound = gameObject.AddComponent<AudioSource>();
        gameOverSound.clip = gameOverAudioClip;
        gameOverSound.loop = false;
    }

    public override IEnumerator Wait()
    {
        while (true)
        {
            //ゲーム終了
            if(isGameOver||isGameFinish)
            {

                //ゾンビやアイテムを消す・プレイヤーを操作できなくする
                gameManage.OnEndGame();
                cursorManage.OnVisible();

                //ゲームオーバーテキスト
                //gameEndText.text = "Game Over";
                if (isGameFinish)
                {

                    gameFinishUI.enabled = true;
                    gameFinishSound.Play();
                    Label ResultLabel = gameFinishUI.rootVisualElement.Q<Label>("Result");
                    ResultLabel.text = "救出した人数 : " + gameManage.rescuedNum;
                    toTitleButton = gameFinishUI.rootVisualElement.Query<Button>();
                    //クリアテキスト
                    //gameEndText.text = "Game Clear!";

                    //スコア表示★スコア取得用の関数を作成する
                    //scoreText.text = "SCORE　"+ViewManager.instance.score;
                }
                else
                {
                    //ゲームオーバーUIを表示
                    gameOverUI.enabled = true;
                    gameOverSound.Play();
                    Label ResultLabel = gameOverUI.rootVisualElement.Q<Label>("Result");
                    ResultLabel.text = "救出した人数 : " + gameManage.rescuedNum;
                    toTitleButton = gameOverUI.rootVisualElement.Query<Button>();
                }
                toTitleButton.clicked += OnButtonClicked;

                //ボタン入力待ち状態にする
                while (true)
                {
                    if (IsClicked)  //タイトルボタン
                    {
                        //ゲーム終了
                        yield break;
                    }

                    yield return null;
                }

            }
            yield return null;
        }
    }
    private void OnButtonClicked()
    {
        cursorManage.CursorSoundPlay();
        IsClicked = true;
    }

    public bool GetGameEnd()
    {
        if(isGameFinish||isGameOver)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class attckHandler : MonoBehaviour
{
    [Header("attackParameter")]
    [SerializeField] private LayerMask attackable;
    [SerializeField] private Material lineColor;
    [SerializeField] private Vector3 tipPosition;
    [SerializeField] private float distance = 50f;
    private bool isAttack;
    private Vector3 attackPosition;

    //敵のコントローラー取得
    private EnemyController enemyController;

    //電撃
    private LineRenderer lr;
    [SerializeField] private Texture[] ElectricTexture;
    private int ElectricAnimationStep;
    [SerializeField] private float fps = 30f;
    private float fpsCounter;
    private float electroRelease = 0.1f;

    //経過時間管理
    private float elapsedTime = 0f;

    //se
    [SerializeField] private AudioClip spark;
    private AudioSource sparkSound;

    private void Awake()
    {
        lr = this.GetComponent<LineRenderer>();
        lr.enabled = false;
        sparkSound = this.AddComponent<AudioSource>();
        sparkSound.clip = spark;
        sparkSound.loop = false;

    }

    private void Update()
    {
        if(isAttack)
        {
            elapsedTime += Time.deltaTime;
            fpsCounter += Time.deltaTime;
            if(fpsCounter >= 1/fps)
            {
                ElectricAnimationStep++;
            }
            if(elapsedTime >electroRelease)
            {
                RemoveElectro();
                elapsedTime = 0f;
            }
        }

        CheckClickLeftMouseButton();
    }

    public void CheckClickLeftMouseButton()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isAttack = true;
        }
        Attack();
    }

    public void Attack()
    {
        if (isAttack)
        {
            lr.enabled = true;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitAttack, distance, attackable))
            {

                //敵の情報取得
                Transform geometry = hitAttack.transform.Find("Geometry");
                Transform mesh = geometry.Find("Armature_Mesh");
                attackPosition = new Vector3(mesh.position.x, mesh.position.y, mesh.position.z);
                enemyController = hitAttack.collider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.SetState(EnemyController.EnemyState.hit);
                    enemyController.EnemyColorYellow(hitAttack);
                }
                else
                {
                    Transform parent = hitAttack.transform.parent;
                    enemyController = parent.GetComponent<EnemyController>();
                    enemyController.SetState(EnemyController.EnemyState.hit);
                    enemyController.EnemyColorYellow(hitAttack);
                }

            }
            else
            {

                Vector3 forwardDirectiion = Camera.main.transform.forward;
                Vector3 targetPosition = Camera.main.transform.position + forwardDirectiion * distance;
                attackPosition = targetPosition;

            }

            //se再生
            sparkSound.Play();
        }
    }

    public void DrawElectro()
    {
        lr.SetPosition(0, tipPosition);
        lr.SetPosition(1, attackPosition);
    }

    public void RemoveElectro()
    {
        isAttack = false;
        if(lr != null)
        {
            lr.enabled = false;
        }
    }
}

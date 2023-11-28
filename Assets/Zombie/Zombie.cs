using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using StarterAssets;
using PLATEAU.Samples;

public class Zombie : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    public float distance;
    public bool search;
    [SerializeField] private SphereCollider searchArea;
    // [SerializeField] float searchAngle = 180f;
    private Contact ContactScript;
    private ThirdPersonController ThirdPersonControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        ContactScript = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        ThirdPersonControllerScript = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = Vector3.Distance(this.transform.position, player.transform.position);
        if(distance < 30 && !ThirdPersonControllerScript.isDied)
        {
            search = true;
        }
        else
        {
            search = false;
        }
        if(distance < 2f)
        {
            ContactScript.GameOverFunc();
        }
        // if(distance < 5f)
        // {
        //     animator.SetTrigger("Attack");
        // }
        // else
        // {
        //     animator.ResetTrigger("Attack");
        // }

        if(search)
        {
            animator.SetBool("Run",true);
            this.transform.LookAt(new Vector3(player.transform.position.x,this.transform.position.y,player.transform.position.z));
            transform.position -= transform.forward * 0.1f;
        }
        else
        {
            animator.SetBool("Run",false);
            animator.SetBool("Walk",true);
            // this.transform.position += transform.forward * 0.03f;
        }
        // Debug.Log(search);
    }
    // プレイヤーが範囲内にいるか
    // private void OnTriggerExit(Collider other)
    // {
    //     if(other.tag == "Player")
    //     {
    //         search = false;
    //     }
    // }
    // private void OnDrawGizmos()
    // {
    //     Handles.color = Color.red;
    //     Handles.DrawSolidArc(transform.position, Vector3.up,Quaternion.Euler(0f,-searchAngle,0f)*transform.forward,searchAngle*2f,searchArea.radius);
    // }
}

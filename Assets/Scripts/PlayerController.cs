using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;  //水平输入
    public float verticalInput;         //垂直输入
    public float horizontalInput_Dir;   //水平转向
    public float verticalInput_Dir;     //垂直转向
    public GameObject leftPart;         //玩家角色左边部分
    public GameObject rightPart;        //玩家角色右边部分
    public float speed = 10.0f;         //角色移动速度
    public float shootRate;             //角色射速
    public int health = 3;              //hp
    public Vector3 movement;            //总体移动方向

    private float shootTimer = 1.0f;    //射击计时器

    public GameObject projectilePrefab;  //子弹prefab
    private Rigidbody playerRb;         //玩家角色刚体

    public GameManager gameManager;     //游戏管理器
    public CoroutineManager coroutineManager;   //协程管理器
    
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();//获取角色刚体
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();//找到游戏管理器实体中的GameManager脚本
        coroutineManager = GameObject.Find("CoroutineManager").GetComponent<CoroutineManager>();//找到协程管理器实体中的CoroutineManager脚本
    }

    private void Update()
    {
        horizontalInput_Dir = Input.GetAxis("Joystick_Dir_Horizontal");  //获取水平垂直移动
        verticalInput_Dir = Input.GetAxis("Joystick_Dir_Vertical");
        Vector3 inputDirection = new Vector3(verticalInput_Dir, 0f, -horizontalInput_Dir);
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection);  //将vector3的方向转化为四元数方向
        if (Mathf.Abs(inputDirection.x) >= 0.1 || Mathf.Abs(inputDirection.z) >= 0.1)  //设置摇杆死区，以免遥感漂移或灵敏度过高
        {
            playerRb.freezeRotation = false;
            transform.rotation = targetRotation;
        }
        playerRb.freezeRotation = true;

        //发射子弹，并控制射速
        shootTimer += Time.deltaTime;
        if ((Input.GetButton("RightButton") || Input.GetKey(KeyCode.Space)) && shootTimer >= 1f / shootRate)
        {
            Instantiate(projectilePrefab, new Vector3(transform.position.x, 1.25f, transform.position.z)+transform.forward*2, transform.rotation);
            shootTimer = 0f;
        }

        



        //按下esc退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");  //获取水平垂直移动方向
        verticalInput = Input.GetAxis("Vertical");
        


        //moves the player based on Horizontal
        movement = new Vector3(horizontalInput, 0, verticalInput);  //得到总体移动方向
        playerRb.velocity = movement * speed;   //移动矢量=方向*速度

    }

    public void UpdateHealth()  //更新玩家HP
    {
        health--;
        if (health <= 2)  //如果hp<=2，就失去左边部件
        {
            leftPart.SetActive(false); 
        }
        if (health <= 1)//HP<=1，失去右边部件
        {
            rightPart.SetActive(false);
        }
        if (health <= 0)  //hp<=0，调用协程管理器，启动协程执行游戏管理器内的重启场景方法。绕一圈主要是为了把协程放在玩家角色外执行，否则角色死亡则协程不执行，则无法切换场景。
        {
            coroutineManager.StartCoroutineInManager(gameManager.RestartScene());
            Destroy(gameObject);
        }
        Debug.Log("player health : " + health);  //控制台提示血量减少，玩家不可见
    }
}

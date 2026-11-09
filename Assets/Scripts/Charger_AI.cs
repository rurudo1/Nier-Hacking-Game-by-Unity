using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger_AI : MonoBehaviour
{
    public int health;  //hp
    public float speed;  //速度
    public float rotateSpeed;  //旋转速度
    public float fireRate;  //射速

    public GameObject prefab;  //子弹prefab
    public GameObject player; //玩家
    private Rigidbody enemyRb;  //刚体

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();  //获得刚体
        player = GameObject.Find("Player");  //找到玩家
        transform.rotation = FireDirection();  //开始时指向玩家
        StartCoroutine(FireBullet());  //开始射击
    }

    // Update is called once per frame
    void Update()
    {
        
        RotateToPlayer();  //转向玩家
        MoveForward();  //向前行走
    }

    IEnumerator FireBullet()
    {
        while (true)  //总是循环按照射速向前射击
        {
            Instantiate(prefab,new Vector3(transform.position.x,1.25f,transform.position.z), transform.rotation);
            yield return new WaitForSeconds(1 / fireRate);
        }
    }
    Quaternion FireDirection()  //获取射击角度
    {
        Vector3 fireDir = new Vector3((player.transform.position.x - transform.position.x), 0, (player.transform.position.z - transform.position.z));
        return Quaternion.LookRotation(fireDir);
    }
    void RotateToPlayer()  //转向玩家
    {
        Quaternion playerDirection = FireDirection();
        transform.rotation = Quaternion.Lerp(transform.rotation, playerDirection, Time.deltaTime * rotateSpeed);
    }
    void MoveForward()  //矢量=向前*速度*每帧时间
    {
        enemyRb.velocity = transform.forward * speed * Time.deltaTime;
    }

    public void UpdateHealth()  //更新hp，到0销毁
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

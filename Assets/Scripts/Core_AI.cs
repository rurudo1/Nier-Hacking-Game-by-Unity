using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core_AI : MonoBehaviour
{
    public enum ShootBehavior  //射击模式
    {
        BulletInLine,
        BulletInRound,
        BulletInCustomize,
        BulletInSurge
    }
    public ShootBehavior selectedShootBehavior;  
    public enum MoveBehavior  //移动模式
    {
        ChasePlayer,
        FollowPath,
        InfinitySymbol,
        Circle,
        Bouncy,
        ChargeToPlayer
    }
    public MoveBehavior selectedMoveBehavior;  
    public GameObject[] pathPoint;  //FollowPath移动模式中的路径点
    public int currentPoint = 0;  //FollowPath移动模式中的当前要前往的路径点
    public float acceleration;  //加速度
    public float currentSpeed = 0;  //目前速度
    public float resetCurrentSpeed;  //重置速度
    public float infinitySymbolSize;  //移动模式中无限形状的大小
    public float symbolRotationAngle = 0;  //无限形状移动转向角度
    public float radius;  //圆形移动半径
    private float circleAngle = 0;  //圆形移动转向角度
    public float circleAngularSpeed;  //圆形移动角速度
    public float chargeDelay;  //冲击移动方式前摇时间

    public GameObject player;  //玩家实体
    private Rigidbody EnemyRb;  //敌方刚体
    public float speed;  //速度
    public float fireRate;  //敌方射速
    public float fireRate_Round;  //圆形射击模式，射速
    public int fireNum_Round;  //圆形设计模式，一次射多少发
    public int fireNum_Surge;  //涌动射击模式，一次射多少发
    public float surgeDelay;   //涌动射击前摇
    public float randomAngle;  //随机角度
    public int health;         //敌方HP
    public int customizeBulletPair;   //自定义射击模式敌方子弹射击对数
    public float customizeIntervalAngle;  //自定义射击模式各子弹发射间隔角度
    public float customizeWaveDelay;  //自定义射击模式后摇
    public int customizeWaveNum;  //一次射击的波数
    public bool customizeWave = false;  //是否开启自定义波数
    public bool randomBullet = true;   //是否开始随机子弹
    public bool changeBullet = false;  //是否开启换弹
    public int bulletsToChange;  //打多少发换弹

    public List<GameObject> prefabs;  //子弹
    public int index = 0;  //编号0

    private int customizeWaveCurrentNum = 0;  //初始化当前自定义波数
    private int currentBulletToChange = 0;  //初始化当前换弹编号
    private Vector3 initialPos;           //初始位置
    private Vector3 direction;              //方向
    private float enemyNum;                 //敌人小兵个数
    private int currentEnemyNum;            //目前小兵个数

    public bool destructable = false;       //是否可破坏
    public ShootBehavior destructableShootBehavior;  //狂暴状态
    public float changedFireRate;    //狂暴状态射速

    public GameManager gameManager;  //游戏管理器
    public CoroutineManager coroutineManager;   //协程管理器
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");  //找到玩家实体
        EnemyRb = GetComponent<Rigidbody>();  //获取敌方刚体
        initialPos = transform.position;  //记录初始位置
        changeBullet = !randomBullet;  //如果随机子弹启动，关闭换弹
        //判断子弹模式
        FireMode();
        enemyNum = (GameObject.FindGameObjectsWithTag("Enemy").Length - GameObject.FindObjectsOfType<Core_Marker>().Length);  //记录小兵个数
        if (selectedMoveBehavior == MoveBehavior.Bouncy)  //如果移动模式为弹性移动，随机一个方向，给一个初速度
        {
            float moveAngleX= Random.Range(-10, 10);
            float moveAngleZ= Random.Range(-10, 10);
            Vector3 moveDirection = new Vector3(moveAngleX, 0, moveAngleZ).normalized;
            EnemyRb.AddForce(moveDirection * speed,ForceMode.Impulse);
        }
        else if (selectedMoveBehavior == MoveBehavior.ChargeToPlayer) //如果为冲向玩家，则启动对应协程
        {
            StartCoroutine(WaitChargeToPlayer());
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        coroutineManager = GameObject.Find("CoroutineManager").GetComponent<CoroutineManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //判断行走模式
        if (selectedMoveBehavior == MoveBehavior.FollowPath)
        {
            direction = new Vector3((pathPoint[currentPoint].transform.position.x - transform.position.x), 0, (pathPoint[currentPoint].transform.position.z - transform.position.z));   //direction没有normalize，承担了变速的责任。虽然说很不严谨，但是还是完成了功能，暂时不要动它。
            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * acceleration);
            EnemyRb.velocity = currentSpeed * direction;
            if (Vector3.Distance(transform.position, pathPoint[currentPoint].transform.position)<=0.1f)  //位置与当前路径点距离<=0.1时，切换下一路径点
            {
                currentPoint++;
                currentSpeed = resetCurrentSpeed;
                if (currentPoint == pathPoint.Length)  //如果当前路径点编号=路径点数量，则说明走完了，设定当前路径的为0，让他走回去，重新一轮
                {
                    currentPoint = 0;
                    
                }
            }
        }
        else if (selectedMoveBehavior == MoveBehavior.ChasePlayer)  //追踪玩家
        {
            direction = new Vector3((player.transform.position.x - transform.position.x), 0, (player.transform.position.z - transform.position.z)).normalized;  //获取走向玩家的方向
            EnemyRb.velocity = speed * direction;  //按照方向和速度去移动
        }
        else if (selectedMoveBehavior == MoveBehavior.InfinitySymbol)  //无限形移动
        {
            float x = infinitySymbolSize * Mathf.Sin(Time.time * speed);
            float z = infinitySymbolSize * Mathf.Sin(2 * Time.time * speed) * 0.5f;
            Vector3 rotatedPosition = Quaternion.Euler(0, symbolRotationAngle, 0) * new Vector3(x, 0, z);
            transform.position = initialPos + rotatedPosition;
        }
        else if(selectedMoveBehavior == MoveBehavior.Circle)   //圆形移动 
        {
            circleAngle += circleAngularSpeed * Time.deltaTime;
            float x = radius * Mathf.Cos(circleAngle * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(circleAngle * Mathf.Deg2Rad);
            transform.position =initialPos + new Vector3(x, 0 , z);
        }
        //是否破防
        if(GameObject.FindGameObjectsWithTag("Enemy").Length<= GameObject.FindObjectsOfType<Core_Marker>().Length)  //如果敌人的数量（核心+小兵）<=敌方核心的数量，则核心破防
        {
            destructable = true;
        }
        currentEnemyNum = (GameObject.FindGameObjectsWithTag("Enemy").Length - GameObject.FindObjectsOfType<Core_Marker>().Length);  //记录当前小兵数量
        if (currentEnemyNum <= enemyNum/3)  //当前小兵数量<=原数量/3，切换狂暴模式
        {
            selectedShootBehavior = destructableShootBehavior;
            fireRate = changedFireRate;
        }


    }
    private void FixedUpdate()
    {
        
        MaxSpeed(); //限制最大移速
    }

    IEnumerator FireBullet()  //普通射击
    {
        yield return null;
        while (selectedShootBehavior == ShootBehavior.BulletInLine)
        {
            Instantiate(prefabs[0],transform.position, FireDirection());
            yield return new WaitForSeconds(1 / fireRate);
            Instantiate(prefabs[1], transform.position,FireDirection());
            yield return new WaitForSeconds(1 / fireRate);
        }
        FireMode();
    }

    IEnumerator FireBullet_Round()  //圆形射击
    {
        yield return null;
        while (selectedShootBehavior == ShootBehavior.BulletInRound)
        {
            float angle = Random.Range(0,randomAngle);
            if (changeBullet)
            {
                ChangeBullet();
            }
            for (int b = 0; b < fireNum_Round; b++)
            {
                if (randomBullet)
                {
                    index = Random.Range(0, prefabs.Count);
                }
                Quaternion fireAngle = Quaternion.Euler(0, angle, 0);
                Instantiate(prefabs[index], transform.position, fireAngle);
                angle += 360/fireNum_Round;
            }
            yield return new WaitForSeconds(1 / fireRate_Round);
        }
        FireMode();
    }

    IEnumerator FireBullet_Customize(int bulletPair,float intervalAngle)  //自定义射击
    {
        yield return null;
        while (selectedShootBehavior == ShootBehavior.BulletInCustomize)
        {
            Vector3 angle = FireDirection().eulerAngles;
            if (changeBullet)
            {
                ChangeBullet();
            }
            for (int b = 0; b < bulletPair; b++)
            {
                if (randomBullet)
                {
                    index = Random.Range(0, prefabs.Count);
                }
                
                if (b == 0)
                {
                    Instantiate(prefabs[index], transform.position, Quaternion.Euler(0, angle.y + b * intervalAngle, 0));
                }
                else
                {
                    Instantiate(prefabs[index], transform.position, Quaternion.Euler(0, angle.y + b * intervalAngle, 0));
                    Instantiate(prefabs[index], transform.position, Quaternion.Euler(0, angle.y - b * intervalAngle, 0));
                }
                
            }
            if (customizeWave)
            {
                customizeWaveCurrentNum++;
            }
            yield return new WaitForSeconds(1 / fireRate);
            if(customizeWaveCurrentNum >= customizeWaveNum)
            {
                customizeWaveCurrentNum = 0;
                yield return new WaitForSeconds(customizeWaveDelay);
            }

        }
        FireMode();
    }
    
    IEnumerator FireBullet_Surge()  //涌动射击
    {
        yield return null;
        while (selectedShootBehavior == ShootBehavior.BulletInSurge)
        {
            for (int b = 0; b < fireNum_Surge; b++)
            {
                Instantiate(prefabs[index], transform.position, FireDirection());
                currentBulletToChange++;
                if (IfChangeBullet())
                {
                    ChangeBullet();
                }
                yield return new WaitForSeconds(1 / fireRate);
            }
            yield return new WaitForSeconds(surgeDelay);
        }
        FireMode();
    }

    Quaternion FireDirection()  //获得向玩家射击的方向
    {
        Vector3 fireDir= new Vector3((player.transform.position.x - transform.position.x), 0, (player.transform.position.z - transform.position.z));
        return Quaternion.LookRotation(fireDir);
    }

    private void OnCollisionEnter(Collision collision)  //检测碰撞箱
    {
        if(selectedMoveBehavior == MoveBehavior.ChargeToPlayer && collision.transform.CompareTag("Terrain"))  //移动模式为冲撞玩家且撞到地形，矢量归零，重新冲撞玩家
        {
            EnemyRb.velocity = new Vector3(0,0,0);
            StartCoroutine(WaitChargeToPlayer());
        }
    }

    void ChangeBullet()  //取反子弹编号以便于换弹
    {
        if (index == 0)
        {
            index = 1;
        }
        else
        {
            index = 0;
        }
    }
    bool IfChangeBullet()  //判断是否需要换弹
    {
        if (currentBulletToChange >= bulletsToChange)  //目前打了多少发>=打多少发换弹，则目前发数归零，返回真值，需要换弹，否则不需要
        {
            currentBulletToChange = 0;
            return true;
        }
        return false;
    }


    IEnumerator WaitChargeToPlayer()  //等待后冲向玩家
    {
        yield return new WaitForSeconds(chargeDelay);
        direction = new Vector3((player.transform.position.x - transform.position.x), 0, (player.transform.position.z - transform.position.z)).normalized;  //冲向玩家的角度
        EnemyRb.AddForce(direction * speed, ForceMode.Impulse);  //照着角度给一个冲击力
    }
    void MaxSpeed()  //最大速度
    {
        Vector3 currentVelocity = EnemyRb.velocity;
        float currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > speed)  //当前速度>最大速度，则将当前矢量回归到基数为1的状态，此时类似于方向，再结合速度
        {
            Vector3 newVelocity = currentVelocity.normalized * speed;
            EnemyRb.velocity = newVelocity;
        }
    }

    void FireMode()  //判断开火模式并开火
    {
        if (selectedShootBehavior == ShootBehavior.BulletInLine)
        {
            StartCoroutine(FireBullet());
        }
        else if (selectedShootBehavior == ShootBehavior.BulletInRound)
        {
            StartCoroutine(FireBullet_Round());
        }
        else if (selectedShootBehavior == ShootBehavior.BulletInCustomize)
        {
            StartCoroutine(FireBullet_Customize(customizeBulletPair, customizeIntervalAngle));
        }
        else if (selectedShootBehavior == ShootBehavior.BulletInSurge)
        {
            StartCoroutine(FireBullet_Surge());
        }
    }

    public void UpdateHealth()   //更新核心血量
    {
        health--;
        if (health <= 0)  //hp见底，通过协程管理器调用游戏管理器内的加载下一场景
        {
            coroutineManager.StartCoroutineInManager(gameManager.LoadNextScene());
            Destroy(gameObject);
        }
    }
}

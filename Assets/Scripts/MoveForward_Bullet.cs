using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward_Bullet : MonoBehaviour
{
    public float speed = 40.0f;
    public float distanceToDestroy;

    private PlayerController playerController;
    private Core_AI core_AI;
    private Charger_AI charger_AI;
    private Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //destroy the object
        float distance = Vector3.Distance(initialPosition, transform.position);
        if (distance >= distanceToDestroy)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            Destroy(gameObject);
        }

        if (gameObject.CompareTag("Player_Bullet") && other.gameObject.name == ("Enemy_Bullet_Normal(Clone)"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        else if (gameObject.CompareTag("Player_Bullet") && other.gameObject.CompareTag("Enemy"))
        {
            if(other.name==("Enemy_Core"))
            {
                core_AI = other.GetComponent<Core_AI>();
                if (core_AI.destructable)
                {
                    core_AI.UpdateHealth();
                }
            }
            else if (other.transform.parent.name == ("Shield_Charger")|| other.transform.parent.name == ("Charger"))
            {
                charger_AI = other.transform.parent.GetComponent<Charger_AI>();
                charger_AI.UpdateHealth();
            }
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Player_Bullet")&& other.gameObject.CompareTag("Enemy_Shield"))
        {
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Enemy_Bullet")&&other.gameObject.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            Destroy(gameObject);
            playerController.UpdateHealth();
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float offset;
    public float followSpeed;
    public Vector3 followDirection;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        followDirection = new Vector3 ((player.transform.position.x - transform.position.x),0, (player.transform.position.z - transform.position.z));
        transform.Translate(followDirection * followSpeed * Time.deltaTime,Space.World);
        transform.position =new Vector3 (transform.position.x, transform.position.y, player.transform.position.z + offset);
    }
}

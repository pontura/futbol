using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    Animation anim;
    public Camera cam;
    float initial_y_position;
    [SerializeField] Transform target;
    float speed = 4.5f;
    bool filmingPlayer;
    public float filming_y;
    public float offsetZ = 25;
    public float originalSize = 5;
    public float originalSizeMobile = 3.5f;
    public float zoomSize = 2;
    public float offsetShootingPlayer_z = 2.5f;
    int pos_z = 14;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        cam = GetComponent<Camera>();
    }
    private void Start()
    {
        if (Data.Instance.isMobile)
        {
            originalSize = originalSizeMobile;
            pos_z = 17;
        }
        initial_y_position = transform.position.y;
    }
    public void Restart()
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.Stop();           
            anim.Play();
        }        
    }
    public void Reset()
    {
        if (anim != null)
            anim.enabled = false;
        filmingPlayer = false;
        cam.orthographicSize = originalSize;
    }
    public void SetTargetTo(Transform t)
    {        
        target = t;
    }
    void Update()
    {
        if (target == null)
            return;
        Vector3 pos = transform.position;
        pos.x = target.position.x;
        pos.z = target.position.z - offsetZ;

        if (!filmingPlayer)
            pos.x *= 0.8f;
        else
        {
            pos.x *= 0.9f;
            pos.z -= 2;
        }


        if (pos.z < -pos_z)  pos.z = -pos_z;
        else if (pos.z > 0)   pos.z = 0;
        transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(20, target.position.x * 15 / 20, target.position.x * 5 / 20);
    }
    public void OnGoal(Character character)
    {
        StartCoroutine(GoalCoroutine(character));        
    }    
    IEnumerator GoalCoroutine(Character character)
    {
        yield return new WaitForSeconds(1.25f);
        filmingPlayer = true;
        SetTargetTo(character.transform);
        while (cam.orthographicSize > zoomSize)
        {
            yield return new WaitForEndOfFrame();
            cam.orthographicSize -= Time.deltaTime*0.5f;
        }
    }
}

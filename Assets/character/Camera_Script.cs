using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Script : MonoBehaviour
{
    private GameObject Player;
    private Camera cam;
    public float MaxPositionX;
    public float MinPositionX;
    public float MaxPositionY;
    public float MinPositionY;

    [Header("Camera Max sapce")]
    [SerializeField]
    private float X;
    [SerializeField]
    private float Y;
    [SerializeField]
    [Tooltip("In how many frames move to desired positon whole way = 1 frane, 0.5 way = 1 frame")]
    private float time = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cam.gameObject.transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(Player.transform.position.x, Player.transform.position.x + ReturnPositionX(), time * Time.deltaTime), MinPositionX, MaxPositionX), 
            Mathf.Clamp(Mathf.Lerp(Player.transform.position.y, Player.transform.position.y + ReturnPositionY(), time * Time.deltaTime), MinPositionY, MaxPositionY), cam.gameObject.transform.position.z);

    }

    private float ReturnPositionX()
    {
        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return Mathf.Clamp(MousePosition.x - cam.transform.position.x, -X , X);
    }

    private float ReturnPositionY()
    {
        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return Mathf.Clamp(MousePosition.y - cam.transform.position.y, -Y, Y);
    }
}

using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update

    private UdpClient client;
    private string m_SeverAdress = "127.0.0.1";
    private int m_ServerPort = 7777;
    [SerializeField] private float m_Speed = 3f;    
    private Transform m_OldTransform;
    void Start()
    {
        client = new UdpClient(m_SeverAdress, m_ServerPort);
        m_OldTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        // if(client.Available > 0)
        // {
        //     IPEndPoint remote = null;
        //     byte[] rbytes = client.Receive(ref remote);
        //     string received = Encoding.UTF8.GetString(rbytes);

        //     Debug.Log($"Client - Recv {received}");

        //     receivedText.text = received;
        // }
    }

    private void Move()
    {
        float xTranslation = Input.GetAxis("Horizontal");
        float yTranslation = Input.GetAxis("Vertical");
        xTranslation *= m_Speed * Time.deltaTime;
        yTranslation *= m_Speed * Time.deltaTime;

        transform.Translate(xTranslation, yTranslation, 0f);

        SendPlayerPositionToServer();
    }
    public void SendPlayerPositionToServer()
    {
        if(Vector3.Distance(m_OldTransform.position, transform.position) > 1f) 
            return;
        
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        Debug.Log(@$"Sending Player {GetInstanceID()} position
                  x:{x},
                  y:{y},
                  z:{z}");
        
        byte[] bytes = Encoding.UTF8.GetBytes($"{x} {y} {z}");
        client.Send(bytes, bytes.Length);                
    }
}

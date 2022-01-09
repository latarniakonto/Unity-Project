using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;
using MyEssentials;
using MyEssentials.Serialization;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update

    private UdpClient m_Client;
    private string m_ClientId = "DEFAULT";
    private string m_SeverAdress = "127.0.0.1";
    private int m_ServerPort = 7777;
    [SerializeField] private float m_Speed = 3f;    
    private Vector3 m_OldPosition;
    private SerializationManager m_Serialization;    
    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i] == "--name")
            {
                if(i + 1 == args.Length)
                    throw new Exception("Argument --name without value.");
                
                m_ClientId = args[i + 1];                
            }
        }
    }
    void Start()
    {
        m_Client = new UdpClient(m_SeverAdress, m_ServerPort);
        
        m_OldPosition = transform.position;
        m_Serialization = new SerializationManager();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {        
        float xTranslation = Input.GetAxis("Horizontal");
        float yTranslation = Input.GetAxis("Vertical");
        xTranslation *= m_Speed * Time.deltaTime;
        yTranslation *= m_Speed * Time.deltaTime;

        transform.Translate(xTranslation, yTranslation, 0f);

        SendPlayerPositionToServer();
        //m_OldTransform = transform; 
    }
    public void SendPlayerPositionToServer()
    {        
        if(Vector3.Distance(m_OldPosition, transform.position) < 1f) 
            return;
        m_OldPosition = transform.position; 

        string id = m_ClientId;
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Player player = new Player(id, x, y, z);

        Debug.Log(@$"Sending Player {id} position x:{x} y:{y} z:{z}");
        
        byte[] bytes = m_Serialization.SerializePlayer(player);
        m_Client.Send(bytes, bytes.Length);                
    }    
}

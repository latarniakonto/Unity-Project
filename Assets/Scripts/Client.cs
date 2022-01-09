using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update

    private UdpClient client;
    private string m_SeverAdress = "127.0.0.1";
    private int m_ServerPort = 7777;
    [SerializeField] private float m_Speed = 3f;    
    private Vector3 m_OldPosition;
    void Start()
    {
        client = new UdpClient(m_SeverAdress, m_ServerPort);
        m_OldPosition = transform.position;
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

        int id = GetInstanceID();
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Player player = new Player(id, x, y, z);

        Debug.Log(@$"Sending Player {id} position
                  x:{x},
                  y:{y},
                  z:{z}");
        
        byte[] bytes = SerializePlayer(player);
        client.Send(bytes, bytes.Length);                
    }
    private byte[] SerializePlayer(Player player)
    {
        var formatter = new BinaryFormatter();
        using(var stream = new MemoryStream())
        {        
            formatter.Serialize(stream, player);
            return stream.ToArray();
        }
    }
    private Player DeserializePlayer(byte[] bytes)
    {
        var formatter = new BinaryFormatter();
        using(var stream = new MemoryStream())
        {        
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            Player player = (Player)formatter.Deserialize(stream);
            return player;
        }
    }
}

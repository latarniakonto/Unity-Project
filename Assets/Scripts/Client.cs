using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using MyEssentials;
using MyEssentials.Serialization;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update

    private UdpClient m_Client;
    private string m_ClientId = null;
    private string m_SeverAdress = "172.20.0.2";
    private int m_ServerPort = 26000;
    [SerializeField] private float m_Speed = 3f; 
    [SerializeField] private GameObject m_PlayerInstance;
    private Vector3 m_OldPosition;
    private SerializationManager m_Serialization;  
    private Player[] m_OtherPlayers;
    private Dictionary<string, GameObject> m_OtherPlayersInstances;
    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i] == "--name")
            {
                if(i + 1 == args.Length)
                    throw new Exception("Argument --name without value.");

                if(args[i + 1] == "")
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
        m_OtherPlayers = new Player[4];
        m_OtherPlayersInstances = new Dictionary<string, GameObject>();
        m_OtherPlayersInstances[m_ClientId] = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (m_Client.Available > 0)
        {
            IPEndPoint remote = null;
            byte[] rbytes = m_Client.Receive(ref remote);
            Player[] players = m_Serialization.DeserializePlayers(rbytes);

            UpdateOtherPlayers(players);      
        }
        UpdateOtherPlayers();
    }
    public void UpdateOtherPlayers(Player[] players)
    {        
        foreach(var player in players)
        {           
            if(player.id != null && m_ClientId != player.id)
            {      
                if(!m_OtherPlayersInstances.ContainsKey(player.id))          
                {
                    Vector3 newPlayerPosition = new Vector3(player.xPosition,
                                                            player.yPosition,
                                                            player.zPosition);
                    GameObject newPlayer = Instantiate(m_PlayerInstance,
                                                       newPlayerPosition,
                                                       Quaternion.identity);
                    m_OtherPlayersInstances[player.id] = newPlayer;
                }else
                {                    
                    var newPlayerPosition = new Vector3(player.xPosition,
                                                            player.yPosition,
                                                            player.zPosition);
                    var currentPlayerPosition = m_OtherPlayersInstances[player.id].transform.position;
                    m_OtherPlayersInstances[player.id].transform.position = Vector3.Lerp(currentPlayerPosition,
                                                                                         newPlayerPosition,
                                                                                         0.3f);
                }
            }
        }
    }
    public void UpdateOtherPlayers()
    {        
        foreach(var key in m_OtherPlayersInstances.Keys)
        {
            var playerPosition = m_OtherPlayersInstances[key].transform.position;
            m_OtherPlayersInstances[key].transform.position = Vector3.Lerp(playerPosition,
                                                                           playerPosition,
                                                                           0.3f);
        }
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
        // if(Vector3.Distance(m_OldPosition, transform.position) < 1f) 
        //     return;
        // m_OldPosition = transform.position; 

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

using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Agones;
using Agones.Model;
using MyEssentials;
using MyEssentials.Serialization;
public class Server : MonoBehaviour
{
    private int m_Port = 7777;
    private UdpClient m_Server;
    private AgonesSdk m_Agones;
    private GameServer m_GameServer; 
    private SerializationManager m_Serialization;
    private Player[] m_Players;
    private int m_PlayersCount;
    
    private async void CreateAgonesServerV1()
    {        
        m_Agones = GetComponent<AgonesSdk>();
        bool connected = await m_Agones.Connect();
        if(!connected)
        {
            Debug.Log("Could not connect to Agones.");
            Application.Quit(1);
        }        
        Debug.Log("Succesfully connected to Agones");        
        bool ready = await m_Agones.Ready();
        if(!ready)
        {
            Debug.Log("Agones is not ready for retrieving the players.");
            Application.Quit(1);
        }
        Debug.Log("Agones is ready for retrieving the players.");
    }
    private async void CreateGameServerV2()
    {
        m_Agones = GetComponent<AgonesSdk>();
        Task<bool> connectTask = m_Agones.Connect();
        Task<bool> readyTask = m_Agones.Ready();
                
        bool connected = await connectTask;
        if(!connected)
        {
            Debug.Log("Could not connect to Agones.");
            Application.Quit(1);
        }        
        Debug.Log("Succesfully connected to Agones");        
        bool ready = await m_Agones.Ready();
        if(!ready)
        {
            Debug.Log("Agones is not ready for retrieving the players.");
            Application.Quit(1);
        }        
    }
    void Awake()
    {
        ParseCommandLineArguments();
    }

    private void ParseCommandLineArguments()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i] == "--port")
            {
                if(i + 1 == args.Length)
                    throw new Exception("Argument --name without value.");

                if(args[i + 1] == "")
                    throw new Exception("Argument --name without value.");
                
                m_Port = Int32.Parse(args[i + 1]);
            }
        }
    }
    void Start()
    {              
        m_Server = new UdpClient(m_Port);
        CreateAgonesServerV1();        
        m_Serialization = new SerializationManager();
        m_Players = new Player[4];
        m_PlayersCount = 0;
    }
    
    private IEnumerator StopForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    // Update is called once per frame
    void Update()
    {
        if(m_Server.Available > 0)
        {
            IPEndPoint remote = null;
            byte[] recvBytes = m_Server.Receive(ref remote);
            Player player = m_Serialization.DeserializePlayer(recvBytes);            
            if(m_PlayersCount <= m_Players.Length)
            {
                AddNewPlayer(player);                
                byte[] bytes = m_Serialization.SerializePlayers(m_Players);
                m_Server.Send(bytes, bytes.Length, remote);            
            }else
            {
                Debug.Log("Server can't listen to more clients.");
            }
        }
    }    
    private void AddNewPlayer(Player player)
    {
        for(int i = 0; i < m_PlayersCount; i++)
        {
            //Don't add the same player twice, just update position
            if(m_Players[i].id == player.id)
            {                
                m_Players[i].xPosition = player.xPosition;
                m_Players[i].yPosition = player.yPosition;
                m_Players[i].zPosition = player.zPosition;
                Debug.Log(@$"Player {player.id} position x:{player.xPosition} y:{player.yPosition} z:{player.zPosition}");
                return;
            }                
        }
        m_Players[m_PlayersCount] = player;
        m_PlayersCount++;        
        Debug.Log(@$"Player {player.id} position x:{player.xPosition} y:{player.yPosition} z:{player.zPosition}");
    }
    void OnDestroy() 
    {
        Debug.Log("Closing server.");
        m_Server.Close();
    }
}

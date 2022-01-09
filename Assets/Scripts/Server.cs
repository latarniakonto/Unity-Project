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
    private UdpClient m_Client;
    private AgonesSdk m_Agones;
    private GameServer m_GameServer; 
    // Start is called before the first frame update
    
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
        Task<GameServer> gameServerTask = m_Agones.GameServer();
        
        bool connected = await connectTask;
        if(connected)
        {
            Debug.Log("Succesfully connected to Agones.");
            bool ready = await readyTask;
            if(ready)
            {
                Debug.Log("Agones is ready for creating game server.");
                m_GameServer = await gameServerTask;
            }else
            {
                Debug.Log("Agones is not ready for creating game server.");            
            }
        }else
        {
            Debug.Log("Could not connect to Agones.");
        }
        
        if(m_GameServer == null)
        {
            Debug.Log("Succesfully created game server.");
        }else
        {
            Debug.Log("Could not create game server");
        }
    }

    void Start()
    {              
        m_Client = new UdpClient(m_Port);

        CreateAgonesServerV1();
    }
    
    private IEnumerator StopForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    // Update is called once per frame
    void Update()
    {
        if(m_Client.Available > 0)
        {
            IPEndPoint remote = null;
            byte[] recvBytes = m_Client.Receive(ref remote);
            string recvText = Encoding.UTF8.GetString(recvBytes);

            string[] recvTexts = recvText.Split(' ');
            Debug.Log(@$"Player position
                      x:{recvTexts[0]}
                      y:{recvTexts[1]}
                      z:{recvTexts[2]}");
        }
    }    
    void OnDestroy() 
    {
        Debug.Log("Closing server.");
        m_Client.Close();
    }
}

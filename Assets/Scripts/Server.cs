using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Agones;
using Agones.Model;
public class Server : MonoBehaviour
{
    private AgonesSdk m_Agones;
    private GameServer m_GameServer; 
    // Start is called before the first frame update
    
    private async void CreateGameServerV1()
    {        
        m_Agones = GetComponent<AgonesSdk>();
        bool connected = await m_Agones.Connect();
        if(!connected)
        {
            Debug.Log("Could not connect to Agones.");
            return;
        }        
        Debug.Log("Succesfully connected to Agones");
        m_GameServer = await m_Agones.GameServer();
        if(m_GameServer == null)
        {            
            Debug.Log("There was problem with retrieving the game server.");
            return;
        }
        Debug.Log("Succesfully created game server.");
        bool ready = await m_Agones.Ready();
        if(!ready)
        {
            Debug.Log("Game server is not ready for retrieving the players.");
            return;
        }
        Debug.Log("Game server is ready for retrieving the players.");
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
        CreateGameServerV1();                    
        // bool ready = await m_Agones.Ready();
        // if(!ready)
        // {
        //     Debug.Log("Game server is not ready yet.");
        //     return;
        // }
        // m_GameServer = await m_Agones.GameServer();
        // if(m_GameServer == null)
        // {
        //     Debug.Log("There was problem with retrieving the server.");
        //     return;
        // }
    }
    
    private IEnumerator StopForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    // Update is called once per frame
    void Update()
    {

    }    
}

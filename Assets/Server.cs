using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Agones;
public class Server : MonoBehaviour
{
    private AgonesSdk m_Agones;
    // Start is called before the first frame update
    
    private async void ConnectToAgonesV1()
    {
        m_Agones = GetComponent<AgonesSdk>();
        bool connected = await m_Agones.Connect();        
        if(!connected)
        {
            Debug.Log("Could not connect to Agones");
            return;
        }
    }
    private void ConnectToAgonesV2()
    {
        m_Agones = GetComponent<AgonesSdk>();
        bool connected = m_Agones.Connect().GetAwaiter().GetResult();        
        if(!connected)
        {
            Debug.Log("Could not connect to Agones");
            return;
        }
    }
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

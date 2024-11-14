using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
public class EEGReceiver : Singleton<EEGReceiver>
{
    Thread receiveThread;
    UdpClient client;
    public int port = 3000;
    private bool startRecieving = true;
    private string data;
    public string Data => data;
    // Start is called before the first frame update
    void Start()
    {
        receiveThread = new Thread(
        new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    public void ReceiveData()
    {
        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);
                Debug.Log(data);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    protected override void InitAfterAwake()
    {
    }
}
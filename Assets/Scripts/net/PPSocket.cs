using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using netty;

public class PPSocket
{
    //Socket客户端对象     
    private Socket clientSocket; 

    //单例模式     
    private static PPSocket instance;

	private HomeController controller = null;

    private Thread socketThread = null;

    public static PPSocket GetInstance()
    {
        if (instance == null)
        {
            instance = new PPSocket();
        }
        return instance;
    }

    //单例的构造函数     
    PPSocket()
    {
        //创建Socket对象， 这里我的连接类型是TCP     
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

	public bool Connect(HomeController control)
	{
		controller = control;

        if (clientSocket == null)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
		//服务器IP地址     

        IPAddress ipAddress = IPAddress.Parse(control.GetServerIP);
        //IPAddress ipAddress = IPAddress.Parse("192.168.1.104");
		//服务器端口     
		IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, control.GetServerPoint);

        Debug.Log("Connect to " + ipEndpoint.ToString());
		//这是一个异步的建立连接，当连接建立成功时调用connectCallback方法     
		IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), clientSocket);
		Debug.logger.Log("开始网络连接");
		//这里做一个超时的监测，当连接超过5秒还没成功表示超时     
		bool success = result.AsyncWaitHandle.WaitOne(5000, true);
		if (!success)
		{
			//超时     
			Closed();
			Debug.Log("网络连接超时!");
            return false;
		}
		else
		{
			Debug.Log("网络连接成功");

			//与socket建立连接成功，开启线程接受服务端数据。     
			socketThread = new Thread(new ThreadStart(ReceiveSorket));
			socketThread.IsBackground = true;
			socketThread.Start();

            controller.Connected = true;

			return true;
		}
	}

    private void connectCallback(IAsyncResult asyncConnect)
    {
        Debug.Log("ConnectSuccess " + asyncConnect.ToString());
    }

    private void ReceiveSorket()
    {
        //在这个线程中接受服务器返回的数据     
        while (true)
        {
            Debug.Log(" socketCycle! ");
            if (clientSocket == null)
            {
                return;
            }
            if (!clientSocket.Connected)
            {
                //与服务器断开连接跳出循环     
                Debug.Log("Failed to clientSocket server.");
                Closed();
                break;
            }
            try
            {
                //接受数据保存至bytes当中     
                byte[] bytes = new byte[4096];
                //Receive方法中会一直等待服务端回发消息     
                //如果没有回发会一直在这里等着。     
				Debug.Log("开始读取socket数据");
                int i = clientSocket.Receive(bytes);
                if (i <= 0)
                {
                    clientSocket.Close();
                    break;
                }

				Debug.Log("开始读取socket数据成功！");
                //我的包头长度是4，先要监测包头长度  
                if (bytes.Length > 4)
                {
                    SplitPackage(bytes, 0);
                }
                else
                {
                    Debug.Log("length is not  >  4");
                }

                Thread.Sleep(30);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to ReceiveSorket error. " + e);
                clientSocket.Close();
                break;
            }
        }
    }

    private void SplitPackage(byte[] bytes, int index)
    {
        //在这里进行拆包，因为一次返回的数据包的数量是不定的     
		//所以需要给数据包进行查分。 

		// 初始化收包列表
		List<MessageInfo> packages = new List<MessageInfo>();    
        while (true)
        {
            //包头是2个字节     
            byte[] head = new byte[2];
            int headLengthIndex = index + 4;
            //把数据包的前两个字节拷贝出来     
            Array.Copy(bytes, index, head, 0, 2);
            //计算包头的长度     
            short length = BitConverter.ToInt16(head, 0);
            //当包头的长度大于0 那么需要依次把相同长度的byte数组拷贝出来     
            if (length > 0)
            {
                byte[] data = new byte[length];
                //拷贝出这个包的全部字节数     
                Array.Copy(bytes, headLengthIndex, data, 0, length);
                
                //通过google.protoc反序列化
                MemoryStream ms1 = new MemoryStream(data);
                MessageInfo package = ProtoBuf.Serializer.Deserialize<MessageInfo>(ms1);
                packages.Add(package);

                //将索引指向下一个包的包头     
                index = headLengthIndex + length;
            }
            else
            {
                //如果包头为0表示没有包了，那么跳出循环 
                controller.AddNewPackage(packages);
                break;
            }
        }
    }

    //向服务端发送数据包，也就是一个结构体对象     
    public void SendMessage(MessageInfo msg)
    {
        if (clientSocket == null)
        {
            return;
        }

        if (!clientSocket.Connected)
        {
            Closed();
            return;
        }
        try
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string debufinfo = msg.ToString();
            Debug.Log(debufinfo);
#endif
            MemoryStream ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize<MessageInfo>(ms, msg);
            byte[] data = ms.ToArray();
            //先得到数据包的长度
            //把数据包的长度写入byte数组中     
            byte[] head = new byte[4];
            head[0] = (byte)(data.Length & 0xff);
            head[1] = (byte)((data.Length >> 8) & 0xff);
            head[2] = 0;
			head[3] = (byte)msg.messageId;

            //此时就有了两个字节数组，一个是标记数据包的长度字节数组， 一个是数据包字节数组，     
            //同时把这两个字节数组合并成一个字节数组     

            byte[] newByte = new byte[head.Length + data.Length];
            Array.Copy(head, 0, newByte, 0, head.Length);
            Array.Copy(data, 0, newByte, head.Length, data.Length);

			Debug.logger.Log("准备发送请求 msgId = " + msg.messageId + " Msg Lenght = " + newByte.Length);
            //向服务端异步发送这个字节数组     
            IAsyncResult asyncSend = clientSocket.BeginSend(newByte, 0, newByte.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
            //监测超时     
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                clientSocket.Close();
                Debug.LogError("Time Out !");
            }else
            {
                Debug.Log("发送请求 msgId = " + msg.messageId + "成功!");
            }

        }
        catch (Exception e)
        {
            Debug.LogError("send message error: " + e);
        }
    }

    private void sendCallback(IAsyncResult asyncSend)
    {

    }

    //关闭Socket     
    public void Closed()
    {
		Debug.Log ("关闭Socket！！");
        
        if (clientSocket != null)
        {
            if (clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            clientSocket.Close();
        }
        clientSocket = null;
    }

    public void OnQuit()
    {
        if (socketThread != null)
        {
            socketThread.Abort();
        }
        Closed();
    }

    public bool IsConnected()
    {
        return clientSocket != null && clientSocket.Connected;
    }
}
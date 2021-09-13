using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    public static class TcpClientService
    {
        static string userName;
        static TcpClient client;
        static NetworkStream stream;
        private const string HOST = "127.0.0.1";
        private const int PORT = 8888;

        public static void Start()
        {
            Console.Write("Please enter your name: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(HOST, PORT); //подключение клиента
                stream = client.GetStream(); // получаем поток

                var message = userName;
                var data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                var receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        // отправка сообщений
        static void SendMessage()
        {
            //Console.WriteLine("Введите сообщение: ");

            while (true)
            {
                var message = Console.ReadLine();
                var data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }


        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    var data = new byte[64]; // буфер для получаемых данных
                    var sb = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        sb.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine(sb.ToString());//вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Connection lost");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }


        public static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
        }
    }
}

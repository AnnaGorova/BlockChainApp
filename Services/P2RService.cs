using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlockChainApp.Models;

namespace BlockChainApp.Services
{
    public class P2RService
    {
        private readonly BlockChain _blockChain;
        private readonly List<TcpClient> _peers = new List<TcpClient>();
        private readonly object _peerLock = new object();
        public int Port { get; private set; } = 5000;

        public P2RService(BlockChain blockChain)
        {
            _blockChain = blockChain;
        }


        public void StartServer(int port)
        {
            Port = port;
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Server Start");

            Task.Run(() =>
            {
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("New peer connected");
                   
                    lock (_peerLock)
                    {
                        _peers.Add(client);
                    }
                    
                    
                    HandleClinet(client);
                }
            });
            
        }

        public void ConnectToPeer(string ip, int port)
        {
            var client = new TcpClient();
            client.Connect(ip, port);
            Console.WriteLine($"Connected to peer {ip}:{port}");
            
            lock (_peerLock)
            {
                _peers.Add(client);
            }
            
            Task.Run(() =>  HandleClinet(client));
        }

        public void HandleClinet(TcpClient client)
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream);

            while(client.Connected)
            {
                try
                {
                    string json = reader.ReadLine();
                    if (!string.IsNullOrEmpty(json))
                    {
                        //Console.WriteLine($"Отримано: {json}");

                        var message = JsonSerializer.Deserialize<P2PMessage>(json);
                        ProcessMessage(message);
                    }
                }
                catch(Exception ex)
                {
                    lock (_peerLock)
                    {
                        _peers.Remove(client);
                    }
                    
                    Console.WriteLine($"Error handling client: {ex.Message}");
                    break;
                }
            }
        }

        private void ProcessMessage(P2PMessage? message)
        {
            if (message == null)
            {
                Console.WriteLine("Отримано null повідомлення");
                return;
            }

            if (message.Type == MessageType.BroadcastBlock)
            {
                var newBlock = JsonSerializer.Deserialize<Block>(message.Data);

                var hasingService = new HashingService();
                var calculatedHash = hasingService.ComputeHash(newBlock);

                var tartetHash = new string('0', newBlock.Difficulty);

                if (calculatedHash == newBlock.Hash && calculatedHash.StartsWith(tartetHash))
                {
                    _blockChain.Chain.Add(newBlock);
                    Console.WriteLine($"New block added: {newBlock.Index}");
                }

                //if (newBlock.Index > _blockChain.Chain.Last().Index)
                //{
                //    _blockChain.Chain.Add(newBlock);
                //    Console.WriteLine($"New block added: {newBlock.Index}");

                //}
            }
            else if (message.Type == MessageType.BroadcastTransaction)
            {
                var newTransaction = JsonSerializer.Deserialize<Transaction>(message.Data);
                _blockChain.AddTransaction(newTransaction);
                //_blockChain.AddTransactionFromNetwork(newTransacAdd      }
        }
        }

        public void BroadCast(MessageType messageType, object data)
        {
            var message = new P2PMessage
            {
                Type = messageType,
                Data = JsonSerializer.Serialize(data)
            };
            string json = JsonSerializer.Serialize(message);

            List<TcpClient> peers;
            lock (_peerLock)
            {
                peers = new List<TcpClient>(_peers);
            }
            
            foreach (var peer in peers)
            {
                try
                {
                    var stream = peer.GetStream();
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    writer.WriteLine(json);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error broadcasting to peer: {ex.Message}");
                }
            }
        }
    }
}

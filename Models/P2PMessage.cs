using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChainApp.Models
{
    public class P2PMessage
    {
        public string Data { get; set; }
        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        BroadcastBlock,
        BroadcastTransaction,
        
    }
}

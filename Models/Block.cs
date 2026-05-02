using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChainApp.Models
{
    public class Block
    {
        public Block(int index, List<Transaction> transactions, string prevHash, string author,  int difficulty)
        {
            Index = index;
            PrevHash = prevHash;
            Author = author;
            Difficulty = difficulty;
            Transactions = transactions;

        }

        public int Index { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public List<Transaction> Transactions { get; set; }
        public string Hash { get; set; }
        public string PrevHash { get; set; }
        public long Nonce { get; set; } //важкість алгоритму майнінгу
        public string Author { get; set; }

        public long Attemps { get; set; } //кількість спроб 

        public double MiningDuration { get; set; } // час в секугдах  для  здобуття блоку 
        
        // властивість для зберігання скаладності майнінгу,
        // яка була використана для майнінгу цього блоку
        public int Difficulty { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockChainApp.Models;
namespace BlockChainApp.Services
{
    public class HashingService
    {
       public string ComputeHash(Block block)
       {
            var transactionData = "";

            foreach (var transaction in block.Transactions)
            {
                transactionData += transaction.ToRawString();
            }

            string blockData = $"{block.Index}{block.Timestamp.ToString("o")}{transactionData}{block.PrevHash}{block.Nonce}{block.Author}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(blockData);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}

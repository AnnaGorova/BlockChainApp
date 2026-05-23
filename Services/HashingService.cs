using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

            //foreach (var transaction in block.Transactions)
            //{
            //    transactionData += transaction.ToRawString();
            //}

            string blockData = $"{block.Index}{block.Timestamp.ToString("o")}{transactionData}{block.PrevHash}{block.Nonce}{block.Author}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(blockData);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            
        }

        public string ComputerSHA256(string rowData)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(rowData);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        public string GetMerkleRoot(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
                return string.Empty;
            List<string> merkleLeaves = transactions.Select(t => ComputerSHA256(t.ToRawString())).ToList();
            while (merkleLeaves.Count > 1)
            {
                List<string> newLewel = new List<string>();
                for (int i = 0; i < merkleLeaves.Count; i += 2)
                {
                    string left = merkleLeaves[i];
                    string right = (i + 1 < merkleLeaves.Count) ? merkleLeaves[i + 1] : left;
                    newLewel.Add(ComputerSHA256(left + right));
                }

                if (merkleLeaves.Count%2 != 0)
                {
                    newLewel.Add(merkleLeaves.Last());
                }

                merkleLeaves = newLewel;
            }
            return merkleLeaves[0];
        }


       
    }
}

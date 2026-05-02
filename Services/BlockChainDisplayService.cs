using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockChainApp.Models;



namespace BlockChainApp.Services
{
    public class BlockChainDisplayService
    {
        private readonly TransactionService _transactionService;
        
        public BlockChainDisplayService()
        {
            _transactionService = null; // порожнє, не ініціалізоване  ->  робиться для більшої гнучкості
        }

    


        // Метод для відображення про блок
        public void PrintBlock(Block block)
        {
            var randomColor = (ConsoleColor)(new Random().Next(11, 15));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Author: {block.Author}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"_________________________");
            Console.ForegroundColor = randomColor;
            Console.WriteLine($"Index: {block.Index}");
            Console.WriteLine($"Timestamp: {block.Timestamp}");
           
            Console.WriteLine($"Hash: {block.Hash}");
            Console.WriteLine($"Prev Hash: {block.PrevHash}");
            Console.WriteLine($"Nonce: {block.Nonce}");
            Console.WriteLine($"Difficulty: {block.Difficulty}");
            Console.WriteLine($"Mining attempts: {block.Attemps}");
            Console.WriteLine($"Mining Duration: {block.MiningDuration} seconds");
            
            Console.WriteLine($"Hasher: {block.Attemps / block.MiningDuration} hasher/second");
                      
                       
            Console.WriteLine(new string('-', 40));


            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (var tx in block.Transactions)
            {
                if (tx.From == "COINBASE")
                {
                    Console.WriteLine($"Transaction: COINBASE -> {tx.To},  Amount: {tx.Amount}");
                }
                else
                {
                    Console.WriteLine($"Transaction: {tx.From} to {tx.To}, Amount: {tx.Amount}");
                }
            }
            Console.WriteLine("----------------------------------------");
        }

        // Метод для відображення валідації блокчейн
        public void PrintValidationResult(bool isValid)
        {
            if (isValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Blockchan is valid.");
                Console.ForegroundColor = ConsoleColor.Gray;


            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Blockchan is invalid.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        // Метод для відображення всієї інформації про блокчейн
        public void printBlockChain(List<Block> chain)
        {
            foreach (var block in chain)
            {
                PrintBlock(block);
            }
        }




        public void PrintBenchmarkResult(Block block)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Mining attempts: {block.Attemps}");
            Console.WriteLine($"Time taken: {block.MiningDuration}");
            Console.WriteLine($"Difficulty: {block.Difficulty}");
            Console.WriteLine($"Hasher: {block.Attemps / block.MiningDuration} hasher/second");
            Console.WriteLine($"Duration: {block.MiningDuration}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        //// Метод для відображення результатів бенчмарку майнінгу
        //public void PrintBenchmarkResult(long attempts, TimeSpan TimeTaken, int difficulty)
        //{
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.WriteLine($"Mining attemps: {attempts}");
        //    Console.WriteLine($"Time taken: {TimeTaken.TotalSeconds} seconds");
        //    Console.WriteLine($"Difficulty: {difficulty}");
        //    Console.WriteLine($"Hashrate: {attempts / TimeTaken.TotalNanoseconds} hashes/second");
        //    Console.WriteLine($"Duration per hash: {attempts / TimeTaken.TotalNanoseconds} es/second");
        //    Console.ForegroundColor = ConsoleColor.Gray;
        //    Console.ForegroundColor = ConsoleColor.Gray;

        //}


       




    }
}

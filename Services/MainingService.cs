using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockChainApp.Models;

namespace BlockChainApp.Services
{
    public class MainingService
    {
        private readonly HashingService _hashingService;
        public MainingService(HashingService hashingService)
        {
            _hashingService = hashingService;
        }



        // Меетод для майнінгу блоку з заданою скадністю 
        public long MineBlock(Block block, int difficulty)
        {
            string target = new string('0', difficulty); // "0000"          


            var stopwatch = Stopwatch.StartNew();

            while(true)
            {
                block.Hash = _hashingService.ComputeHash(block);
                
                //перевіряємо, чи відповідає хеш вимогам складності (кількість перевірених нулів)
                if (block.Hash.Substring(0, difficulty) == target)
                {
                   break;
;               }
                block.Nonce++;
            }
            stopwatch.Stop();
            block.MiningDuration = stopwatch.Elapsed.TotalSeconds;
            block.Attemps = block.Nonce;
            return block.Nonce;
        }




    }

}

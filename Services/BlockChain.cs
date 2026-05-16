using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using BlockChainApp.Models;

namespace BlockChainApp.Services
{
    public class BlockChain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; } = 1;

        private readonly double _targetBlockTime = 10;  // цільвий час майнінгу блоку в секундах 

        private readonly int _adjustmentIntervsl = 1; // інтервал для коригування складності (кожні 5 блоків)

        private readonly HashingService _hashingService;

        private readonly MainingService _mainingService;

        private readonly List<Transaction> _pendingTransaction = new List<Transaction>();


       

        private readonly WalletService _walletService = new WalletService();

        

        private readonly int minerReward = 0; // винагорода майнеру за створення блоку

        private readonly string _storageFilePath = "blockchain_data.dat";
        public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();

        public BlockChain(int difаicalty)
        {
            Chain = new List<Block>();
            _hashingService = new HashingService();
            _mainingService = new MainingService(_hashingService);
            this.Difficulty = difаicalty;
            //CreateGenesisBlock();
            if (Chain.Count == 0)
            {
                CreateGenesisBlock();
            }

        }

        // Метод для створення генезис-блоку 
        private void CreateGenesisBlock()
        {
            var genesissBlock = new Block(0, new List<Transaction>(), "0", "Admin", 0)
            {
                Timestamp = DateTime.Parse("2024-01-01T01:00:00Z"),
                Nonce = 0,
            };
           
            // Майніннг генезис-блоку для встановлення правильного хешу
            _mainingService.MineBlock(genesissBlock, Difficulty);
            
            Chain.Add(genesissBlock);
            
            this.AppyBlockToState(genesissBlock);
            this.AppendBlockToFile(genesissBlock);
        }

        //Метод для додавання нового блоку до блокчейну
        public void AddBlock(List<Transaction> transactions, string author)
        {
            var lastBlock = Chain.Last();
            var newBlock = new Block(lastBlock.Index + 1, transactions, lastBlock.Hash, author, Difficulty);
            
            //Майніннг нового блокук для встановлення правильного хешу
            _mainingService.MineBlock(newBlock, Difficulty);
            
            
            Chain.Add(newBlock);

            if (newBlock.Index % _adjustmentIntervsl == 0 ) 
            {
                AdjustDiffucuty();
            }


        }


        public bool AddTransaction(Transaction transaction)
        {
            // 1. пропускаємо перевірку балансу для системних транзакцій 
            if (transaction.From != "COINBASE")
            {
                // перевіряємо підпис транзакції 
                bool isValid = _walletService.VeryfiSignature(
                    transaction.GetDataSing(), 
                    transaction.Signature, 
                    transaction.PublicKey
                    );
                
                if (!isValid)
                {
                    Console.Write("Транзакція відхилена: невалідний підпис!");
                    return false;
                }

                //if (transaction.From != "COINBASE")
                //{
                //    decimal senderBalance = GetBalance(transaction.From);
                //    if (senderBalance < transaction.Amount + transaction.Fee);
                //}

                // Перевірка балансу  через словник State
                //decimal balance = GetBalance(transaction.From);
                // отримуємо баланс з кешу State
                decimal balance = Balances.ContainsKey(transaction.From) 
                    ? Balances[transaction.From] 
                    : 0;

                //віднімаю суму від транзакій які не були підтверджені в Mempool
                foreach(var tx in _pendingTransaction)
                {
                    if (tx.From == transaction.From)
                    {
                        balance -= (tx.Amount + tx.Fee);
                    }
                }

                decimal requireAmount = transaction.Amount + transaction.Fee;

                if (balance < requireAmount)
                {
                    Console.WriteLine("Транзакція відхилена: недостатньо коштів");
                    Console.WriteLine($"Баланс: {balance}, Потрібно: {requireAmount}");
                    return false;
                }

                Console.WriteLine($"Транзакція валідна - Баланс: {balance} >= Потрібно: {requireAmount}");
            }

            // Додаємо в mempool
            _pendingTransaction.Add(transaction);
            return true;
        }

        //  Додає транзакцію, отриману з мережі, до локального Mempool
        public bool AddTransactionFromNetwork(Transaction transaction)
        {
            if (transaction.From != "COINBASE")
            {
                bool isValid = _walletService.VeryfiSignature(
                    transaction.GetDataSing(),
                    transaction.Signature,
                    transaction.PublicKey
                );

                if (!isValid)
                    return false;

                decimal balance = GetBalance(transaction.From);
                decimal requireAmount = transaction.Amount + transaction.Fee;

                if (balance < requireAmount)
                    return false;

            }

            _pendingTransaction.Add(transaction);
            return true;
        }

        //public decimal GetBalance(string address)
        //{
        //    decimal balance = 0;

        //    // Преревіряємо всі блоки в ланцюгу
        //    foreach (var block in Chain)
        //    {

        //        foreach (var tx in block.Transactions)
        //        {
        //            // якщо адреса отримувача - додаємо суму
        //            if (tx.To == address)
        //            {
        //                balance += tx.Amount;
        //            }

        //            // якщо адреса відправник = віднімаємо суму + комісію
        //            if (tx.From == address && tx.From != "COINBASE")
        //            {
        //                balance -= (tx.Amount + tx.Fee);
        //            }
        //        }

        //    }

        //    foreach (var tx in _pendingTransaction)
        //    {
        //        if (tx.From == address && tx.From != "COINBASE")
        //        {
        //            balance -= (tx.Amount + tx.Fee);
        //        }
        //    }

        //    return balance;

        //}



        public void MinePendingTransaction(Wallet minerWallet, int max)
        {
            var lastBlock = Chain.Last();

            Console.WriteLine($"Mempul має {_pendingTransaction.Count} транзакцій");
            Console.WriteLine($"Майнер  {minerWallet.Address} починає майніннг...");



            var transactionToInclude = _pendingTransaction
                .OrderByDescending(t => t.Fee)
                .Take(max)
                .ToList();

            Console.WriteLine($"Обрано транзакції: {transactionToInclude.Count}");

            foreach (var tx in transactionToInclude)
            {
                Console.WriteLine($"Транзакція: {tx.From} -> {tx.To}, " +
                    $"Amount: {tx.Amount}, Fee: {tx.Fee}");
            }

            var totalFees = transactionToInclude.Sum(t => t.Fee);
            Console.WriteLine($"Загальна сума комісії: {totalFees}");

            var block = new Block(
               lastBlock.Index + 1,
               transactionToInclude, 
               lastBlock.Hash,
               minerWallet.Address,
               Difficulty
             );

            var minerRewarTX = new Transaction
            {
                From = "COINBASE",
                To = minerWallet.Address,
                Amount = minerReward + totalFees,
                Timestamp = DateTime.UtcNow
            };

            block.Transactions.Add(minerRewarTX);
            _mainingService.MineBlock(block, Difficulty);
            Chain.Add(block);
           
            this.AppyBlockToState(block);
            this.AppendBlockToFile(block);
            _pendingTransaction.RemoveAll(t => transactionToInclude.Contains(t));
            if (block.Index % _adjustmentIntervsl == 0)
            {
                AdjustDiffucuty();
            }
        }


        // Це ключова частина логіки блокчейну.
        // Вона відповідає за те, щоб блоки створювалися стабільно, незалежно від того, наскільки потужний комп'ютер у майнера.
        private void AdjustDiffucuty()
        {
            // коригуємо складність майніннгу кожні _adjustmentIntervsl блоків
            var recentBlock = Chain.Where(b => b.Index > 0).TakeLast(_adjustmentIntervsl).ToList();

            if (recentBlock.Count < _adjustmentIntervsl)
            {
                return; // недостатньо блоків для коригування 
            }

            // обчислює середній час майнінгу за останні _adjustmentIntervsl блоків
            double avrageTime = recentBlock.Average(b => b.MiningDuration);

            if (avrageTime < _targetBlockTime)
            {
                Difficulty++;  // збільшуємо майнінг бо блоки створються надто швидко 
            }
            else if (avrageTime > _targetBlockTime)
            {
                Difficulty = Math.Max(1, Difficulty - 1); // зменшуємо складність, якщо блоки майняться занадто повільно
            }
        }






        //Метод для перевірки цілісності блокчейну
        public bool isValid(List<Block> chain)
        {
            for (int i = 1; i < chain.Count; i++)
            {
                var currentBlock = chain[i];

                var previousBlock = chain[i - 1];
                // перевіряємо чи хеш поточного блоку правильний (чи не були змінені дані в блоці)
                if (currentBlock.Hash != _hashingService.ComputeHash(currentBlock))
                    return false;
                // Перевіряємо чи PrevHesh вказує на попередній
                if (currentBlock.PrevHash != previousBlock.Hash)
                    return false;
                //Перевірка складності майніннгу
                if (!currentBlock.Hash.StartsWith(new string('0', currentBlock.Difficulty)))
                    return false;

                foreach (var transaction in currentBlock.Transactions)
                {
                    if (transaction.From != "COINBASE")
                    {
                        bool isValid = _walletService.VeryfiSignature(transaction.GetDataSing(), transaction.Signature, transaction.PublicKey);
                        if (!isValid)
                            return false;
                    }
                }

            }
            Console.WriteLine("Валідний");
            return true;
        }


        public decimal GetBalance(string address)
        {

            var balance = Balances.ContainsKey(address) ? Balances[address] : 0;

            foreach (var tx in _pendingTransaction)
            {
                if (tx.From == address)
                {
                    balance -= tx.Amount + tx.Fee;
                }
                if(tx.To == address)
                {
                    balance += tx.Amount;
                }

                
            }
            return balance;

            //decimal balance = 0;

            //// Преревіряємо всі блоки в ланцюгу
            //foreach (var block in Chain)
            //{

            //    foreach (var transaction in block.Transactions)
            //    {
            //        // якщо адреса отримувача - додаємо суму
            //        if (transaction.To == address)
            //        {
            //            balance += transaction.Amount;
            //        }

            //        // якщо адреса відправник = віднімаємо суму + комісію
            //        if (transaction.From == address)
            //        {
            //            balance -= (transaction.Amount + transaction.Fee);
            //        }
            //    }

            //}

            //foreach (var transaction in _pendingTransaction)
            //{
            //    if (transaction.To == address )
            //    {
            //        balance += transaction.Amount;
            //    }
            //    if (transaction.From == address)
            //    {
            //        balance -= (transaction.Amount + transaction.Fee);
            //    }
            //}

            return balance;

        }



        private void AppyBlockToState(Block block)
        {
            foreach (var transaction in block.Transactions)
            {
                if (transaction.From != "COINBASE")
                {
                    if (Balances.ContainsKey(transaction.From))
                    {
                        Balances[transaction.From] -= transaction.Amount + transaction.Fee;

                    }
                    else
                    {
                        Balances[transaction.From] = -(transaction.Amount + transaction.Fee);
                    }
                                        
                }
                if (Balances.ContainsKey(transaction.To))
                {
                    Balances[transaction.To] += transaction.Amount;
                }
                else
                {
                    Balances[transaction.To] = transaction.Amount;
                }

            }
        }

        public void AppendBlockToFile(Block block)
        {
            string jsonLine = JsonSerializer.Serialize(block);
            File.AppendAllLines(_storageFilePath, new[] { jsonLine });
        }
           
        public void LoadChainFromFile()
        {
            if (!File.Exists(_storageFilePath))
                return;
            var lines = File.ReadLines(_storageFilePath);
            var loadedChain = new List<Block>();
           
            Chain.Clear();
            Balances.Clear();

            foreach (var line in lines)
            {
                var block = JsonSerializer.Deserialize<Block>(line);
                if (block != null)
                {
                    //Chain.Add(block);
                    //this.AppyBlockToState(block);
                    loadedChain.Add(block);
                }
            }
            var filteredChain = new List<Block>();
            var seenIndices = new HashSet<int>();

            foreach (var block in loadedChain)
            {
                if (!seenIndices.Contains(block.Index))
                {
                    seenIndices.Add(block.Index);
                    filteredChain.Add(block);
                }
                else
                {
                    Console.WriteLine($"Виявлено дублікат блоку з індексом {block.Index}. Видаляємо...");
                }
            }

            //  перевіряю цілісність ланцюга
            if (!ValidateBlockchainIntegrity(filteredChain))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("КРИТИЧНА ПОМИЛКА: Файл blocks.dat скомпрометовано!");
                Console.WriteLine("Завантаження блокчейну скасовано!!!");
                Console.ForegroundColor = ConsoleColor.White;


                Chain.Clear();
                Balances.Clear();

                return;
            }

            // Якщо перевірку пройдено, то завантажуємо ланцюг і відновлюємо State
            Chain.Clear();
            Balances.Clear();
            
            foreach (var block in filteredChain)
            {
                Chain.Add(block);
                AppyBlockToState(block);
            }

            Console.WriteLine($"Завантажено {filteredChain.Count} блоків.");
        }

        private bool ValidateBlockchainIntegrity(List<Block> blocks)
        {
            if (blocks.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                var currentBlock = blocks[i];

                // перевіряємо хеш блоку 
                string computedHash = _hashingService.ComputeHash(currentBlock);

                if (currentBlock.Hash != computedHash)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка в блоці {i}: Хеш не відповідає обчисленому!");
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }

                // перевіряємо PrevHash всіх, крім генезис - блоку
                if (i > 0)
                {
                    var previousBlock = blocks[i - 1];
                    if (currentBlock.PrevHash != previousBlock.Hash)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Помилка в блоці {i}: PrevHash не вказує на попередній блок!");
                        Console.ForegroundColor = ConsoleColor.White;
                        return false;
                    }
                }

                // перевірка складності майнінгу
                if (!currentBlock.Hash.StartsWith(new string('0', currentBlock.Difficulty))) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка в блоці {i}: Неправильна складність майніннгу! ");
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }


                // перевірку піддпису
                foreach (var tx in currentBlock.Transactions)
                {
                    if (tx.From != "COINBASE")
                    {
                        bool isValid = _walletService.VeryfiSignature(
                            tx.GetDataSing(),
                            tx.Signature,
                            tx.PublicKey
                            );
                        if (!isValid)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Помилка в блоці {i}: Невалідний підпис транзакції! ");
                            Console.ForegroundColor = ConsoleColor.White;
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        public int GetPendingTransactionCount()
        {
            return _pendingTransaction.Count;
        }





        public bool ReplaceChain(List<Block> newChain)
        {
            if (newChain.Count <= Chain.Count)
                return false;
            if (!isValid(newChain))
                return false;

            var oldTransactions = Chain.SelectMany(b => b.Transactions).Where(x => x.From != "COINBASE").ToList();            
            var newTransactions = newChain.SelectMany(b => b.Transactions).Where(x => x.From != "COINBASE").Select(x => x.Signature).ToList(); // Всі транзакції з нового ланцюга
           
            foreach (var tx in oldTransactions)
            {
                if (!newTransactions.Any(sig => sig.SequenceEqual(tx.Signature)))
                {
                    _pendingTransaction.Add(tx);
                }
            }
            
            
            Chain = newChain;
            Difficulty = newChain.Last().Difficulty;

            if (File.Exists(_storageFilePath))
            {
                File.Delete(_storageFilePath);
            }

            Balances.Clear();
            foreach (var block in Chain)
            {
                AppyBlockToState(block);
                AppendBlockToFile(block);

            }
            return true;
                   
        }
    }
}

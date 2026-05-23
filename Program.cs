using System.Security.Cryptography.X509Certificates;
using BlockChainApp.Models;
using BlockChainApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlockChainApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 5000;
            if (args.Length > 0)
            {
                port = int.Parse(args[0]);
            }


            string storageFile = "blockchain_data.dat";
            if (File.Exists(storageFile)) File.Delete(storageFile);

            var walletService = new WalletService();
            var transactionService = new TransactionService(walletService);

            var originalChain = new BlockChain(1);

            var alice = walletService.CreateWallet("Alice");
            var hacker = walletService.CreateWallet("Hacker");


            originalChain.AddTransaction(new Transaction
            {
                From = "COINBASE",
                To = alice.Address,
                Amount = 100,
                Fee = 0,
                Timestamp = DateTime.UtcNow
            });
            originalChain.MinePendingTransaction(alice, 10);

            var legalTx = transactionService.CreateTransaction(alice, hacker.Address, 5, 0.1m);
            originalChain.AddTransaction(legalTx);
            originalChain.MinePendingTransaction(alice, 10);

            Console.WriteLine($"Баланс Аліси до злому: {originalChain.GetBalance(alice.Address)}");
            Console.WriteLine($"Баланс Хакера до злому: {originalChain.GetBalance(hacker.Address)}");
            Console.WriteLine($"Файл збережено:{storageFile}");



            string fileContent = File.ReadAllText(storageFile);

            string originalAmount = "\"Amount\":5";
            string fakeAmount = "\"Amount\":50000";

            if (fileContent.Contains(originalAmount))
            {
                fileContent = fileContent.Replace(originalAmount, fakeAmount);
                Console.WriteLine($"Злом - знайдено транзакцію {originalAmount} та змінено на {fakeAmount}");
            }
            else
            {
                Console.WriteLine($"Не знайдено транзакцію з {originalAmount}");
            }

            File.WriteAllText(storageFile, fileContent);
            Console.WriteLine($"Файл {storageFile} скомпроментовано!");

            Console.WriteLine($"Перезапуск ноди...");

            var newChain = new BlockChain(1);
            newChain.LoadChainFromFile();

            // Перевірка 1 (Реакція системи): Зробіть скріншот консолі,
            // де видно, що під час ініціалізації нового об'єкта BlockChain
            // спрацював ваш захист і вивів червоне повідомлення про
            // компрометацію.
            Console.WriteLine("Перевірка 1 (Реакція системи) - повідомлення червоним вище");


            // Перевірка 2 (Ізоляція вірусу): Виведіть довжину ланцюга
            // відновленої мережі newBlockChain.Chain.Count.
            // Очікується: 0 (або 1, якщо ваш конструктор одразу перестворив
            // чистий Генезис).
            Console.WriteLine("Перевірка 2 (Ізоляція вірусу");
            Console.WriteLine($"Довжина ланцюга після злому: {newChain.Chain.Count} - очікується 0");
            Console.WriteLine(newChain.Chain.Count == 0
                ? "Перевірку пройддено - очікується 0"
                : "НЕ ПРОЙДЕНО");


            // Перевірка 3 (Блокування фінансів): Виведіть баланс Хакера
            // в новій мережі: newBlockChain.GetBalance(Hacker.Address).
            // Очікується: 0 (підроблені 50000 монет не були завантажені).
            Console.WriteLine("Перевірка 3 (Блокування фінансів)");
            decimal hackerBalance = newChain.GetBalance(hacker.Address);
            Console.WriteLine($"Баланс Хакера після злому: {hackerBalance} - очікується 0");
            Console.WriteLine(hackerBalance == 0
                ? "Пройдено перевірку - підроблені кошти відхилено"
                : "Перевірку не пройдено");





            //var localNode = new BlockChain(1);
            //var hackerNode = new BlockChain(1);
            //var honestNetwork = new BlockChain(1);

            //var minerWallet = walletService.CreateWallet("Miner");
            //var hackerWallet = walletService.CreateWallet("Hacker");
            //var poolWallet = walletService.CreateWallet("Pool");

            //for (int i = 0; i < 2; i++)
            //{
            //    localNode.AddTransaction(new Transaction
            //    {
            //        From = "COINBASE",
            //        To = minerWallet.Address,
            //        Amount = 50,
            //        Fee = 0,
            //        Timestamp = DateTime.UtcNow
            //    });
            //    localNode.MinePendingTransaction(minerWallet, 10);
            //}
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"localNode має {localNode.Chain.Count} блоків - очікувано 3");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine();
            //Console.WriteLine();



            //hackerNode.AddTransaction(new Transaction
            //{
            //    From = "COINBASE",
            //    To = hackerWallet.Address,
            //    Amount = 50,
            //    Fee = 0,
            //    Timestamp = DateTime.UtcNow
            //});
            //hackerNode.MinePendingTransaction(hackerWallet, 10);

            //var lastBlock = hackerNode.Chain.Last();
            //for (int i = 0; i < 5; i++)
            //{
            //    var fakeBlock = new Block(
            //        lastBlock.Index + 1 + i,
            //        new List<Transaction>(),
            //        lastBlock.Hash,
            //        "Hacker",
            //        1
            //    );
            //    fakeBlock.Hash = "000Fake";
            //    fakeBlock.Nonce = 0;
            //    hackerNode.Chain.Add(fakeBlock);
            //    lastBlock = fakeBlock;
            //}
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"hackerNode має {hackerNode.Chain.Count} блоків - очікувано 7 -  хеші невалідні");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine();
            //Console.WriteLine();


            //for (int i = 0; i < 4; i++)
            //{
            //    honestNetwork.AddTransaction(new Transaction
            //    {
            //        From = "COINBASE",
            //        To = poolWallet.Address,
            //        Amount = 50,
            //        Fee = 0,
            //        Timestamp = DateTime.UtcNow
            //    });
            //    honestNetwork.MinePendingTransaction(poolWallet, 10);
            //}
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"honestNetwork має {honestNetwork.Chain.Count} блоків - очікувано 5");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine();
            //Console.WriteLine();



            //// Перевірка 1 (Стійкість до фейкового хешрейту):
            //// Спробуйте викликати localNode.ReplaceChain(hackerNode.Chain);
            //// і виведіть результат цієї операції, а також поточну довжину
            //// ланцюга localNode. Очікується: операція поверне False (відхилено),
            //// а довжина ланцюга залишиться 3.

            //bool attackResult = localNode.ReplaceChain(hackerNode.Chain);
            //Console.WriteLine($"Результат ReplaceChain (hackerNode.Chain): {attackResult}");
            //Console.WriteLine($"localNode має: {localNode.Chain.Count} має блоків після атаки");

            //if (!attackResult && localNode.Chain.Count == 3)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Первірку пройдено, Фейковий ланцюг віхилено");
            //    Console.ForegroundColor = ConsoleColor.White;
            //} else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Первірку НЕ пройдено, Фейковий ланцюг прийнято");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            //Console.WriteLine();
            //Console.WriteLine();

            //// Перевірка 2(Успішний Консенсус Накамото):
            //// Тепер викличте localNode.ReplaceChain(honestNetwork.Chain);
            //// і виведіть результат, а також нову довжину ланцюга.Очікується:
            //// операція поверне True(прийнято), довжина ланцюга стане 5.
            //Console.WriteLine($"localNode має {localNode.Chain.Count} до синхронізації");
            //bool consensusResalt = localNode.ReplaceChain(honestNetwork.Chain);
            //Console.WriteLine($"результат ReplaceChain(honestNetwork.Chain): {consensusResalt}");
            //Console.WriteLine($"localNode має: {localNode.Chain.Count} бллоків після синхронізації");


            //if (consensusResalt && localNode.Chain.Count == 5)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Первірку пройдено, Чесний довший ланцюг прийнято");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Первірку НЕ пройдено, Чесний ланцюг не прийняято");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            //Console.WriteLine();
            //Console.WriteLine();

            //// Перевірка 3 (Економіка після синхронізації):
            //// Виведіть баланс гаманця Pool, використовуючи
            //// миттєвий словник нашої синхронізованої ноди:
            //// localNode.State[Pool.Address].
            //// Очікується: 200 (4 нагороди по 50 монет від
            //// чесної мережі).
            //// (Підказка: якщо отримуєте KeyNotFoundException,
            //// перевірте, чи коректно ваш метод ReplaceChain
            //// викликає оновлення State).
            //decimal poolBalace = localNode.Balances.ContainsKey(poolWallet.Address)
            //    ? localNode.Balances[poolWallet.Address]
            //    : 0;
            //Console.WriteLine($"Баланс Pool після синхронізації: {poolBalace} монет" +
            //    $"- Очікується 200 монет - 4 винагороди по 50");
            //Console.WriteLine();

            //if (poolBalace == 200)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Первірку пройдено, баланс відповідає потрібному");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine($"Первірку НЕ пройдено,Баланс  = {poolBalace} монет - очікували 200 монет.");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            //Console.WriteLine();
            //Console.WriteLine();




            //var blockChain = new BlockChain(1);
            //var transactionService = new TransactionService(walletService);

            //var p2pService = new P2RService(blockChain);
            //var displayService = new BlockChainDisplayService();
            //var hashingService = new HashingService();





            ////var alice = walletService.CreateWallet("Alice");
            ////var miner = walletService.CreateWallet("Bob");
            //////var myWallet = walletService.CreateWallet("Vlad");

            //var aliceWallet = walletService.CreateWallet("Alice");
            //var hackerWallet = walletService.CreateWallet("Hacker");
            //var bobeWallet = walletService.CreateWallet("Bob");
            //var myWallet = walletService.CreateWallet("Vlad");
            ////var minerWallet = walletService.CreateWallet("Miner");



            //// Майнинг початкового блоку для отримання нагороди
            //blockChain.MinePendingTransaction (aliceWallet, 10);
            //blockChain.MinePendingTransaction(aliceWallet, 10);

            //// Перевірка балансу після майнингу
            //Console.WriteLine($"Alice wallet balance: {blockChain.GetBalance(aliceWallet.Address)}");
            //Console.WriteLine($"Hacker wallet balance: {blockChain.GetBalance(hackerWallet.Address)}");

            //// Створення транзакцій з різними комісіями
            ////var transaction1 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 4, 1.01m);
            ////var transaction2 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 2, 0.8m);
            ////var transaction3 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 9, 2.0m);
            //var transaction1 = transactionService.CreateTransaction(aliceWallet, hackerWallet.Address, 5, 1.0m);
            //var transaction2 = transactionService.CreateTransaction(aliceWallet, hackerWallet.Address, 10, 1.0m);
            //var transaction3 = transactionService.CreateTransaction(aliceWallet, hackerWallet.Address, 15, 1.0m);
            //var transaction4 = transactionService.CreateTransaction(aliceWallet, hackerWallet.Address, 20, 1.0m);


            //// Додавання транзакцій до блокчейну
            //blockChain.AddTransaction(transaction1);
            //blockChain.AddTransaction(transaction2);
            //blockChain.AddTransaction(transaction3);
            //blockChain.AddTransaction(transaction4);

            //// Майнинг блоку для обробки транзакцій
            ////blockChain.MinePendingTransaction(bobeWallet, 5);
            //blockChain.MinePendingTransaction(aliceWallet, 10);

            //var lastBlock = blockChain.Chain.Last();
            //Console.WriteLine($"Блок {lastBlock.Index} намайнено!");
            //Console.WriteLine($"Кількість транзакцій у блоці: {lastBlock.Transactions.Count}");

            //string originalMerkleRoot = hashingService.GetMerkleRoot(lastBlock.Transactions);
            //Console.WriteLine($"Оригінальний Корінь Меркла: {originalMerkleRoot}");


            //Console.WriteLine("Хакер");
            //Console.WriteLine("Хакер змінює суму переказу з 10 на 999999 монет!");
            //lastBlock.Transactions[1].Amount = 999999m;

            //////("Перевірка 1:  (Block Explorer): " +
            ////    "Використайте ваш новий метод GetTransactionById " +
            ////    "(або GetTransactionByHash, залежно від того, що ви реалізували), " +
            ////    "щоб знайти цю зламану транзакцію в мережі. Виведіть її суму. " +
            ////    "Очікується: 999999.")

            //var foundTx = lastBlock.Transactions[1]; ;
            //if (foundTx != null)
            //{
            //    Console.WriteLine($"Знайдено транзакцію: {foundTx.Id}");
            //    Console.WriteLine($"СУМА: {foundTx.Amount} монет");
            //    Console.WriteLine($"Очікувана сума після атаки: 999999");
            //}
            //Console.WriteLine();
            //Console.WriteLine();
            //// Перевірка 2 (Лавинний ефект Меркла):
            //// Знову викличте GetMerkleRoot для транзакцій
            //// цього ж блоку і виведіть новий результат поруч
            //// із оригінальним. Очікується: Два абсолютно різні 7
            //// 64-символьні хеші (доводить, що зміна однієї цифри
            //// повністю переписала корінь).
            //string newMerkleRoot = hashingService.GetMerkleRoot(lastBlock.Transactions);
            //Console.WriteLine($"Оригінальний корінь: {originalMerkleRoot}");
            //Console.WriteLine($"Новий корінь:      {newMerkleRoot}");
            //Console.WriteLine($"Корені різні: {originalMerkleRoot != newMerkleRoot}");
            //Console.WriteLine();
            //Console.WriteLine();

            //// Перевірка 3 (Глобальна безпека):
            //// Викличте загальний метод валідації всієї
            ////Очікується: False (Мережа виявила підробку і зупинила роботу).
            //bool isValid = blockChain.isValid(blockChain.Chain);
            //Console.WriteLine($"Результат валідації блокчейну: {isValid}");

            //if (!isValid)
            //{
            //    Console.WriteLine("Мережа виявила підробку!" +
            //        " Блокчейн невалідний через зміну Кореня Меркла.");
            //}
            //else
            //{
            //    Console.WriteLine("Мережа не виявила підробку!");
            //}
            //Console.WriteLine();
            //Console.WriteLine();


            //Console.WriteLine($"Перевірка 1 (Block Explorer): Сума = {foundTx.Amount} (очікується 999999)");
            //Console.WriteLine($"Перевірка 2 (Лавинний ефект): Корені різні = {originalMerkleRoot != newMerkleRoot}");
            //Console.WriteLine($"Перевірка 3 (Глобальна безпека): Блокчейн валідний = {isValid}");
            //Console.WriteLine();
            //Console.WriteLine();




            //// Перевірка балансу після майнингу
            //Console.WriteLine($"Bob wallet balance: {blockChain.GetBalance(bobeWallet.Address)}");
            //Console.WriteLine($"Alice wallet balance: {blockChain.GetBalance(aliceWallet.Address)}");

            //displayService.printBlockChain(blockChain.Chain);

           
           






            //var nodeA = new BlockChain(1);
            //var nodeB = new BlockChain(1);

            //var satoshi = walletService.CreateWallet("Satoshi"); 
            //var vitalik = walletService.CreateWallet("Vitalik"); 


            //nodeA.AddTransaction(new Transaction
            //{
            //    From = "COINBASE",
            //    To = satoshi.Address,
            //    Amount = 50,
            //    Fee = 0,
            //    Timestamp = DateTime.UtcNow
            //});
            //nodeA.MinePendingTransaction(satoshi, 10);


            //nodeA.AddTransaction(new Transaction
            //{
            //    From = "COINBASE",
            //    To = satoshi.Address,
            //    Amount = 50,
            //    Fee = 0,
            //    Timestamp = DateTime.UtcNow
            //});
            //nodeA.MinePendingTransaction(satoshi, 10);


            //for (int i = 0; i < 4; i++)
            //{
            //    nodeB.AddTransaction(new Transaction
            //    {
            //        From = "COINBASE",
            //        To = vitalik.Address,
            //        Amount = 50,
            //        Fee = 0,
            //        Timestamp = DateTime.UtcNow
            //    });
            //    nodeB.MinePendingTransaction(vitalik, 10);
            //}







            //bool replaced = nodeA.ReplaceChain(nodeB.Chain);

            //// Перевірка 1 (Перемога найдовшого ланцюга):
            //// Вивести поточну кількість блоків у nodeA.Chain.Count.
            //// Очікується: 5.
            //Console.WriteLine($"Кількість блоків в nodeA: {nodeA.Chain.Count} - очікується 5");


            //// Перевірка 2(Економічна справедливість State):
            //// Вивести баланси Satoshi та Vitalik на nodeA,
            //// звертаючись безпосередньо до миттєвого словника nodeA.State[...].
            //// Очікується: Satoshi = 0(його блоки відкинуто мережею),
            //// Vitalik = 200(4 нагороди по 50). (Підказка: перевірте через ContainsKey,
            //// щоб не отримати помилку, якщо Satoshi взагалі зник зі словника).

            //decimal satoshBalance = nodeA.Balances.ContainsKey(satoshi.Address) ? nodeA.Balances[satoshi.Address] : 0;
            //decimal vitalikBalance = nodeA.Balances.ContainsKey(vitalik.Address) ? nodeA.Balances[vitalik.Address] : 0;

            //Console.WriteLine($"Баланс Satoshi: {satoshBalance}  -  очікую 0");
            //Console.WriteLine($"Баланс Vitalik: {vitalikBalance}  -  очікую 200");

            //// Перевірка 3(Синхронізація диска): Вивести кількість рядків у
            //// локальному файлі сховища blocks.dat через File.ReadLines("blocks.dat").
            //// Count().Очікується: 5.
            //storageFile = "blockchain_data.dat";
            //int lineCount = File.Exists(storageFile)
            //    ? File.ReadAllLines(storageFile).Length
            //    : 0;

            //Console.WriteLine($"Кількість рядків у файлі blocks.dat: {lineCount} - очікується 5");




            //blockChain.AddTransaction(new Transaction
            //{
            //    From = "COINBASE",
            //    To = miner.Address,
            //    Amount = 100,
            //    Fee = 0,
            //    Timestamp = DateTime.UtcNow
            //});


            //blockChain.MinePendingTransaction(miner, 10);
            //blockChain.MinePendingTransaction(miner, 10);


            //var transferTx = transactionService.CreateTransaction(miner, alice.Address, 20, 1.0m);
            //blockChain.AddTransaction(transferTx);

            //blockChain.MinePendingTransaction(miner, 10);

            //Console.WriteLine($"Кількість блоків у Chain: {blockChain.Chain.Count}");
            //Console.WriteLine($"Баланс Аліси: {blockChain.GetBalance(alice.Address)} монет.");
            //Console.WriteLine();
            //Console.WriteLine();

            //var restored = new BlockChain(1);
            //restored.Chain.Clear();
            //restored.Balances.Clear();
            //restored.LoadChainFromFile();




            //int lineCount = File.ReadAllLines(storageFile).Length;
            //Console.WriteLine($"(Цілісність файлу): Вивести кількість рядків у файлі blocks.dat " +
            //    $"{lineCount}  - повинно бути 5");

            //Console.WriteLine($"Баланс Аліси: {restored.Balances[alice.Address]} - повинно бути 20 монет");

            //Console.WriteLine($"Очищення мемпулу: {restored.GetPendingTransactionCount()} - повинно бути 0 монет");


            //if (restored.Chain.Count > 0)
            //{
            //    decimal aliceBalance = restored.Balances.ContainsKey(alice.Address)
            //        ? restored.Balances[alice.Address]
            //        : 0;
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine($"Блокчейн відновлено! Баланс Аліси: {aliceBalance} монет");
            //    Console.ForegroundColor = ConsoleColor.White;
            //}

            //displayService.printBlockChain(blockChain.Chain);

            //var initialBalance = new Transaction
            //{
            //    From = "COINBASE",
            //    To = aliceWallet.Address,
            //    Amount = 7000,
            //    Fee = 0,
            //    Timestamp = DateTime.UtcNow
            //};

            //blockChain.AddTransaction(initialBalance);
            //blockChain.MinePendingTransaction(aliceWallet, 1);
            //displayService.PrintBlock(blockChain.Chain.Last());


            //Console.WriteLine($"Баланс Аліси:  {blockChain.GetBalance(aliceWallet.Address)} монет");
            //Console.WriteLine("-----------------------------");



            ////У мемпулі (_pendingTransactions) чекають 5 переказів:
            //Console.WriteLine($"У мемпулі (_pendingTransactions) чекають 5 переказів: ");
            //var tx1 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 1000, 0.5m);
            //var tx2 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 10, 2.0m);
            //var tx3= transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 5000, 0.0m);
            //var tx4 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 5, 5.0m);
            //var tx5 = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, 100, 1.0m);

            //Console.WriteLine($"Перевіряємо баланс Аліси перед додаванням транзакцій:  {blockChain.GetBalance(aliceWallet.Address)} монет");

            //blockChain.AddTransaction(tx1);
            //blockChain.AddTransaction(tx2);
            //blockChain.AddTransaction(tx3);
            //blockChain.AddTransaction(tx4);
            //blockChain.AddTransaction(tx5);

            //Console.WriteLine("Шукаємо 3 транзакції де найбільша комісія");
            //blockChain.MinePendingTransaction(aliceWallet, 3);

            //Console.WriteLine($"Баланс Аліси: {blockChain.GetBalance(aliceWallet.Address)} монет");
            //Console.WriteLine($"Баланс Боба: {blockChain.GetBalance(bobeWallet.Address)} монет");
            //displayService.PrintBlock(blockChain.Chain.Last());


            //Console.WriteLine($"1. В блок потрапили транзакції: TX4 (Fee:5.0), TX2 (Fee:2.0), TX5 (Fee:1.0)");
            //Console.WriteLine($"2. Винагорода майнера: 58 монет (50 + 5.0 + 2.0 + 1.0)");
            //Console.WriteLine($"3. TX3 (Fee:0.0) залишилась в мемпулі. Шанс потрапити в блок є, коли не буде транзакцій з вищою комісією");

            //Console.WriteLine();           
            //Console.WriteLine($"===============================================================");


            //// 1. Сценарій 1 (Атака "Гроші з повітря"):
            //// Створити порожній гаманець і спробувати
            //// відправити 100 монет. Система повинна відмовити.
            //Console.WriteLine("Створено пустий гаманець");
            //var emptyWallet = walletService.CreateWallet("0000");
            //Console.WriteLine("спробувати відправити 100 монет");

            //var fakeTransaction = transactionService.CreateTransaction(emptyWallet, bobeWallet.Address, 100, 0.01m);
            //var result = blockChain.AddTransaction(fakeTransaction);

            //if (result)
            //{
            //    Console.WriteLine("Система повинна відмовити в транзакції");
            //}

            ////Сценарій 2 (Атака "Фейковий блок"):
            ////Спробувати додати в Chain блок з неправильним хешем.
            ////Система повинна відмовити (або IsValid() має
            ////повернути false).
            //Console.WriteLine("Створено фейковий блок");
            //var fakeBlock = new Block(600, new List<Transaction>(), "0000", "Anna", 1);
            //fakeBlock.Hash = "0000";

            //blockChain.Chain.Add(fakeBlock);

            //bool isValid = blockChain.isValid();

            //if (!isValid)
            //{
            //    Console.WriteLine("Система повинна відмовити в додаванні блоку");
            //    blockChain.Chain.Remove(fakeBlock);
            //}

            //// Сценарій 3 (Легальна операція):
            //// Намайнити блок, отримати винагороду (COINBASE)
            //// і успішно переказати частину коштів іншому
            //// користувачу. Система повинна дозволити.
            //var legTransaction = new Transaction
            //{
            //    From = "COINBASE",
            //    To = bobeWallet.Address,
            //    Amount = 10,
            //    Fee = 0,
            //    Timestamp = DateTime.Now
            //};
            //blockChain.AddTransaction(legTransaction);
            //blockChain.MinePendingTransaction(aliceWallet, 10);
            //Console.WriteLine($"Аліса отримала 10 монет {blockChain.GetBalance(aliceWallet.Address)}");

            //bool legResult = blockChain.AddTransaction(legTransaction);

            //if (legResult)
            //{
            //    Console.WriteLine("Транзакцію додано в mtmpool");
            //    blockChain.MinePendingTransaction(aliceWallet, 10);
            //    Console.WriteLine($"Баланс Аліси  {blockChain.GetBalance(aliceWallet.Address)} монет");
            //    Console.WriteLine($"Баланс Боба {blockChain.GetBalance(bobeWallet.Address)} монет");
            //    Console.WriteLine("Легальна операція");
            //}






            //p2pService.StartServer(port);

            //if (args.Length > 1)
            //{
            //    int peerPort = int.Parse(args[1]);
            //    p2pService.ConnectToPeer("127.0.0.1", peerPort);
            //}


            //while (true)
            //{
            //    Console.WriteLine($"Нода порт {port}");
            //    Console.WriteLine($"==============================");
            //    Console.WriteLine($"1. Створити транзакцію");
            //    Console.WriteLine($"2. Майнити блок");
            //    Console.WriteLine($"3. Показати блокчейн");
            //    Console.WriteLine($"4. Підключитися до іншої ноди вручну");
            //    Console.WriteLine($"5. Перевірти валідацію блокчейну");
            //    Console.WriteLine($"Оберіть дію:");

            //    string choice = Console.ReadLine();

            //    switch (choice)
            //    {
            //        case "1":
            //            Console.WriteLine("Введіть суму: ");
            //            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            //            {
            //                var transaction = transactionService.CreateTransaction(aliceWallet, bobeWallet.Address, amount, 0.01m);
            //                if (blockChain.AddTransaction(transaction))
            //                {
            //                    Console.WriteLine("Транзакція додана до черги.");
            //                    p2pService.BroadCast(MessageType.BroadcastTransaction, transaction);
            //                }
            //                else
            //                {
            //                    Console.WriteLine("Помилка при додаванні транзакції.");
            //                }


            //            }
            //            else
            //            {
            //                Console.WriteLine("Невірна сума.");
            //            }
            //            break;

            //        case "2":
            //            Console.WriteLine("Майнінг блоку ...");
            //            blockChain.MinePendingTransaction(aliceWallet, 5);
            //            var latestBlock = blockChain.Chain.Last();
            //            p2pService.BroadCast(MessageType.BroadcastBlock, latestBlock);
            //            break;

            //        case "3":
            //            displayService.printBlockChain(blockChain.Chain);
            //            break;

            //        case "4":
            //            Console.WriteLine("Введіть порт іншої ноди: ");
            //            if (int.TryParse(Console.ReadLine(), out int peerPort))
            //            {
            //                p2pService.ConnectToPeer("127.0.0.1", peerPort);
            //            }
            //            break;

            //        case "5":
            //            //bool
            //            isValid = blockChain.isValid(blockChain.Chain);

            //            Console.WriteLine(isValid ? "Блокчейн валідний." : "Блокчейн не валідний!");

            //            break;
            //    }
            //}

           

            //displayService.printBlockChain(blockChain.Chain);





            // var blockChain = new BlockChain(1);

            // // Створюємо сервіс для відображення блокчейну
            // var displayService = new BlockChainDisplayService();


            // blockChain.AddBlock("Alisa send 10 BTC to Bob", "Anna");
            // blockChain.AddBlock("Bob send 10 BTC to Charli", "Eva");
            // blockChain.AddBlock("Charli send 100 BTC tDave", "Victor");
            // blockChain.AddBlock("Dave send 1 BTC to Eva", "Vlad");

            // // Показуємо блокчейн
            // displayService.printBlockChain(blockChain.Chain);

            // // Перевіряємо валідність блокчейну
            // bool isValid = blockChain.isValid();
            // displayService.PrintValidationResult(isValid);


            // //Змінемо дані в одному блоків для демонстрації невалідності імітації атаки  
            // blockChain.Chain[2].Data = "Bob send 100 BTC to Charli"; // зміна даних в 3-му блоці
            //// displayService.PrintValidationResult(isValid);

            // //Показуємо блокчейн
            // displayService.printBlockChain(blockChain.Chain);

            // //Перевірка валідності блокчейну
            // isValid = blockChain.isValid();
            // displayService.PrintValidationResult(isValid);




            //--------------------------------------------

            //// Бенчмарк майнінгу
            //var sw = System.Diagnostics.Stopwatch.StartNew();
            ////Створюємо блокчейн і сервіс для відображення 
            //var testBlockChain = new BlockChain(1);



            //var displayService = new BlockChainDisplayService();


            //var walletService = new WalletService();
            //Console.WriteLine("Створюємо гаманець для ...");
            //var name = Console.ReadLine();
            //var wallet1 = walletService.CreateWallet(name);
            //Console.WriteLine("Private key Please Save And Removee");
            //Console.WriteLine(Convert.ToBase64String(wallet1.PrivateKey));

            //Console.WriteLine("Public key Please Save And Removee");
            //Console.WriteLine(Convert.ToBase64String(wallet1.PublicKey));

            //Console.WriteLine("Wallrt Address");
            //Console.WriteLine(Convert.ToString(wallet1.Address));

            //var transactionService = new TransactionService(walletService);

            //var wallet1 = walletService.CreateWallet("Alice");
            //var wallet2 = walletService.CreateWallet("Bob");

            //testBlockChain.MinePendingTransaction(wallet1, 2);
            //testBlockChain.MinePendingTransaction(wallet1, 2);

            //var transaction1 = transactionService.CreateTransaction(wallet1, wallet2.Address, 10, 0.1m);
            //var transaction2 = transactionService.CreateTransaction(wallet1, wallet2.Address, 10, 0.1m);
            //var transaction3 = transactionService.CreateTransaction(wallet1, wallet2.Address, 10, 0.1m);


            //testBlockChain.AddTransaction(transaction1);
            //testBlockChain.AddTransaction(transaction2);
            //testBlockChain.AddTransaction(transaction3);

            //testBlockChain.MinePendingTransaction(wallet2, 3);

            //sw.Stop();

            //long attempts = testBlockChain.Chain.Last().Nonce;

            //displayService.printBlockChain(testBlockChain.Chain);


            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Ivan");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));

            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Eva");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));

            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Ada");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));

            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Sasha");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));

            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Vlad");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));

            //testBlockChain.AddBlock($"Test block with difficulty {testBlockChain.Difficulty}", "Danilo");
            //displayService.printBlockChain(testBlockChain.Chain);
            //Console.WriteLine(new String('-', 40));



            // Створюємо сервіс для відображення блокчейну
            //var displayService = new BlockChainDisplayService();



            // // Додаємо блок для бенчмарку 
            // testBlockChain.AddBlock("Test block for benncharking", "Ivan");






            // sw.Stop();  // зупиняємо таймер 

            // // Отримуємо кількість спроб майніннгу з останнього блоку 
            // long attemps = testBlockChain.Chain.Last().Nonce;


            // Console.WriteLine("/////////////////");
            // // виводимо результат бенчмарку
            // //displayService.PrintBenchmarkResult(attemps, sw.Elapsed, testBlockChain.Difficulty);


            // Console.ForegroundColor = ConsoleColor.Yellow;
            // Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\/////////////////");
            // Console.WriteLine("Тестування транзакції");

            //// var walletService = new WalletService();
            // var transactionService = new TransactionService(walletService);

            // Console.WriteLine("Створюємо гаманець...");
            // var sender = walletService.CreateWallet("Alice");
            // var receiver = walletService.CreateWallet("Bob");

            // Console.WriteLine($"Sender Address: {sender.Address}");
            // Console.WriteLine($"Receiver Address: {receiver.Address}");

            // Console.WriteLine($"Створення транзакції...");
            // var transaction = transactionService.CreateTransaction(
            //     sender,
            //     receiver.Address,
            //     100.5m,
            //     0.5m,
            //     sender.PublicKey
            //  );


            // Console.WriteLine($"Деталі транзакції...");
            // transactionService.DisplayTransaction(transaction);


            // Console.WriteLine($"Перевірка підпису...");
            // bool signatureValid = transactionService.VerifySignature(transaction);
            // Console.WriteLine($"Підпис: {signatureValid}");


            // Console.WriteLine($"Перевірка валідності транзакції...");
            // bool isValidTransaction = transactionService.ValidateTransaction(transaction);
            // Console.WriteLine($"Транзакція: {isValidTransaction}");
            // Console.ResetColor();


        }
    }
}

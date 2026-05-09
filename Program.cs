using System.Security.Cryptography.X509Certificates;
using BlockChainApp.Models;
using BlockChainApp.Services;

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
            //if (File.Exists(storageFile)) File.Delete(storageFile);

            var walletService = new WalletService();
            var blockChain = new BlockChain(1);
            var transactionService = new TransactionService(walletService);

            var p2pService = new P2RService(blockChain);
            var displayService = new BlockChainDisplayService();






            var alice = walletService.CreateWallet("Alice");
            var miner = walletService.CreateWallet("Bob");
            //var myWallet = walletService.CreateWallet("Vlad");


            blockChain.AddTransaction(new Transaction
            {
                From = "COINBASE",
                To = miner.Address,
                Amount = 100,
                Fee = 0,
                Timestamp = DateTime.UtcNow
            });


            blockChain.MinePendingTransaction(miner, 10);
            blockChain.MinePendingTransaction(miner, 10);
            

            var transferTx = transactionService.CreateTransaction(miner, alice.Address, 20, 1.0m);
            blockChain.AddTransaction(transferTx);

            blockChain.MinePendingTransaction(miner, 10);

            Console.WriteLine($"Кількість блоків у Chain: {blockChain.Chain.Count}");
            Console.WriteLine($"Баланс Аліси: {blockChain.GetBalance(alice.Address)} монет.");
            Console.WriteLine();
            Console.WriteLine();

            var restored = new BlockChain(1);
            restored.Chain.Clear();
            restored.Balances.Clear();
            restored.LoadChainFromFile();




            //int lineCount = File.ReadAllLines(storageFile).Length;
            //Console.WriteLine($"(Цілісність файлу): Вивести кількість рядків у файлі blocks.dat " +
            //    $"{lineCount}  - повинно бути 5");
            
            //Console.WriteLine($"Баланс Аліси: {restored.Balances[alice.Address]} - повинно бути 20 монет");

            //Console.WriteLine($"Очищення мемпулу: {restored.GetPendingTransactionCount()} - повинно бути 0 монет");

           
            if (restored.Chain.Count > 0)
            {
                decimal aliceBalance = restored.Balances.ContainsKey(alice.Address)
                    ? restored.Balances[alice.Address]
                    : 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Блокчейн відновлено! Баланс Аліси: {aliceBalance} монет");
                Console.ForegroundColor = ConsoleColor.White;
            }

            displayService.printBlockChain(blockChain.Chain);

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

            //    p2pService.StartServer(port);

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
            //            isValid = blockChain.isValid();

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

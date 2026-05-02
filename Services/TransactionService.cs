using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlockChainApp.Models;

namespace BlockChainApp.Services
{
    public class TransactionService
    {
        private readonly WalletService walletService;

        public TransactionService(WalletService walletService)
        {
            this.walletService = walletService;

        }

        //Створення тре=анзакції 
        public Transaction CreateTransaction(Wallet wallet, string to, decimal amountn, decimal fee)
        {
            // створюємо транзакцію
            var transaction = new Transaction(wallet.Address, to, amountn, fee, wallet.PublicKey);
            
            // отримуємо дані для підпису (БФЙТИБ, не хеш !!!)
            byte[] dateToSing = transaction.GetDataSing();


            // підписуємо ці дані приватним ключем
            using var ecdsa = System.Security.Cryptography.ECDsa.Create();
            ecdsa.ImportECPrivateKey(wallet.PrivateKey, out _);
            

            //ECDsa САМА обчислє хеш і піжписує !
            transaction.Signature = ecdsa.SignData(dateToSing, HashAlgorithmName.SHA256);
                    //                     ↑
                    //                     Метод SignData:

                    //                      1. Бере dateToSing(байти)
                    //                      2. обчислює SHA256 хеш 
                    //                      3. підписує хеш закритим ключем 
                    //                      4. повертає підпис
            return transaction;
        }

        // метод для перевірки підпису  транзакції
        public bool VerifySignature(Transaction transaction)
        {
            if (transaction == null || transaction.Signature == null || transaction.PublicKey == null)
                    return false;


            try
            {   // отримуємо ті самі дані 
                byte[] dataToVerify = transaction.GetDataSing(); // <- ті ж самі байти
                
                // використовуємо публічний ключ для перевірки 
                using var ecdsa = ECDsa.Create();

                // Імпортуємо публічний ключ відправника з транзакції
                // Цей ключ був збережений у транзакції при її створенні
                // Формат SubjectPublicKeyInfo - стандартний X.509 формат
                // out _ - ігноруємо кількість прочитаних байт (нам це не потрібно)
                ecdsa.ImportSubjectPublicKeyInfo(transaction.PublicKey, out _);


                // VerifyData сама обчислює хеш і перевіряє підпис
                return ecdsa.VerifyData(dataToVerify, transaction.Signature, HashAlgorithmName.SHA256);
                        //     ↑
                        //     Метод VerifyData:
                        //     1. Бере dataToVerify (байти)
                        //     2. Обчислює SHA256 хеш
                        //     3. Перевіряє, чи підпис відповідає хешу та публічному ключу
            }
            catch
            {
                return false;
            }
        }



        // метод для валідації транзакції 
        public bool ValidateTransaction(Transaction transaction)
        {
            if (transaction == null )
               return false;

            if (transaction.Amount <= 0)
                return false;

            if (transaction.Fee < 0)
                return false;

            if (string.IsNullOrEmpty(transaction.From) || string.IsNullOrEmpty(transaction.To))
                return false;

            if (transaction.From == transaction.To)
                return false;

            if (!VerifySignature(transaction))
                return false;

            return true;
        }



        // метод ля виведення інформації про транзацію 
        public void DisplayTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                Console.WriteLine("Transaction is null");
                return;
            }


            Console.WriteLine($"Transaction: ");
            Console.WriteLine($"From: {transaction.From} ");
            Console.WriteLine($"To: {transaction.To} ");
            Console.WriteLine($"Amount: {transaction.Amount} ");
            Console.WriteLine($"Fee: {transaction.Fee}");
            Console.WriteLine($"Time: {transaction.Timestamp} ");
            Console.WriteLine($"Valid: {ValidateTransaction(transaction)} ");



        }




    }
}
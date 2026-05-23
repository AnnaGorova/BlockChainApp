using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChainApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string From { get; set; } // адреса відправника, зазвичай публічний ключ 
        public string To { get; set; } // адреса отримувача (куди йдуть кошти)

        // decimal для точності фінансових операцій  28-29 цифр
        public decimal Amount { get; set; } = 0; // сума перекузу в криптовалюті, decimal 
        public decimal Fee { get; set; } // коміся за транзакція, виплачується майнерву за включення транзакції в блок
        public DateTime Timestamp { get; set; } = DateTime.Now; // час створення транзакції - автоматично
        public byte[] Signature { get; set; } // цифровий підпис транзакції, створюється закритим привтним ключем відправника,
        //гарантує що транзакція не буда змінена. 
        
        

        // публічний ключ відправника, використовується для перевірки підпису транзакції
        public byte[] PublicKey { get; set; }

        public Transaction() { }
        public Transaction(string from, string to, decimal amount, decimal fee, byte[] publicKey)
        {
            From = from;
            To = to;
            Amount = amount;
            Fee = fee;
            PublicKey = publicKey;
            Timestamp = DateTime.Now;
        }

        // Отримуємо дані для підпису транзакції
        // формуємо рядок з усіх важливих полів транзакції 
        // саме ці дані будуть підписані закритим ключем (приватиним ключем)
        public byte[] GetDataSing()
        {
            string data = $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("o")}";
           
            // перетворюємо у масив байтів 
            return Encoding.UTF8.GetBytes(data);
        }


        //  повертаємо транзакцію у рядок представлення
        // виккорисстовується для зберігання логування та створення хешу
        public string ToRawString()
        {   
            // перетворюємо підпис у з байтів у HEX - рядок для зручного відображення 
            // якщо підпис відсутній виводимо null
            string hexSignaturer = Signature != null ? BitConverter.ToString(Signature).Replace("-", "") : "null";
           
            // повертаємо повний рядок з усіма полями транзакції 
            return $"{From}:{To}:{Amount}:{Fee}:{Timestamp}:{hexSignaturer}";
        }


    }
}

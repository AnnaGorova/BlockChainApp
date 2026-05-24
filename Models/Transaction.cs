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

        public string Memo { get; set; } = string.Empty;

        public Transaction() { }
        public Transaction(string from, string to, decimal amount, decimal fee, byte[] publicKey, string memo = "")
        {
            From = from;
            To = to;
            Amount = amount;
            Fee = fee;
            PublicKey = publicKey;
            Memo = memo ?? string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        // Отримуємо дані для підпису транзакції
        // формуємо рядок з усіх важливих полів транзакції 
        // саме ці дані будуть підписані закритим ключем (приватиним ключем)
        public byte[] GetDataSing()
        {
            // string data = $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("o")}";
            //string data = $"{From}:{To}:{Amount}:{Fee}:{Timestamp.Ticks}:{Memo}";

            string data = $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("O")}:{Memo}";
            return Encoding.UTF8.GetBytes(data);
            // перетворюємо у масив байтів 
            
        }


        //  повертаємо транзакцію у рядок представлення
        // виккорисстовується для зберігання логування та створення хешу
        //public string ToRawString()
        //{   
        //    // перетворюємо підпис у з байтів у HEX - рядок для зручного відображення 
        //    // якщо підпис відсутній виводимо null
        //    string hexSignaturer = Signature != null ? BitConverter.ToString(Signature).Replace("-", "") : "null";

        //    // повертаємо повний рядок з усіма полями транзакції 
        //    return $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("O")}:{Memo}:{hexSignaturer}";
        //}

        //public string ToRawString()
        //{
        //    string hexSignature = Signature != null ? BitConverter.ToString(Signature).Replace("-", "") : "null";
        //    // Memo в кінці, Timestamp.ToString("O") з великої O !

        //    return $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("O")}:{hexSignature}:{Memo}";
        //}

        public string ToRawString()
        {
            string hexSignature = Signature != null
                ? BitConverter.ToString(Signature).Replace("-", "")
                : "null";
            return $"{From}:{To}:{Amount}:{Fee}:{Timestamp.ToString("o")}:{hexSignature}:{Memo}";
        }


        public string CalculateHash()
        {
            string data = $"{From}{To}{Amount}{Fee}{Timestamp.Ticks}{Memo}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}

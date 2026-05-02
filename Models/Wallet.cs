using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockChainApp.Models
{
    public class Wallet
    {
        public string Name { get; set; }

        public string Address { get; set; }  // це зазвичай хеш від публічного ключа 

        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey {get; set;}

        public Wallet(string name, string address, byte[] publicKey, byte[] privateKey)
        {
            Name = name;
            Address = address;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }




        // ========== КРИПТОГРАФІЧНІ МЕТОДИ ==========

        /// <summary>
        /// Підписує дані закритим (приватним) ключем
        /// Використовує алгоритм ECDsa (Elliptic Curve Digital Signature Algorithm)
        /// 
        /// Процес підпису:
        /// 1. Імпортуємо приватний ключ
        /// 2. Обчислюємо хеш даних (SHA256)
        /// 3. Підписуємо хеш закритим ключем
        /// 4. Повертаємо цифровий підпис
        /// </summary>
        /// <param name="data">Дані для підпису (зазвичай GetDataSing() з Transaction)</param>
        /// <returns>Цифровий підпис у вигляді масиву байтів</returns>
        /// 
        /// 🔒 Важливо: цей метод використовує ПРИВАТНИЙ ключ
        /// Викликати тільки коли потрібно підписати транзакцію
        public byte[] Sign(byte[] data)
        {
            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportECPrivateKey(PrivateKey, out _);
                return ecdsa.SignData(data, HashAlgorithmName.SHA256);
            }
        }
        
        
    }
}

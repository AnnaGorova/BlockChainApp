using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlockChainApp.Models;

namespace BlockChainApp.Services
{
    /// Сервіс для роботи з криптографічними гаманцями
    /// Відповідає за створення гаманців та перевірку цифрових підписів
    public class WalletService
    {
        /// Створює новий криптографічний гаманець з унікальною парою ключів
        /// Використовує алгоритм еліптичних кривих ECDsa (nistP256)
        /// 
        /// Процес створення:
        /// 1. Генеруємо криптографічну пару ключів (приватний + публічний)
        /// 2. Експортуємо ключі у стандартному форматі PKCS8 та SubjectPublicKeyInfo
        /// 3. Генеруємо адресу гаманця з публічного ключа (Base64)
        /// 4. Створюємо об'єкт Wallet з отриманими даними
        /// </summary>
        /// <param name="name">Ім'я власника гаманця (для зручності ідентифікації)</param>
        /// <returns>Об'єкт Wallet, що містить публічний та приватний ключі</returns>
        /// 
        /// 🔒 Важливо: Приватний ключ генерується випадковим чином і зберігається в об'єкті Wallet
        /// 🔒 Ніколи не зберігайте приватні ключі у відкритому вигляді в реальному застосунку!
        public Wallet CreateWallet(string name)
        {
            // Створюємо генератор ключів на основі еліптичної кривої nistP256 (secp256r1)
            // Це стандартна крива, яка використовується в багатьох блокчейнах
            using var ecdsa = System.Security.Cryptography.ECDsa.Create(ECCurve.NamedCurves.nistP256);


            // Експортуємо приватний ключ у форматі PKCS8
            // PKCS8 - це стандартний формат для зберігання приватних ключів
            // Він містить інформацію про алгоритм та сам ключ
            byte[] privateKey = ecdsa.ExportECPrivateKey();


            // Експортуємо публічний ключ у форматі SubjectPublicKeyInfo
            // Це стандартний формат для обміну публічними ключами
            // Містить інформацію про алгоритм та сам ключ
            byte[] publicKey = ecdsa.ExportSubjectPublicKeyInfo();


            // Генеруємо адресу гаманця з публічного ключа
            // Використовуємо Base64 для зручного відображення та передачі
            string address = Convert.ToBase64String(publicKey);


            // Повертаємо новий об'єкт Wallet з усіма даними
            return new Wallet(name, address, publicKey, privateKey);
        }




        // ========== МЕТОДИ ПЕРЕВІРКИ ПІДПИСІВ ==========

        /// <summary>
        /// Перевіряє цифровий підпис даних за допомогою публічного ключа
        /// Використовується для верифікації, що дані дійсно підписані власником приватного ключа
        /// 
        /// Процес перевірки:
        /// 1. Імпортуємо публічний ключ
        /// 2. Перевіряємо, чи відповідає підпис даним та ключу
        /// 3. Повертаємо результат перевірки
        /// </summary>
        /// <param name="data">Дані, які були підписані (оригінальні дані)</param>
        /// <param name="sighature">Цифровий підпис для перевірки</param>
        /// <param name="publicKey">Публічний ключ відправника</param>
        /// <returns>
        /// True - якщо підпис валідний (дані справжні та не змінені)
        /// False - якщо підпис невірний або сталася помилка
        /// </returns>
        public bool VeryfiSignature(byte[] data, byte[] sighature, byte[] publicKey)
        {
            try
            {
                // Створюємо екземпляр ECDsa для перевірки
                using var ecdsa = ECDsa.Create();

                // Імпортуємо публічний ключ у форматі SubjectPublicKeyInfo
                // Цей формат використовувався при створенні гаманця
                ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _); // out _ - це discard (відкидання) в C#. Воно означає, що метод повертає значення, але воно нам не потрібне, тому ми його ігноруємо.


                // Перевіряємо підпис:
                // 1. VerifyData автоматично обчислює SHA256 хеш даних
                // 2. Порівнює обчислений хеш з тим, що в підписі
                // 3. Використовує публічний ключ для верифікації
                return ecdsa.VerifyData(data, sighature, HashAlgorithmName.SHA256);
            }
            catch(Exception)
            {
                // Повертаємо false - підпис невірний
                return false;
            }
        }



       

    }
}

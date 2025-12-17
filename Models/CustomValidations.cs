using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EventDeneme.Models
{
    // Kart Numarası Validasyonu - Luhn Algoritması
    public class CardNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string cardNumber = value.ToString().Replace(" ", "").Replace("-", "");

            // Sadece sayı olmalı
            if (!Regex.IsMatch(cardNumber, @"^\d+$"))
                return false;

            // 13-19 haneli olmalı
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Luhn algoritması kontrolü
            return IsValidLuhn(cardNumber);
        }

        private bool IsValidLuhn(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            // Sağdan sola doğru işle
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }

        public override string FormatErrorMessage(string name)
        {
            return "Invalid card number. Please enter a valid 13-19 digit card number.";
        }
    }

    // CVV Validasyonu - 3-4 haneli sayı
    public class CVVAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string cvv = value.ToString().Trim();

            // Sadece sayı olmalı
            if (!Regex.IsMatch(cvv, @"^\d+$"))
                return false;

            // 3 veya 4 haneli olmalı
            return cvv.Length == 3 || cvv.Length == 4;
        }

        public override string FormatErrorMessage(string name)
        {
            return "CVV must be 3 or 4 digits.";
        }
    }

    // Expiry Date Validasyonu - MM/YY formatı ve geçmiş tarih kontrolü
    public class ExpiryDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string expiryDate = value.ToString().Trim();

            // MM/YY formatı kontrolü
            if (!Regex.IsMatch(expiryDate, @"^\d{2}/\d{2}$"))
                return false;

            try
            {
                string[] parts = expiryDate.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]);

                // Ay 1-12 arasında olmalı
                if (month < 1 || month > 12)
                    return false;

                // Yıl kontrolü (20XX formatında)
                int fullYear = 2000 + year;
                DateTime expiry = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month));

                // Geçmiş tarih kontrolü
                if (expiry < DateTime.Now.Date)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return "Invalid expiry date. Please enter in MM/YY format and ensure the date is not expired.";
        }
    }

    // Telefon Numarası Validasyonu
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string phone = value.ToString().Trim();

            // Boşluk, tire, parantez gibi karakterleri temizle
            phone = Regex.Replace(phone, @"[\s\-\(\)]", "");

            // Türkiye telefon formatları: +90 veya 0 ile başlayabilir
            // Örnek: +905551234567, 05551234567, 5551234567
            if (phone.StartsWith("+90"))
                phone = phone.Substring(3);
            else if (phone.StartsWith("0"))
                phone = phone.Substring(1);

            // Sadece sayı olmalı ve 10 haneli olmalı
            if (!Regex.IsMatch(phone, @"^\d{10}$"))
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Invalid phone number. Please enter a valid 10-digit phone number (e.g., 05551234567 or +905551234567).";
        }
    }

    // Tam İsim Validasyonu - En az 2 kelime ve sadece harf
    public class FullNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string fullName = value.ToString().Trim();

            // En az 2 kelime olmalı
            string[] words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            // Her kelime sadece harf içermeli (Türkçe karakterler dahil)
            foreach (string word in words)
            {
                if (!Regex.IsMatch(word, @"^[a-zA-ZçğıöşüÇĞIİÖŞÜ]+$"))
                    return false;

                // Her kelime en az 2 karakter olmalı
                if (word.Length < 2)
                    return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Full name must contain at least 2 words with letters only (e.g., 'John Doe').";
        }
    }
}


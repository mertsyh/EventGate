using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EventDeneme.Models
{
    
    public class CardNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string cardNumber = value.ToString().Replace(" ", "").Replace("-", "");

            
            if (!Regex.IsMatch(cardNumber, @"^\d+$"))
                return false;

            
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            
            return IsValidLuhn(cardNumber);
        }

        private bool IsValidLuhn(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            
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

    
    public class CVVAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string cvv = value.ToString().Trim();

            
            if (!Regex.IsMatch(cvv, @"^\d+$"))
                return false;

            
            return cvv.Length == 3 || cvv.Length == 4;
        }

        public override string FormatErrorMessage(string name)
        {
            return "CVV must be 3 or 4 digits.";
        }
    }

    
    public class ExpiryDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string expiryDate = value.ToString().Trim();

            
            if (!Regex.IsMatch(expiryDate, @"^\d{2}/\d{2}$"))
                return false;

            try
            {
                string[] parts = expiryDate.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]);

                
                if (month < 1 || month > 12)
                    return false;

                
                int fullYear = 2000 + year;
                DateTime expiry = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month));

                
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

    
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string phone = value.ToString().Trim();

            
            phone = Regex.Replace(phone, @"[\s\-\(\)]", "");

            
            
            if (phone.StartsWith("+90"))
                phone = phone.Substring(3);
            else if (phone.StartsWith("0"))
                phone = phone.Substring(1);

            
            if (!Regex.IsMatch(phone, @"^\d{10}$"))
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Invalid phone number. Please enter a valid 10-digit phone number (e.g., 05551234567 or +905551234567).";
        }
    }

    
    public class FullNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            string fullName = value.ToString().Trim();

            
            string[] words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            
            foreach (string word in words)
            {
                if (!Regex.IsMatch(word, @"^[a-zA-ZçğıöşüÇĞIİÖŞÜ]+$"))
                    return false;

                
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




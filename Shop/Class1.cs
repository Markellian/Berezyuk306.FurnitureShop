using System;
using System.Text.RegularExpressions;

namespace ClassLibrary1
{
    public class Captcha
    {
        public readonly string CaptchaString;
        private const string StringForCapcha = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
        public Captcha()
        {
            Random random = new Random();
            for (int x = 0; x < 4; x++) CaptchaString += StringForCapcha[random.Next(0, StringForCapcha.Length - 1)];
        }
    }

    public class Password
    {
        public readonly string Pas;
        public Password(string password)
        {
            this.Pas = password;
        }
        public string CheckPassword()
        {
            string misstake = "";
            if (Pas == "") misstake = "Введите пароль";
            else if (Pas.Length < 6 || Pas.Length > 18) misstake = "Длана пароля должна быть не менее 6 и не более 18 символов";
            else if (!Regex.IsMatch(Pas, "[*|&{}+]")) misstake = "Пароль должен содеражать хотя бы один из символов: +|{}*";
            else if (!Regex.IsMatch(Pas, @"\d")) misstake = "Пароль должен содержать цифры";
            else if (Regex.IsMatch(Pas, @"(.)\1\1")) misstake = "Пароль не должен содеражть три подряд идущих символа";
            return misstake;
        }
    }
    public class Materials2
    {
        public string Articyl { get; set; }
        public string Name { get; set; }
        public int Kolichestvo { get; set; }
        public string Edinica_izmerenia_name { get; set; }
        public decimal Price { get; set; }
        public string Shipper_name { get; set; }
        public DateTime? DateShip { get; set; }
        public string QualityName { get; set; }
    }
}

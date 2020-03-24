using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using Image = System.Windows.Controls.Image;
using System.Windows.Threading;
using Microsoft.Win32;
using ClassLibrary1;

namespace Shop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> StringList { get; set; }
        private const string Error = "Ошибка!";
        User user;
        Captcha captcha;
        public MainWindow()
        {
            InitializeComponent();
            CapchaChange();
            AddInformationToEquipment();
        }
        private void MakeAuthVisible()
        {
            AuthGrid.Visibility = Visibility.Visible;
            CapchaChange();
            LoginTextBox.Text = "";
            PasswordPasswordBox.Password = "";
            user = new User();
        }
        private void CapchaChange()
        {
            CapchaTextBox.Text = "";
            captcha = new Captcha();
            CapchaLabel.Content = captcha.CaptchaString;
        }
        
        
        private void Authication_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text.ToString() == "") MessageBox.Show("Введите логин", Error);
            else if (PasswordPasswordBox.Password.ToString() == "") MessageBox.Show("Введите пароль", Error);
            else if (CapchaTextBox.Text.ToString() != captcha.CaptchaString) MessageBox.Show("Неправильной ввод капчи", Error);
            else
            {
                bool enter = false;
                using (var db2 = new FurnitureShopEntities())
                {                    
                    foreach (var log in from f in db2.User select f)
                    {
                        if (log.Login.ToString() == LoginTextBox.Text.ToString() && log.Password.ToString() == PasswordPasswordBox.Password.ToString())
                        {
                            enter = true;
                            user = log;
                            break;
                        }
                    };
                }
                if (enter)
                {
                    switch (user.RoleId.ToString())
                    {
                        case "1": CustomerGrid.Visibility = Visibility.Visible; break;
                        case "2": MasterGrid.Visibility = Visibility.Visible; break;
                        case "3": DirectorGrid.Visibility = Visibility.Visible; break;
                        case "4": DeputyDirectorGrid.Visibility = Visibility.Visible; break;
                        case "5": ManagerGrid.Visibility = Visibility.Visible; break;
                    }
                    AuthGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль", Error);
                    CapchaChange();
                }
            }
        }
        private void MakeHiddenGrid(Grid g)
        {
            g.Visibility = Visibility.Hidden;
        }
        private void GoToAuth(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MakeHiddenGrid((Grid)(button.Parent));
            MakeAuthVisible();
        }

        private void GoToRegistration(object sender, RoutedEventArgs e)
        {
            AuthGrid.Visibility = Visibility.Hidden;
            RegistrationGrid.Visibility = Visibility.Visible;
        }
        private byte[] ImageToByteArray(System.Drawing.Image i)
        {
            using (var ms = new MemoryStream())
            {
                i.Save(ms, i.RawFormat);
                return ms.ToArray();
            }
        }
        private void Registrate(object sender, RoutedEventArgs e)
        {
            string Pas = PasswordRegistrationPasswordBox.Password.ToString();
            if (LoginRegistrationTextBox.Text.ToString() == "") MessageBox.Show("Введите логин", Error);
            else if (Pas == "") MessageBox.Show("Введите пароль", Error);
            else if (Pas.Length < 6 || Pas.Length > 18) MessageBox.Show("Длана пароля должна быть не менее 6 и не более 18 символов", Error);
            else if (!Regex.IsMatch(Pas, "[*|&{}+]")) MessageBox.Show("Пароль должен содеражать хотя бы один из символов: +|{}*", Error);
            else if (!Regex.IsMatch(Pas, @"\d")) MessageBox.Show("Пароль должен содержать цифры", Error);
            else if (Regex.IsMatch(Pas, @"(.)\1\1")) MessageBox.Show("Пароль не должен содеражть три подряд идущих символа", Error);
            else if (Pas != PasswordRegistration2PasswordBox.Password.ToString()) MessageBox.Show("Пароли не совпадают", Error);
            else 
            {
                using (var d1 = new FurnitureShopEntities())
                {
                    bool enter = false;
                
                    foreach (var log in from f in d1.User select f)
                    {
                        if (LoginRegistrationTextBox.Text.ToString() == log.Login.ToString())
                        {
                            enter = true;
                            MessageBox.Show("Пользователь с таким логином уже существует", Error);
                            break;
                        }
                    }
                    if (!enter)
                    {
                        User user = new User()
                        {
                            Login = LoginRegistrationTextBox.Text.ToString(),
                            RoleId = 1,
                            Password = Pas          
                        };
                        if (FirstNameTextBox.Text.ToString() == "") user.FirstName = null; else user.FirstName = FirstNameTextBox.Text.ToString();
                        if (LastNameTextBox.Text.ToString() == "") user.LastName = null; else user.LastName = LastNameTextBox.Text.ToString();
                        if (SecondNameTextBox.Text.ToString() == "") user.SecondName = null; else user.SecondName = SecondNameTextBox.Text.ToString();
                        if (PhotoRegistarationTextBox.Text.ToString() != "")
                        try
                        {
                            user.Photo = ImageToByteArray(System.Drawing.Image.FromFile(PhotoRegistarationTextBox.Text.ToString()));
                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("Не удалось загрузить фотографию", Error);
                        }
                        d1.User.Add(user);
                        if (d1.SaveChanges() == 1) MessageBox.Show("Регистрация завершена успешно", Error); else MessageBox.Show("Регистрация не удалась", Error);

                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string regex = @"\.[(png)(jpg)]";
                if (Regex.IsMatch(openFileDialog.FileName, regex))
                {
                    PhotoRegistarationTextBox.Text = openFileDialog.FileName;
                    ImageBrush image = new ImageBrush
                    {

                        ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Relative))
                    };
                    PhotoRegistarationLabel.Background = image;
                    PhotoRegistarationLabel.Content = "";
                }
                else MessageBox.Show("Неверный файл изображения",Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DirectorGrid.Visibility = Visibility.Hidden;
            EquipmentAccountingGrid.Visibility = Visibility.Visible;
            AddInformationToEquipment();
            ShowEquipmentInformation();
        }

        private void GoToDirector(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MakeHiddenGrid((Grid)button.Parent);
            DirectorGrid.Visibility = Visibility.Visible;
        }
        private void SwitchingBetweenTabs (Grid gridMakeActive, Grid gridMakeInactive, Label labelMakeActive, Label labelMakeInactive)
        {
            if (gridMakeActive.Visibility == Visibility.Hidden)
            {
                gridMakeActive.Visibility = Visibility.Visible;
                gridMakeInactive.Visibility = Visibility.Hidden;
                labelMakeActive.Background = System.Windows.Media.Brushes.LightGray;
                labelMakeInactive.Background = System.Windows.Media.Brushes.Gray;
            }
        }
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SwitchingBetweenTabs(EnterInformationGrid, ShowInformationGrid, EnterInformationLabel, ShowInformationLabel);
        }

        private void Label_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            SwitchingBetweenTabs(ShowInformationGrid, EnterInformationGrid, ShowInformationLabel, EnterInformationLabel);
            ShowEquipmentInformation();
        }
        private class Spec
        {
            public string Marking { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public List<string> Specification { get; set; }
            public Spec(string Marking, string Name, string Type)
            {
                this.Marking = Marking;
                this.Name = Name;
                this.Type = Type;
                this.Specification = new List<string>();
            }
        }
        private void ShowEquipmentInformation()
        {
            using (var db = new FurnitureShopEntities()) 
            {
                List<Spec> listS = new List<Spec>();
                var list = db.Equipment.Select(p => new { p.Marking, p.Name, p.Equipment_type });
                var q = from f in db.Equipment_specification select f;
                list.ToList();
                q.ToList();
                foreach (var l in list)
                {
                    Spec spec = new Spec(l.Marking,l.Name,l.Equipment_type);

                    foreach (var s in q)
                    {
                        if (l.Marking == s.Marking)
                        {                            
                            spec.Specification.Add(s.Specification_name + ": " + s.Specification_value);
                        }
                    }
                    listS.Add(spec);
                }
                EquipmentDataGrid.ItemsSource = null;
                EquipmentDataGrid.ItemsSource = listS;                
            }
        }

        private void AddInformationToEquipment()
        {
            using (var db = new FurnitureShopEntities1())
            {
                List<string> list = new List<string>();
                foreach(var v in from f in db.Equipment_type select f.Equipment_type_name)
                {
                    list.Add(v);
                }
                EquipmentTypeComboBox.ItemsSource = list;
            }
        }
        private class EquipmentSpecification
        {
            public EquipmentSpecification(string Name, string Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
            public string Name;
            public string Value;
        }
        List<EquipmentSpecification> equipmentSpecifications = new List<EquipmentSpecification>();
        private void AddEquipmentSpecification(object sender, RoutedEventArgs e)
        {
            if (EquipmentSpecificationNameComboBox.Text != "" && EquipmentSpecificationValueTextBox.Text != "")
            {
                if (EquipmentSpecificationNameComboBox.Text.Length > 50) MessageBox.Show("Максимальная длина названия характеристики - 50 символов", Error);
                else
                {
                    bool enter = true;
                    foreach (var v in equipmentSpecifications)
                    {
                        if (v.Name == EquipmentSpecificationNameComboBox.Text.ToString())
                        {
                            v.Value = EquipmentSpecificationValueTextBox.Text;
                            enter = false;
                            break;
                        }
                    }
                    if (enter)
                    {
                        equipmentSpecifications.Add(new EquipmentSpecification(EquipmentSpecificationNameComboBox.Text.ToString(), EquipmentSpecificationValueTextBox.Text));
                        FillEquipmentSpecificationComboBox();
                    }
                }
            }
            else MessageBox.Show("Характеристика не заполнена", Error);
        }

        private void FillEquipmentSpecificationComboBox()
        {
            EquipmentSpecificationNameComboBox.ItemsSource = null;
            List<string> list = new List<string>();
            foreach (var v in equipmentSpecifications)
            {
                list.Add(v.Name);
            }
            EquipmentSpecificationNameComboBox.ItemsSource = list;
        }

        private void EquipmentSpecificationNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                foreach (var v in equipmentSpecifications)
                {
                    if (v.Name == comboBox.SelectedItem.ToString())
                    {
                        EquipmentSpecificationValueTextBox.Text = v.Value;
                        break;
                    }
                }
            }
        }

        private void AddEquipment(object sender, RoutedEventArgs e)
        {
            if (EquipmentMarkingTextBox.Text == "") MessageBox.Show("Введите маркировку", Error);
            else if (EquipmentNameTextBox.Text == "") MessageBox.Show("Введите название", Error);
            else if (EquipmentTypeComboBox.Text == "") MessageBox.Show("введите тип оборудования", Error);
            else if (EquipmentDatePurchaseDatePicker.Text == "") MessageBox.Show("Введите дату покупки", Error);
            else 
            {
                using (var db = new FurnitureShopEntities1())
                {
                    bool enter = true;
                    foreach (var v in from f in db.Equipment select f.Marking)
                    {
                        if (EquipmentMarkingTextBox.Text == v.Trim())
                        {
                            MessageBox.Show("Такая маркировка уже зарегистрирована", Error);
                            enter = false;
                            break;
                        }
                    }
                    if (enter)
                    {
                        Equipment equipment = new Equipment()
                        {
                            Marking = EquipmentMarkingTextBox.Text,
                            Name = EquipmentNameTextBox.Text,
                            Equipment_type = EquipmentTypeComboBox.Text,
                            Date_purchase = EquipmentDatePurchaseDatePicker.SelectedDate
                        };
                        db.Equipment.Add(equipment);
                        db.SaveChanges();
                        foreach(var v in equipmentSpecifications)
                        {
                            Equipment_specification specification = new Equipment_specification()
                            {
                                Marking =equipment.Marking.Trim(),
                                Specification_name = v.Name.Trim(),
                                Specification_value = v.Value.Trim()
                            };
                            db.Equipment_specification.Add(specification);

                        }
                        db.SaveChanges();
                        MessageBox.Show("Информация об оборудовании сохранена", Error);
                    }
                    
                }
            }
        }
        private void DeleteEquipmentSpecification(object sender, RoutedEventArgs e)
        {
            List<EquipmentSpecification> equipmentSpecifications2 = new List<EquipmentSpecification>();
            foreach (var v in equipmentSpecifications)
            {
                if (EquipmentSpecificationNameComboBox.Text != v.Name)
                {
                    equipmentSpecifications2.Add(v);
                }
            }
            equipmentSpecifications = equipmentSpecifications2;
            EquipmentSpecificationNameComboBox.Text = "";
            EquipmentSpecificationValueTextBox.Text = "";
            FillEquipmentSpecificationComboBox();
        }
    }
}

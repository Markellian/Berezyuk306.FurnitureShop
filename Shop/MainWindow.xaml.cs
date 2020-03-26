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
using Brushes = System.Windows.Media.Brushes;

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
        Grid GridToReturn;
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
                using (var db2 = new FurnitureShopEntitie())
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
        private void MakeHiddenParentGridOfButton(Button button)
        {
            Grid g = (Grid)button.Parent;
            g.Visibility = Visibility.Hidden;
        }
        private void GoToAuth(object sender, RoutedEventArgs e)
        {
            MakeHiddenParentGridOfButton((Button)sender);
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
            Password password = new Password(PasswordRegistrationPasswordBox.Password.ToString());
            if (LoginRegistrationTextBox.Text.ToString() == "") MessageBox.Show("Введите логин", Error);
            else
            {
                string mistake = password.CheckPassword();
                if (mistake != "") MessageBox.Show(mistake, Error);
                else if (password.Pas != PasswordRegistration2PasswordBox.Password.ToString()) MessageBox.Show("Пароли не совпадают", Error);
                else
                {
                    using (var d1 = new FurnitureShopEntitie())
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
                                Password = password.Pas
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
            MakeHiddenParentGridOfButton((Button)sender);
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
            using (var db = new FurnitureShopEntitie()) 
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
            using (var db = new FurnitureShopEntitie())
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
                using (var db = new FurnitureShopEntitie())
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

        private void GoToListMaterials(object sender, RoutedEventArgs e)
        {
            MaterialsGrid.Visibility = Visibility.Visible;
            MakeHiddenParentGridOfButton((Button)sender);
            GridToReturn = (Grid)((Button)sender).Parent;
            if (GridToReturn == DirectorGrid || DeputyDirectorGrid == GridToReturn)
            {
                MaterialsDataGrid.IsReadOnly = false;
                AddMaterialButton.IsEnabled = true;
                DeleteMaterial.IsEnabled = true;
            }
            else
            {
                MaterialsDataGrid.IsReadOnly = true;
                AddMaterialButton.IsEnabled = false;
                DeleteMaterial.IsEnabled = false;
            }
        }

        private void ReturnToLastGrid(object sender, RoutedEventArgs e)
        {
            MakeHiddenParentGridOfButton((Button)sender);
            GridToReturn.Visibility = Visibility.Visible;
        }
        private void ShowMaterials(string quality)
        {
            using(var db = new FurnitureShopEntitie())
            {
                var k = from m in db.Materials
                        where m.Quality1.QualityName == quality || quality == MaterialQualityAll
                        select new Materials2
                        {
                            Articyl = m.Articyl,
                            Name = m.Name,
                            Kolichestvo = m.Kolichestvo,
                            Edinica_izmerenia_name = m.Edinica_izmerenia1.Edinica_izmerenia_name,
                            Price = m.Price,
                            Shipper_name = m.Shipper.Shipper_name,
                            DateShip = m.DateShip,
                            QualityName = m.Quality1.QualityName
                        };

                if (quality == MaterialQualityAll)
                {
                    listMaterials = k.ToList();
                    MaterialsAll = listMaterials.Count;
                    MaterialsShowLabel.Content = MaterialsAll;
                }
                else
                {
                    listMaterials = new List<Materials2>();
                    foreach (var l in k)
                    {
                        if (l.QualityName == quality) listMaterials.Add(l);
                    }
                    MaterialsShowLabel.Content = listMaterials.Count;
                }
                MaterialsAllLabel.Content = MaterialsAll;
                MaterialsDataGrid.ItemsSource = listMaterials;

            }

        }
        List<Materials2> listMaterials = new List<Materials2>();
        int MaterialsAll = 0;
        const string MaterialQualityAll = "Все";
        private void MaterialsGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Grid)sender).Visibility == Visibility.Visible) ShowMaterials(MaterialQualityAll);
            List<string> list = (from q in new FurnitureShopEntitie().Quality select q.QualityName).ToList();
            list.Sort();
            list.Insert(0, MaterialQualityAll);
            MaterialsQualityComboBox.ItemsSource = list;
            MaterialsQualityComboBox.SelectedIndex = 0;
        }

        private void MaterialsQualityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowMaterials(((ComboBox)sender).SelectedItem.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Удалить выбранную строку?", "Внимание!", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                Materials2 o = (Materials2)MaterialsDataGrid.Items.CurrentItem;
                listMaterials.Remove((Materials2)MaterialsDataGrid.Items.CurrentItem);
                using (var db = new FurnitureShopEntitie())
                {
                    var obj = from f in db.Materials select f;
                    foreach (var c in obj)
                    {
                        if (c.Articyl == o.Articyl)
                        {
                            db.Materials.Remove(c);
                            break;
                        }                        
                    }
                    db.SaveChanges();
                }
                MaterialsDataGrid.ItemsSource = null;
                MaterialsDataGrid.ItemsSource = listMaterials;
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MaterialsGrid.IsEnabled = false;
            AddMaterialGrid.Visibility = Visibility.Visible;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddMaterialGrid.Visibility = Visibility.Hidden;
            MaterialsGrid.IsEnabled = true;
        }

        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            decimal b = 0;
            int a = 0;
            if (MaterialArticylTextBox.Text == "") MessageBox.Show("Артикул не указан", Error);
            else if (MaterialNameTextBox.Text == "") MessageBox.Show("Название не указано", Error);
            else if (MaterialQuantityTextBox.Text == "") MessageBox.Show("Количество не указано", Error);
            else if (MaterialPriceTextBox.Text == "") MessageBox.Show("Цена не указана", Error);
            else
            {
                if (MaterialQuantityTextBox.Text != "0")
                {
                    Int32.TryParse(MaterialQuantityTextBox.Text, out a);
                    if (a == 0)
                    {
                        MessageBox.Show("Количество указано неверно", Error);
                        return;
                    }
                }
                if (MaterialPriceTextBox.Text != "0")
                {
                    Decimal.TryParse(MaterialPriceTextBox.Text, out b);
                    if (b == 0)
                    {
                        MessageBox.Show("Цена указана неверно", Error);
                        return;
                    }
                }

                using (var db = new FurnitureShopEntitie())
                {
                    var mat = db.Materials;
                    foreach (var m in mat)
                    {
                        if (m.Articyl == MaterialArticylTextBox.Text)
                        {
                            MessageBox.Show("Материал с таким артикулом уже существует", Error);
                            return;
                        }
                    }
                    var EI = from f in db.Edinica_izmerenia where f.Edinica_izmerenia_name == MaterialEdIzmComboBox.SelectedItem.ToString() select f.Edinica_izmerenia_id;
                    int ei = 1;
                    foreach (var v in EI)
                    {
                        ei = v;
                    }
                    var MT = from f in db.Type_material where f.Type_material_name == MaterialTypeComboBox.SelectedItem.ToString() select f.Type_material_id;
                    int mt = 1;
                    foreach (var v in MT)
                    {
                        mt = v;
                    }
                    Materials materials = new Materials()
                    {
                        Articyl = MaterialArticylTextBox.Text,
                        Name = MaterialNameTextBox.Text,
                        Edinica_izmerenia = ei,
                        Kolichestvo = a,
                        Type_material = mt,
                        Price = b
                    };
                    db.Materials.Add(materials);
                    if (db.SaveChanges() == 0) MessageBox.Show("Не удалось добавить материал", Error);
                    else
                    {
                        MessageBox.Show("Материал добавлен");
                        Button_Click_4(null, null);
                        ShowMaterials(MaterialQualityAll);
                    }
                }
            }
        }

        private void AddMaterialGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (var db = new FurnitureShopEntitie())
            {
                var list = from f in db.Edinica_izmerenia select f.Edinica_izmerenia_name;
                MaterialEdIzmComboBox.ItemsSource = list.ToList();
                MaterialEdIzmComboBox.SelectedIndex = 0;

                list = from f in db.Type_material select f.Type_material_name;
                MaterialTypeComboBox.ItemsSource = list.ToList();
                MaterialTypeComboBox.SelectedIndex = 0;
            }
            MaterialArticylTextBox.Text = "";
            MaterialNameTextBox.Text = "";
            MaterialQuantityTextBox.Text = "";
            MaterialPriceTextBox.Text = "";
        }
    }
}

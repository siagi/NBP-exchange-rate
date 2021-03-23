using System;
using System.Collections.Generic;
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
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net;
using System.Xml.Linq;

namespace KursyWalutNBPWPFdotNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today;

        }

        private DateTime dateFrom;

        public DateTime DateFrom
        {
            get { return dateFrom; }
            set 
            {
                    dateFrom = value.Date;
                    OnPropertyChanged("DateFrom");
            }
        }

        private DateTime dateTo;

        public DateTime DateTo
        {
            get { return dateTo; }
            set 
            { 
                dateTo = value.Date;
                OnPropertyChanged("DateTo");
            }
        }

        private string currencySymbol;

        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set 
            {
                //value = CurrencyComboBox.Text; 
                //currencySymbol = value;
                currencySymbol = value.Substring(value.Length-3);
                Console.WriteLine("zmiana");
                OnPropertyChanged("CurrencySymbol");
            }
        }

        private DateTime dateStart; 

        public DateTime DateStart
        {
            get { return dateStart; }
            set { dateStart = value; OnPropertyChanged("DateStart"); }
        }

        private decimal sredniKursKupna;

        public decimal SredniKursKupna
        {
            get { return sredniKursKupna; }
            set { sredniKursKupna = value; OnPropertyChanged("SredniKursKupna"); }
        }

        private decimal sredniKursSprzedazy;

        public decimal SredniKursSprzedazy
        {
            get { return sredniKursSprzedazy; }
            set { sredniKursSprzedazy = value; OnPropertyChanged("SredniKursSprzedazy"); }
        }

        private decimal minimalnyKursKupna;

        public decimal MinimalnyKursKupna
        {
            get { return minimalnyKursKupna; }
            set { minimalnyKursKupna = value; OnPropertyChanged("MinimalnyKursKupna"); }
        }

        private decimal maksymalnyKursKupna;

        public decimal MaksymalnyKursKupna
        {
            get { return maksymalnyKursKupna; }
            set { maksymalnyKursKupna = value; OnPropertyChanged("MaksymalnyKursKupna"); }
        }

        private decimal minimalnyKursSprzedazy;

        public decimal MinimalnyKursSprzedazy
        {
            get { return minimalnyKursSprzedazy; }
            set { minimalnyKursSprzedazy = value; OnPropertyChanged("MinimalnyKursSprzedazy"); }
        }

        private decimal maksymalnyKursSprzedazy;

        public decimal MaksymalnyKursSprzedazy
        {
            get { return maksymalnyKursSprzedazy; }
            set { maksymalnyKursSprzedazy = value; OnPropertyChanged("MaksymalnyKursSprzedazy"); }
        }

        private double odchylenieStandardoweKursKupna;

        public double OdchylenieStandardoweKursKupna
        {
            get { return odchylenieStandardoweKursKupna; }
            set { odchylenieStandardoweKursKupna = value; OnPropertyChanged("OdchylenieStandardoweKursKupna"); }
        }

        private double odchylenieStandardoweKursSprzedazy;

        public double OdchylenieStandardoweKursSprzedazy
        {
            get { return odchylenieStandardoweKursSprzedazy; }
            set { odchylenieStandardoweKursSprzedazy = value; OnPropertyChanged("OdchylenieStandardoweKursSprzedazy"); }
        }

        private KeyValuePair<DateTime, decimal> maxDataKursKupna;

        public KeyValuePair<DateTime, decimal> MaxDataKursKupna
        {
            get { return maxDataKursKupna; }
            set { maxDataKursKupna = value; OnPropertyChanged("MaxDataKursKupna"); }
        }

        private KeyValuePair<DateTime, decimal> maxDataKursSprzedazy;

        public KeyValuePair<DateTime, decimal> MaxDataKursSprzedazy
        {
            get { return maxDataKursSprzedazy; }
            set { maxDataKursSprzedazy = value; OnPropertyChanged("MaxDataKursSprzedazy"); }
        }













        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void DownloadTxtFileWithCodes()
        {
            List<decimal> kursKupnaList = new List<decimal>();
            List<decimal> kursSprzedazyList = new List<decimal>();

            List<KeyValuePair<DateTime, decimal>> dataKursKupnaList = new List<KeyValuePair<DateTime, decimal>>();
            List<KeyValuePair<DateTime, decimal>> dataKursSprzedazyList = new List<KeyValuePair<DateTime, decimal>>();



            DateStart = dateFrom;
            double daysBetweenDates = (DateTo - DateFrom).TotalDays;
            for (double i = 0; i < daysBetweenDates; i++)
            {
                try
                {

                    string dirName = "dir" + DateStart.Year + ".txt";
                    string adres = "https://www.nbp.pl/kursy/xml/" + dirName;
                    using WebClient wc = new WebClient();
                    string txtCodes = wc.DownloadString(adres);
                    string[] codesArray = txtCodes.Split("\n");
                    string lookingForDateCode = DateStart.Year.ToString("D2").Substring(DateStart.Year.ToString("D2").Length - 2) + DateStart.Month.ToString("D2") + DateStart.Day.ToString();
                    string xmlCode = "";
                    for (int j = 0; j < codesArray.Length - 1; j++)
                    {
                        if (codesArray[j].Trim().Substring(codesArray[j].Length - 7) == lookingForDateCode && codesArray[j].Contains('c'))
                        {
                            xmlCode = codesArray[j];
                        }

                    }
                    if (xmlCode != "" && xmlCode != null)
                    {
                        using WebClient wcForXmlString = new WebClient();
                        string xmlData = wcForXmlString.DownloadString("https://www.nbp.pl/kursy/xml/" + xmlCode.Substring(xmlCode.Length - 12, 11) + ".xml");
                        XDocument convertStringtoXml = XDocument.Parse(xmlData);

                        XElement xmlTabelaKursow = convertStringtoXml.Element("tabela_kursow");
                        IEnumerable<XElement> xmlPozycje = xmlTabelaKursow.Elements("pozycja");
                        foreach (var pozycja in xmlPozycje)
                        {
                            XElement kodWaluty = pozycja.Element("kod_waluty");
                            XElement kursKupna = pozycja.Element("kurs_kupna");
                            XElement kursSprzedazy = pozycja.Element("kurs_sprzedazy");
                            if (kodWaluty.Value == CurrencySymbol)
                            {

                                kursKupnaList.Add(decimal.Parse(kursKupna.Value));
                                kursSprzedazyList.Add(decimal.Parse(kursSprzedazy.Value));

                                dataKursKupnaList.Add(new KeyValuePair<DateTime, decimal>(DateStart, decimal.Parse(kursKupna.Value)));
                                dataKursSprzedazyList.Add(new KeyValuePair<DateTime, decimal>(DateStart, decimal.Parse(kursSprzedazy.Value)));




                            }
                        }




                    }


                    DateStart = DateStart + TimeSpan.FromDays(1);
                }
                catch (WebException)
                {

                    MessageBox.Show("Błąd połączenia - sprawdz swoje połączenie lub adres hosta");
                }

            }
            SredniKursKupna = kursKupnaList.Sum() / kursKupnaList.Count;
            SredniKursSprzedazy = kursSprzedazyList.Sum() / kursSprzedazyList.Count;

            MinimalnyKursKupna = kursKupnaList.Min();
            MaksymalnyKursKupna = kursKupnaList.Max();

            MinimalnyKursSprzedazy = kursSprzedazyList.Min();
            MaksymalnyKursSprzedazy = kursSprzedazyList.Max();
            List<decimal> wariancjaLicznikkursKupnaList = new List<decimal>();
            List<decimal> wariancjaLicznikkursSprzedazyList = new List<decimal>();

            foreach (var item in kursKupnaList)
            {
                wariancjaLicznikkursKupnaList.Add(((item - SredniKursKupna) * (item - SredniKursKupna)));
            }

            foreach (var item in kursSprzedazyList)
            {
                wariancjaLicznikkursSprzedazyList.Add(((item - SredniKursSprzedazy) * (item - SredniKursSprzedazy)));
            }

            OdchylenieStandardoweKursKupna = Math.Sqrt((double)(wariancjaLicznikkursKupnaList.Sum() / sredniKursKupna));
            OdchylenieStandardoweKursSprzedazy = Math.Sqrt((double)(wariancjaLicznikkursSprzedazyList.Sum() / sredniKursSprzedazy));

            Console.WriteLine($"Sredni kurs kupna {CurrencySymbol} : {sredniKursKupna:F2} || Minimalny kurs kupna {CurrencySymbol} : {minimalnyKursKupna:F2} || Maksymalny kurs kupna {CurrencySymbol} : {maksymalnyKursKupna:F2} || Odchylenie standardowe : {odchylenieStandardoweKursKupna}");
            Console.WriteLine($"----------------");
            Console.WriteLine($"Sredni kurs sprzedazy {CurrencySymbol} : {sredniKursSprzedazy:F2} || Minimalny kurs sprzedazy {CurrencySymbol} : {minimalnyKursSprzedazy:F2} || Maksymalny kurs sprzedazy {CurrencySymbol} : {maksymalnyKursSprzedazy:F2} || Odchylenie standardowe : {odchylenieStandardoweKursSprzedazy}");
            Console.WriteLine($"----------------");
            maxDataKursKupna = new KeyValuePair<DateTime, decimal>();

            for (int i = 0; i < dataKursKupnaList.Count - 2; i++)
            {
                for (int j = 0; j < dataKursKupnaList.Count - 2; j++)
                {
                    if (Math.Abs((dataKursKupnaList[j + 1].Value - dataKursKupnaList[j].Value)) > Math.Abs((dataKursKupnaList[j + 2].Value - dataKursKupnaList[j + 1].Value)))
                    {
                        MaxDataKursKupna = new KeyValuePair<DateTime, decimal>(dataKursKupnaList[j].Key, Math.Abs((dataKursKupnaList[j + 1].Value - dataKursKupnaList[j].Value)));
                    }
                    else
                    {
                        MaxDataKursKupna = new KeyValuePair<DateTime, decimal>(dataKursKupnaList[j + 1].Key, Math.Abs((dataKursKupnaList[j + 2].Value - dataKursKupnaList[j + 1].Value)));
                    }
                }
            }

            maxDataKursSprzedazy = new KeyValuePair<DateTime, decimal>();

            Console.WriteLine($"Maksymalna roznica kursu kupna to : {maxDataKursKupna.Value:F4} dnia : {maxDataKursKupna.Key.ToString("yyyy-MM-dd")} ");

            for (int i = 0; i < dataKursSprzedazyList.Count - 2; i++)
            {
                for (int j = 0; j < dataKursSprzedazyList.Count - 2; j++)
                {
                    if (Math.Abs((dataKursSprzedazyList[j + 1].Value - dataKursSprzedazyList[j].Value)) > Math.Abs((dataKursSprzedazyList[j + 2].Value - dataKursSprzedazyList[j + 1].Value)))
                    {
                        MaxDataKursSprzedazy = new KeyValuePair<DateTime, decimal>(dataKursSprzedazyList[j].Key, Math.Abs((dataKursSprzedazyList[j + 1].Value - dataKursSprzedazyList[j].Value)));
                    }
                    else
                    {
                        MaxDataKursSprzedazy = new KeyValuePair<DateTime, decimal>(dataKursSprzedazyList[j + 1].Key, Math.Abs((dataKursSprzedazyList[j + 2].Value - dataKursSprzedazyList[j + 1].Value)));
                    }
                }
            }
            Console.WriteLine($"----------------");
            Console.WriteLine($"Maksymalna roznica kursu sprzedazy to : {maxDataKursSprzedazy.Value:F4} dnia : {maxDataKursSprzedazy.Key.ToString("yyyy-MM-dd")} ");


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DateFrom > DateTime.Today || DateTo > DateTime.Today)
            {
                
                MessageBox.Show("Zły zakres dat. Data zakresu nie może być późniejsza niż dzień dzisiejszy");
            }
            else if(DateFrom > DateTo)
            {
               
                MessageBox.Show("Zły zakres dat. Data zakresu \"do\" nie może być wcześniejsza niż data zakresu \"od\"");
            }
            else
            {
     
                DownloadTxtFileWithCodes();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy2
{
    class Osoba
    {
        private string _nazwa;

        public string Nazwa
        {
            get { return _nazwa; }
            set { _nazwa = value; }
        }

        private string _data_urodzenia;

        public string Data_urodzenia
        {
            get { return _data_urodzenia; }
            set { _data_urodzenia = value; }
        }

        private string _data_smierci;

        public string  Data_smierci
        {
            get { return _data_smierci; }
            set { _data_smierci = value; }
        }

        private string _plec;

        public string Plec
        {
            get { return _plec; }
            set { _plec = value; }
        }

        private Osoba _ojciec;

        public Osoba Ojciec
        {
            get { return _ojciec; }
            set { _ojciec = value; }
        }

        private Osoba _matka;

        public Osoba Matka
        {
            get { return _matka; }
            set { _matka = value; }
        }

        public Osoba(string nazwa, string data_urodzenia, string data_smierci, string plec, Osoba ojciec, Osoba matka, List<Osoba> lista_dzieci)
        {
            _nazwa = nazwa;
            _data_urodzenia = data_urodzenia;
            _data_smierci = data_smierci;
            _plec = plec;
            _ojciec = ojciec;
            _matka = matka;
            _lista_dzieci = lista_dzieci;
        }

        private List<Osoba> _lista_dzieci;

        public List <Osoba> Lista_dzieci
        {
            get { return _lista_dzieci; }
            set { _lista_dzieci = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using Db4objects.Db4o.Reflect.Generic;
using System.IO;

namespace Bazy2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        IObjectContainer db;
        Osoba osoba;
        string data_ur;
        string data_sm;
        string plec;
        
        private List<Osoba> ListaPrzodkow(Osoba osoba)
        {
            var lista = new List<Osoba>();
            if (osoba.Matka != null)
            {
                lista.Add(osoba.Matka);
                lista.AddRange(ListaPrzodkow(osoba.Matka));
            }
            if (osoba.Ojciec != null)
            {
                lista.Add(osoba.Ojciec);
                lista.AddRange(ListaPrzodkow(osoba.Ojciec));
            }
            return lista;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox22.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();
                IObjectSet result1 = db.QueryByExample(new Osoba(textBox23.Text, null, null, null, null, null, null));
                Osoba found1 = (Osoba)result1.Next();
                
                List<Osoba> lista_rodzica = new List<Osoba>();
                List<Osoba> lista_dziecka = new List<Osoba>();

                lista_rodzica = ListaPrzodkow(found);
                lista_dziecka = ListaPrzodkow(found1);

                if (found.Data_urodzenia == null || found1.Data_urodzenia == null)
                {
                    if (lista_rodzica.Contains(found1))
                    {
                        MessageBox.Show("Cykle są niedopuszczalne.");
                    }
                }
                else
                {
                    if (Convert.ToDateTime(found.Data_urodzenia).Year < Convert.ToDateTime(found1.Data_urodzenia).Year)
                    {
                        if (found.Plec == "kobieta")
                        {
                            if (found1.Matka == null)
                            {
                                if (found.Data_smierci != null)
                                {
                                    if (Convert.ToDateTime(found.Data_smierci) > Convert.ToDateTime(found1.Data_urodzenia))
                                    {
                                        if (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year >= 10 && Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year <= 60)
                                        {
                                            found1.Matka = found;
                                            db.Store(found1);
                                            found.Lista_dzieci.Add(found1);
                                            db.Store(found.Lista_dzieci);

                                            MessageBox.Show("Utworzono relację");
                                            textBox22.Text = "Rodzic";
                                            textBox23.Text = "Dziecko";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Niemożliwe. Matka miałaby " + (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year) + " lat podczas narodzin dziecka.");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Dziecko nie mogło narodzić się po śmierci matki.");
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year >= 10 && Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year <= 60)
                                    {
                                        found1.Matka = found;
                                        db.Store(found1);
                                        found.Lista_dzieci.Add(found1);
                                        db.Store(found.Lista_dzieci);

                                        MessageBox.Show("Utworzono relację");
                                        textBox22.Text = "Rodzic";
                                        textBox23.Text = "Dziecko";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Niemożliwe. Matka miałaby " + (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year) + " lat podczas narodzin dziecka.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(found1.Nazwa + " ma już matkę.");
                            }
                        }

                        if (found.Plec == "mężczyzna")
                        {
                            if (found1.Ojciec == null)
                            {
                                if (found.Data_smierci != null)
                                {
                                    if (Convert.ToDateTime(found.Data_smierci) < Convert.ToDateTime(found1.Data_urodzenia))
                                    {
                                        if (Math.Abs((Convert.ToDateTime(found1.Data_urodzenia) - Convert.ToDateTime(found.Data_smierci)).Days) <= 270)
                                        {
                                            if (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year >= 12 && Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year <= 70)
                                            {
                                                found1.Ojciec = found;
                                                db.Store(found1);
                                                found.Lista_dzieci.Add(found1);
                                                db.Store(found.Lista_dzieci);

                                                MessageBox.Show("Utworzono relację");
                                                textBox22.Text = "Rodzic";
                                                textBox23.Text = "Dziecko";
                                            }
                                            else
                                            {
                                                MessageBox.Show("Niemożliwe. Ojciec miałby " + (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year) + " lat podczas narodzin dziecka.");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Dziecko, które urodziło sie później niż 9 miesięcy po śmierci ojca nie może byc jego.");
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year >= 12 && Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year <= 70)
                                        {
                                            found1.Ojciec = found;
                                            db.Store(found1);
                                            found.Lista_dzieci.Add(found1);
                                            db.Store(found.Lista_dzieci);

                                            MessageBox.Show("Utworzono relację");
                                            textBox22.Text = "Rodzic";
                                            textBox23.Text = "Dziecko";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Niemożliwe. Ojciec miałby " + (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year) + " lat podczas narodzin dziecka.");
                                        }
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year >= 12 && Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year <= 70)
                                    {
                                        found1.Ojciec = found;
                                        db.Store(found1);
                                        found.Lista_dzieci.Add(found1);
                                        db.Store(found.Lista_dzieci);

                                        MessageBox.Show("Utworzono relację");
                                        textBox22.Text = "Rodzic";
                                        textBox23.Text = "Dziecko";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Niemożliwe. Ojciec miałby " + (Convert.ToDateTime(found1.Data_urodzenia).Year - Convert.ToDateTime(found.Data_urodzenia).Year) + " lat podczas narodzin dziecka.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(found1.Nazwa + " ma już ojca.");
                            }
                        }
                        if (found.Plec == null)
                        {
                            MessageBox.Show("Nie można utworzyć relacji z uwagi na niewiadomą płeć rodzica.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Niemożliwe! Rodzic musi byc starszy.");
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox24.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();
                IObjectSet result1 = db.QueryByExample(new Osoba(textBox25.Text, null, null, null, null, null, null));
                Osoba found1 = (Osoba)result1.Next();

                for (int i = 0; i < found.Lista_dzieci.Count; i++)
                {
                    if (found.Lista_dzieci[i] == found1)
                    {
                        found.Lista_dzieci.RemoveAt(i);
                        db.Store(found.Lista_dzieci);
                    }
                }

                if (found.Plec == "kobieta")
                {
                    found1.Matka = null;
                    db.Store(found1);
                }
                if (found.Plec == "mężczyzna")
                {
                    found1.Ojciec = null;
                    db.Store(found1);
                }

                MessageBox.Show("Usunięto relację");

                textBox24.Text = "Rodzic";
                textBox25.Text = "Dziecko";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                Osoba osoba = new Osoba(textBox15.Text, null, null, null, null, null, null);
                IObjectSet result = db.QueryByExample(osoba);

                foreach (Osoba item in result)
                {
                    textBox16.Text = item.Nazwa;

                    if (item.Data_urodzenia == null)
                    {
                        textBox17.Text = "brak daty ur.";
                    }
                    else
                    {
                        textBox17.Text = item.Data_urodzenia;
                    }
                    if (item.Data_smierci == null)
                    {
                        textBox18.Text = "brak daty śm.";
                    }
                    else
                    {
                        textBox18.Text = item.Data_smierci;
                    }
                    if (item.Plec == null)
                    {
                        textBox19.Text = "brak płci";
                    }
                    else
                    {
                        textBox19.Text = item.Plec;
                    }
                    if (item.Ojciec == null)
                    {
                        textBox20.Text = "brak ojca";
                    }
                    else
                    {
                        textBox20.Text = item.Ojciec.Nazwa;
                    }
                    if (item.Matka == null)
                    {
                        textBox21.Text = "brak matki";
                    }
                    else
                    {
                        textBox21.Text = item.Matka.Nazwa;
                    }
                    if (item.Lista_dzieci.Count == 0)
                    {
                        comboBox2.Text = "brak dzieci";
                    }
                    else
                    {
                        for (int i = 0; i < item.Lista_dzieci.Count; i++)
                        {
                            {
                                comboBox2.Items.Add(item.Lista_dzieci[i].Nazwa);
                            }
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                Osoba osoba = new Osoba(textBox14.Text, null, null, null, null, null, null);
                IObjectSet result = db.QueryByExample(osoba);

                foreach (Osoba item in result)
                {
                    textBox8.Text = item.Nazwa;

                    if (item.Data_urodzenia == null)
                    {
                        textBox9.Text = "brak daty ur.";
                    }
                    else
                    {
                        textBox9.Text = item.Data_urodzenia;
                    }
                    if (item.Data_smierci == null)
                    {
                        textBox10.Text = "brak daty śm.";
                    }
                    else
                    {
                        textBox10.Text = item.Data_smierci;
                    }
                    if (item.Plec == null)
                    {
                        textBox11.Text = "brak płci";
                    }
                    else
                    {
                        textBox11.Text = item.Plec;
                    }
                    if (item.Ojciec == null)
                    {
                        textBox12.Text = "brak ojca";
                    }
                    else
                    {
                        textBox12.Text = item.Ojciec.Nazwa;
                    }
                    if (item.Matka == null)
                    {
                        textBox13.Text = "brak matki";
                    }
                    else
                    {
                        textBox13.Text = item.Matka.Nazwa;
                    }
                    if (item.Lista_dzieci.Count == 0)
                    {
                        comboBox1.Text = "brak dzieci";
                    }
                    else
                    {
                        for (int i = 0; i < item.Lista_dzieci.Count; i++)
                        {
                            {
                                comboBox1.Items.Add(item.Lista_dzieci[i].Nazwa);
                            }
                        }
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox15.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();
                found.Nazwa = textBox16.Text;
                int ilosc = 0;
                int il = 0;

                if (textBox17.Text == "brak daty ur." || textBox17.Text == "")
                {
                    found.Data_urodzenia = null;
                }
                else
                {
                    if (found.Plec == "kobieta")
                    {
                        il = 0;
                        for (int i = 0; i < found.Lista_dzieci.Count; i++)
                        {
                            if (Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia).Year - Convert.ToDateTime(textBox17.Text).Year >= 10 && Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia).Year - Convert.ToDateTime(textBox17.Text).Year <= 60)
                            {
                                il++;
                            }
                        }
                        if (found.Lista_dzieci.Count == il)
                        {
                            found.Data_urodzenia = textBox17.Text;
                        }
                        else
                        {
                            MessageBox.Show("Nie można zmodyfikować daty urodzenia, gdyż przy tej dacie matka urodziłaby któreś z dzieci spoza przedziału <10; 60> lat.");
                        }
                    }
                    if (found.Plec == "mężczyzna")
                    {
                        il = 0;
                        for (int i = 0; i < found.Lista_dzieci.Count; i++)
                        {
                            if (Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia).Year - Convert.ToDateTime(textBox17.Text).Year >= 12 && Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia).Year - Convert.ToDateTime(textBox17.Text).Year <= 70)
                            {
                                il++;
                            }
                        }
                        if (found.Lista_dzieci.Count == il)
                        {
                            found.Data_urodzenia = textBox17.Text;
                        }
                        else
                        {
                            MessageBox.Show("Nie można zmodyfikować daty urodzenia, gdyż przy tej dacie ojciec miałby któreś z dzieci spoza przedziału <12; 70> lat.");
                        }
                    }
                }
                if (textBox18.Text == "brak daty śm." || textBox18.Text == "")
                {
                    found.Data_smierci = null;
                }
                else
                {
                    if (found.Plec == "kobieta")
                    {
                        for (int i = 0; i < found.Lista_dzieci.Count; i++)
                        {
                            if (Convert.ToDateTime(textBox18.Text) > Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia))
                            {
                                ilosc++;
                            }
                        }
                        if (found.Lista_dzieci.Count == ilosc)
                        {
                            found.Data_smierci = textBox18.Text;
                        }
                        else
                        {
                            MessageBox.Show("Nie można zmienić daty śmierci, gdyż nie spełnia ona kryterium: podana data jest większa niż data narodzin któregoś z dzieci");
                        }
                    }

                    if (found.Plec == "mężczyzna")
                    {
                        ilosc = 0;
                        for (int i = 0; i < found.Lista_dzieci.Count; i++)
                        {
                            if (Convert.ToDateTime(textBox18.Text) < Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia))
                            {
                                if (Math.Abs((Convert.ToDateTime(found.Lista_dzieci[i].Data_urodzenia) - Convert.ToDateTime(textBox18.Text)).Days) <= 270)
                                {
                                    ilosc++;
                                }
                            }
                            else
                            {
                                ilosc++;
                            }
                        }
                        if (found.Lista_dzieci.Count == ilosc)
                        {
                            found.Data_smierci = textBox18.Text;
                        }
                        else
                        {
                            MessageBox.Show("Nie można zmienić daty śmierci, gdyż nie spełnia ona kryterium: podana data musi być większa od daty narodzin każdego z dzieci lub nie przekraczać różnicy tych dat 270 dni.");
                        }
                    }
                }
                if (textBox19.Text == "brak płci" || textBox19.Text == "")
                {
                    found.Plec = null;
                }
                else
                {
                    found.Plec = textBox19.Text;
                }

                db.Store(found);
                MessageBox.Show("Zmodyfikowano");

                textBox15.Text = "Podaj Nazwę";
                textBox16.Text = "Nazwa";
                textBox17.Text = "Data urodzenia";
                textBox18.Text = "Data śmierci";
                textBox19.Text = "Płeć";
                textBox20.Text = "Ojciec";
                textBox21.Text = "Matka";
                comboBox2.Text = "Lista dzieci";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox14.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();
                db.Delete(found);
                MessageBox.Show("Usunięto");

                textBox14.Text = "Podaj Nazwę";
                textBox8.Text = "Nazwa";
                textBox9.Text = "Data urodzenia";
                textBox10.Text = "Data śmierci";
                textBox11.Text = "Płeć";
                textBox12.Text = "Ojciec";
                textBox13.Text = "Matka";
                comboBox1.Text = "Lista dzieci";
                comboBox1.Items.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                data_ur = textBox2.Text;
            }
            else
            {
                data_ur = null;
            }
            if (textBox3.Text != "")
            {
                data_sm = textBox3.Text;
            }
            else
            {
                data_sm = null;
            }
            if (comboBox3.Text != "")
            {
                plec = comboBox3.Text;
            }
            else
            {
                plec = null;
            }

            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(typeof(Osoba));

                if (result.Count == 0)
                {
                    osoba = new Osoba(textBox1.Text, data_ur, data_sm, plec, null, null, new List<Osoba>());
                    db.Store(osoba);

                    MessageBox.Show("Dodano do bazy kontakt: " + textBox1.Text);

                    textBox1.Text = "Nazwa";
                    textBox2.Text = "Data urodzenia";
                    textBox3.Text = "Data śmierci";
                    comboBox3.Text = "Płeć";
                }
                else
                {
                    try
                    {
                        IObjectSet result1 = db.QueryByExample(new Osoba(textBox1.Text, null, null, null, null, null, null));
                        Osoba found1 = (Osoba)result1.Next();

                        MessageBox.Show("Osoba " + found1.Nazwa + " istnieje. Podaj inne dane.");
                        textBox1.Text = "Nazwa";
                        textBox2.Text = "Data urodzenia";
                        textBox3.Text = "Data śmierci";
                        comboBox3.Text = "Płeć";
                    }
                    catch (Exception)
                    {
                        osoba = new Osoba(textBox1.Text, data_ur, data_sm, plec, null, null, new List<Osoba>());
                        db.Store(osoba);

                        MessageBox.Show("Dodano do bazy kontakt: " + textBox1.Text);

                        textBox1.Text = "Nazwa";
                        textBox2.Text = "Data urodzenia";
                        textBox3.Text = "Data śmierci";
                        comboBox3.Text = "Płeć";
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                listBox1.Items.Clear();

                IObjectSet result = db.QueryByExample(typeof(Osoba));

                foreach (Osoba item in result)
                {
                    listBox1.Items.Add(item.Nazwa);
                }
            }
        }

        private string RysujAkapit(int ak)
        {
            string akapit = "";

            for (int i = 0; i < ak; i++)
            {
                akapit += " ";
            }

            return akapit;
        }

        void PrintTree(Osoba osoba, int ak)
        {
            if (osoba.Lista_dzieci.Count > 0)
            {
                foreach (var item in osoba.Lista_dzieci)
                {
                    listBox2.Items.Add(RysujAkapit(ak) + item.Nazwa + " - dziecko osoby " + osoba.Nazwa);
                    PrintTree(item, ak + 3);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();

            int ak = 3;

            var config = Db4oEmbedded.NewConfiguration();
            config.Common.ActivationDepth = 20;
            config.Common.UpdateDepth = 20;

            using (db = Db4oEmbedded.OpenFile(config, "Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox4.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();

                listBox2.Items.Add(found.Nazwa);

                PrintTree(found, ak);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();

            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox5.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();
                IObjectSet result1 = db.QueryByExample(new Osoba(textBox6.Text, null, null, null, null, null, null));
                Osoba found1 = (Osoba)result1.Next();

                List<Osoba> t5 = new List<Osoba>();
                List<Osoba> t6 = new List<Osoba>();

                t5 = ListaPrzodkow(found);
                t6 = ListaPrzodkow(found1);

                var wspolna = t5.Intersect(t6);

                foreach (Osoba item in wspolna)
                {
                    listBox3.Items.Add(item.Nazwa);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (db = Db4oEmbedded.OpenFile("Baza.data"))
            {
                IObjectSet result = db.QueryByExample(new Osoba(textBox7.Text, null, null, null, null, null, null));
                Osoba found = (Osoba)result.Next();

                Stack<Osoba> stack = new Stack<Osoba>();

                stack.Push(found);

                while (stack.Count != 0)
                {
                    found = stack.Pop();

                    if (found.Lista_dzieci.Count != 0)
                    {
                        {
                            for (int i = 0; i < found.Lista_dzieci.Count; i++)
                            {
                                if (found.Lista_dzieci[i].Data_smierci == null)
                                {
                                    listBox4.Items.Add(found.Lista_dzieci[i].Nazwa + " - dziecko osoby " + found.Nazwa);
                                }
                                else
                                {
                                    stack.Push(found.Lista_dzieci[i]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
using System;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace DM_Cooking
{
    public class Client
    {
        #region attributs
        private string idClient;
        private string nom_C;
        private string prenom_C;
        private string num_tel;
        private string adresse;
        private string num_compte;
        private bool statut_CdR;
        private int nbr_recette_com;
        private int solde_cook;
        private string idCuisinier;
        #endregion

        #region constructeur

        public Client(string idClient = "0", string prenom_C = "0", string nom_C = "0", string num_tel = "0", string num_compte = "0", string adresse = "0", bool statut_CdR = false, int nbr_recette_com = 0, int solde_cook = 0, string idCuisinier = "0")
        {
            this.idClient = idClient;
            this.prenom_C = prenom_C;
            this.nom_C = nom_C;
            this.num_tel = num_tel;
            this.num_compte = num_compte;
            this.adresse = adresse;
            this.statut_CdR = statut_CdR;
            this.nbr_recette_com = nbr_recette_com;
            this.solde_cook = solde_cook;
            this.idCuisinier = idCuisinier;
        }

        public Client(MySqlDataReader reader)
        {
            //reader.Read();
            this.idClient = reader.GetString(0);
            this.prenom_C = reader.GetString(1);
            this.nom_C = reader.GetString(2);
            this.num_tel = reader.GetString(3);
            this.num_compte = reader.GetString(4);
            this.adresse = reader.GetString(5);
            this.statut_CdR = reader.GetBoolean(6);
            this.nbr_recette_com = reader.GetInt32(7);
            this.solde_cook = reader.GetInt32(8);
            this.idCuisinier = reader.GetString(9);

        }

        #endregion

        #region proprietes

        public string IdClient
        {
            get { return this.idClient; }
            set { this.idClient = value; }
        }

        public string Prenom_C
        {
            get { return this.prenom_C; }
            set { this.prenom_C = value; }
        }

        public string Nom_C
        {
            get { return this.nom_C; }
            set { this.nom_C = value; }
        }

        public string Num_tel
        {
            get { return this.num_tel; }
            set { this.num_tel = value; }
        }

        public bool Statut_Cdr
        {
            get { return this.statut_CdR; }
            set { this.statut_CdR = value; }
        }

        public string Num_compte
        {
            get { return this.num_compte; }
            set { this.num_compte = value; }
        }

        public int Nbr_recette_com
        {
            get { return this.nbr_recette_com; }
            set { this.nbr_recette_com = value; }
        }

        public int Solde_cook
        {
            get { return this.solde_cook; }
            set { this.solde_cook = value; }
        }

        public string IdCuisinier
        {
            get { return this.idCuisinier; }
            set { this.idCuisinier = value; }
        }

        public string Adresse
        {
            get { return this.adresse; }
            set { this.adresse = value; }
        }

        #endregion

        #region affichage

        public override string ToString()
        {
            string a = this.prenom_C + " " + this.nom_C + " est client, a pour id:" + this.idClient + " et son numéro de compte est: " + this.num_compte + " et son numéro de téléphone est : " + this.num_tel + " et a pour adresse " + this.adresse + ". \n";
            if (this.statut_CdR)
            {
                a += "Ce client est également créateur de recette, " + this.nbr_recette_com + " de ses recettes ont été commandées et son solde cook est de : " + this.solde_cook;
            }
            if(this.IdCuisinier!="0")
            {
                a += "Il est égalent client-cuisinier dont l'ID est : " + this.idCuisinier;
            }
            return a;
        }

        #endregion

        #region BDD -> C#

        public List<Client> Clients() //on crée une liste contenant tous les clients de la base de donnée
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from client order by nom_C;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            List<Client> clients = new List<Client>();
            while (reader.Read())
            {
                clients.Add(new Client(reader));
            }
            reader.Close();

            connection.Close();
            return clients;

        }
        #endregion

        #region identification client

        public string IdentificationClient()
        {
            List<string> id = new List<string>();
            foreach (Client c in this.Clients())
            {
                id.Add(c.IdClient);
            }
            string IDC = "";
            do
            {
                Console.WriteLine("Veuillez saisir votre identifiant :");
                IDC = Console.ReadLine();
            } while (id.Contains(IDC) == false);

            
            return IDC;
        }

        public Client BDClient(string IDC)
        {
            Client client_BD = new Client();
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Client WHERE idClient = @id;";
            command.Parameters.AddWithValue("@id", IDC);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                client_BD = new Client(IDC, reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetBoolean(6), reader.GetInt32(7), reader.GetInt32(8), reader.GetString(9));
            }
            reader.Close();

            connection.Close();

            return client_BD;
        } 

        public Client CreationClient()
        {
            List<Client> clients = this.Clients();
            List<string> id = new List<string>();
            foreach (Client c in clients)
            {
                id.Add(c.IdClient);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            // associer un identifiant au client
            string idc;
            do { Console.WriteLine("Choisissez un Id tel que A****"); Console.ForegroundColor = ConsoleColor.White; idc = Console.ReadLine(); } while (id.Contains(idc));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir votre prénom : ");
            Console.ForegroundColor = ConsoleColor.White;
            string prenom = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir votre nom : ");
            Console.ForegroundColor = ConsoleColor.White;
            string nom = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir votre numéro de téléphone tel que 0633333333: ");
            Console.ForegroundColor = ConsoleColor.White;
            string num = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir votre numéro de compte bancaire: ");
            Console.ForegroundColor = ConsoleColor.White;
            string num_compte = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir votre adresse : ");
            Console.ForegroundColor = ConsoleColor.White;
            string adr = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Etes-vous créateur de recettes ? : tapez oui ou non ");
            Console.ForegroundColor = ConsoleColor.White;
            string s = Console.ReadLine().ToLower();
            bool cdr = false;
            if (s == "oui") { cdr = true; }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Etes-vous cuisinier ? : tapez oui ou non ");
            Console.ForegroundColor = ConsoleColor.White;
            string s2 = Console.ReadLine().ToLower();
            string cuis = "0";
            // l'id cuisinier se termine pareil que l'idClient
            if (s2 == "oui") { cuis = idc.Replace("A", "C"); }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("On vous associe alors l'id cuisinier suivant : " + cuis);
            Client newc = new Client(idc, prenom, nom, num, num_compte, adr, cdr, 0, 0, cuis);

            //insertion dans la BD
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO client Values (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10)";
            command.Parameters.AddWithValue("@p1", newc.IdClient);
            command.Parameters.AddWithValue("@p2", newc.Prenom_C);
            command.Parameters.AddWithValue("@p3", newc.Nom_C);
            command.Parameters.AddWithValue("@p4", newc.Num_tel);
            command.Parameters.AddWithValue("@p5", newc.Num_compte);
            command.Parameters.AddWithValue("@p6", newc.Adresse);
            command.Parameters.AddWithValue("@p7", newc.Statut_Cdr);
            command.Parameters.AddWithValue("@p8", newc.Nbr_recette_com);
            command.Parameters.AddWithValue("@p9", newc.Solde_cook);
            command.Parameters.AddWithValue("@p10", newc.IdCuisinier);
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            connection.Close();

            return newc;
        }

        #region IdentificationCdR 
        
        public void IdentificationCdR(string IDC)
        {
            //on récupère les données du client dont l'identifiant est IDC
            Client c0 = BDClient(IDC);

            //on demande à l'utilisateur s'il veut devenir CdR
            if (c0.Statut_Cdr == false)
            {
                string i = "";
                do
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Voulez-vous être CdR ?(oui/non) ");
                    Console.ForegroundColor = ConsoleColor.White;
                    i = Console.ReadLine().ToLower();
                } while (i != "oui" && i != "non");
                if (i == "oui") 
                {
                    if (c0.Statut_Cdr == false)
                    {
                        c0.Statut_Cdr = true;
                        string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        connection.Open();
                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "UPDATE client SET statut_cdr = REPLACE(statut_cdr, false, true) WHERE idClient = @id;";
                        command.Parameters.AddWithValue("@id", IDC);
                        MySqlDataReader reader;
                        reader = command.ExecuteReader();
                        reader.Close();
                        connection.Close();
                    }
                }
                
            }
            //on accède aux fonctionnalités de CdR-cuisinier en vérifiant qu'il soit bien cdr
            int j = 0;
            if (c0.Statut_Cdr)
            {
                string fin = "non";
                do
                {
                    do
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Vous êtes bien créateur de recettes. Que voulez-vous faire ? ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(" saisir une nouvelle recette : tapez 1 \n consulter votre solde : tapez 2 \n afficher la liste de vos recettes : tapez 3");
                        j = Convert.ToInt32(Console.ReadLine());
                    } while (j != 1 && j != 2 && j != 3);

                    Recette r = new Recette();
                    if (j == 1)
                    {
                        r.SaisieRecette(IDC);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Voulez-vous faire autre chose concernant les fonctionnalités CdR ? (oui/non) ");
                        Console.ForegroundColor = ConsoleColor.White;
                        fin = Console.ReadLine().ToLower();
                    }
                    else
                    {
                        if (j == 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("Votre solde cook est de : " + c0.Solde_cook);
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Voulez-vous faire autre chose concernant les fonctionnalités CdR ? (oui/non) ");
                            Console.ForegroundColor = ConsoleColor.White;
                            fin = Console.ReadLine().ToLower();
                        }
                        else
                        {
                            if (j == 3)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                r.AffichageListe(IDC);
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Voulez-vous faire autre chose concernant les fonctionnalités CdR ? (oui/non) ");
                                Console.ForegroundColor = ConsoleColor.White;
                                fin = Console.ReadLine().ToLower();
                            }

                        }
                    }


                } while (fin == "oui");

            }

            //reader.Close();

        }

        #endregion

        #endregion
        

    }


    




}





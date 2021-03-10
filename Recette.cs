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
    public class Recette
    {
        #region attributs
        private string idRecette, nom_R, type, descriptif, idClient;
        private int prix_vente, nb_commande, temps_conservation;
        #endregion

        #region constructeur

        public Recette(string idRecette = "0", string nom_R = "0", string type = "0", string descriptif = "0", int prix_vente = 0, int nb_commande = 0, int temps_conservation = 0, string idClient = "0")
        {
            this.idRecette = idRecette;
            this.nom_R = nom_R;
            this.type = type;
            this.descriptif = descriptif;
            this.prix_vente = prix_vente;
            this.nb_commande = nb_commande;
            this.temps_conservation = temps_conservation;
            this.idClient = idClient;
        }

        public Recette(MySqlDataReader reader)
        {
            this.idRecette = reader.GetString(0);
            this.nom_R = reader.GetString(1);
            this.type = reader.GetString(2);
            this.descriptif = reader.GetString(3);
            this.prix_vente = reader.GetInt32(4);
            this.nb_commande = reader.GetInt32(5);
            this.temps_conservation = reader.GetInt32(6);
            this.idClient = reader.GetString(7);
        }

        #endregion

        #region proprietes

        public string IdRecette
        {
            get { return this.idRecette; }
            set { this.idRecette = value; }
        }

        public string Nom_R
        {
            get { return this.nom_R; }
            set { this.nom_R = value; }
        }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string Descriptif
        {
            get { return this.descriptif; }
            set { this.descriptif = value; }
        }

        public int Prix_vente
        {
            get { return this.prix_vente; }
            set { this.prix_vente = value; }
        }

        public int Nb_commande
        {
            get { return this.nb_commande; }
            set { this.nb_commande = value; }
        }

        public int Temps_conservation
        {
            get { return this.temps_conservation; }
            set { this.temps_conservation = value; }
        }

        public string IdClient
        {
            get { return this.idClient; }
            set { this.idClient = value; }
        }

        #endregion

        #region affichage

        public override string ToString()
        {
            return this.nom_R + " est un(e) " + this.type + " d'ID " + this.IdRecette + ".\n Son créateur " + this.idClient + " la décrit comme : " + this.descriptif + " et peut se conserver " + this.temps_conservation + "jours.\n Elle est proposée à " + this.prix_vente + " cooks et a déjà été vendue à " + this.nb_commande + " exemplaires.";
        }

        #endregion

        #region BDD -> C#

        public List<Recette> Recettes() //on crée une liste contenant tous les clients de la base de donnée
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from Recette order by idrecette;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            List<Recette> recettes = new List<Recette>();

            while (reader.Read())
            {
                recettes.Add(new Recette(reader));
            }

            connection.Close();
            return recettes;

        }

        #endregion

        //on affecte les valeurs de la base de donnée au client correspondant
        public Recette BDRecette(string IDR)
        {
            Recette recette_BD = new Recette();
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from Recette where idRecette = @id;";
            command.Parameters.AddWithValue("@id", IDR);
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string nom, typ, descrip, idC;
            int prix, nb, temps;
            while (reader.Read())
            {
                nom = reader.GetString(1);
                typ = reader.GetString(2);
                descrip = reader.GetString(3);
                prix = reader.GetInt32(4);
                nb = reader.GetInt32(5);
                temps = reader.GetInt32(6);
                idC = reader.GetString(7);
                recette_BD = new Recette(IDR, nom, typ, descrip, prix, nb, temps, idC);
            }
            reader.Close();

            connection.Close();

            return recette_BD;
        }

        public void ModifSiCommande(string idRec, int q)
        {
            //modification du prix de vente selon le nombre de commandes
            //le nombre de commandes augmente à chaque fois qu'on appelle cette fonction
            Recette r0 = BDRecette(idRec);
            r0.Nb_commande = r0.Nb_commande + q;
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            int aug = 0;
            if (r0.Nb_commande > 10) { r0.Prix_vente += 2; }
            if (r0.Nb_commande > 50) { r0.Prix_vente += 5; aug = 2; }
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Recette SET prix_vente = @prix , nb_commande = @nbr WHERE idRecette = @id;";
            command.Parameters.AddWithValue("@prix", r0.Prix_vente);
            command.Parameters.AddWithValue("@nbr", r0.Nb_commande);
            command.Parameters.AddWithValue("@id", r0.IdRecette);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            //modification du solde cook et du nombre de recettes commandées créées par le CdR
            MySqlCommand command2 = connection.CreateCommand();
            command2.CommandText = "select solde_cook,nbr_recette_com_CdR from Client where idClient = @id;";
            command2.Parameters.AddWithValue("@id", r0.IdClient);
            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            int s = 0;
            int n=0;
            while (reader2.Read())
            {
                s = reader2.GetInt32(0);
                s = s + 2 + aug; //on fixe la remuneration du cdr à 2 cooks si nb_commandes < 50
                n = reader2.GetInt32(1);
                n = n + 1;
            }
            reader2.Close();

            //la BD est modifiée
            MySqlCommand command3 = connection.CreateCommand();
            command3.CommandText = "UPDATE Client SET solde_cook = @sol , nbr_recette_com_CdR = @nbr WHERE idClient = @id;";
            command3.Parameters.AddWithValue("@sol", s);
            command3.Parameters.AddWithValue("@nbr", n);
            command3.Parameters.AddWithValue("@id", r0.IdClient);
            MySqlDataReader reader3;
            reader3 = command3.ExecuteReader();
            connection.Close();
        }

        public Recette SaisieRecette(string idc)
        {
            List<Recette> recettes = this.Recettes();
            List<string> id = new List<string>();
            foreach (Recette r in recettes)
            {
                id.Add(r.IdRecette);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            string idr;
            do { Console.WriteLine("Choisissez un identifiant de recette tel que R****"); Console.ForegroundColor = ConsoleColor.White; idr = Console.ReadLine(); } while (id.Contains(idr));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez nommer cette recette : ");
            Console.ForegroundColor = ConsoleColor.White;
            string nom = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez donner un type à cette recette : ");
            Console.ForegroundColor = ConsoleColor.White;
            string t = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez décrire cette recette : ");
            Console.ForegroundColor = ConsoleColor.White;
            string descrip = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez donner un prix de vente (prix entier): ");
            Console.ForegroundColor = ConsoleColor.White;
            int prix = Convert.ToInt32(Console.ReadLine());
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Veuillez saisir le nombre de jours où cette recette peut être conservée : ");
            Console.ForegroundColor = ConsoleColor.White;
            int temps = Convert.ToInt32(Console.ReadLine());
            Recette newr = new Recette(idr, nom, t, descrip, prix, 0, temps, idc);
            

            //insertion dans la BD
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Recette Values (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)";
            command.Parameters.AddWithValue("@p1", newr.IdRecette);
            command.Parameters.AddWithValue("@p2", newr.Nom_R);
            command.Parameters.AddWithValue("@p3", newr.Type);
            command.Parameters.AddWithValue("@p4", newr.Descriptif);
            command.Parameters.AddWithValue("@p5", newr.Prix_vente);
            command.Parameters.AddWithValue("@p6", newr.Nb_commande);
            command.Parameters.AddWithValue("@p7", newr.Temps_conservation);
            command.Parameters.AddWithValue("@p8", newr.IdClient);
            MySqlDataReader reader2;
            reader2 = command.ExecuteReader();
            connection.Close();

            //liste ingrédients

            ListeIngredients l = new ListeIngredients();
            string i = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Voulez-vous ajouter un ingrédient à la liste ? (oui/non)");
                Console.ForegroundColor = ConsoleColor.White;
                i = Console.ReadLine().ToLower();
                if (i == "oui") { l.liste(idr); }
            } while (i != "non" );

            return newr;
        }

        public void AffichageListe(string idc)
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select nom_R, nb_commande from Recette where idClient = @id;";
            command.Parameters.AddWithValue("@id", idc);

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string nom;
            int nb;
            bool boucle = false;
            while (reader.Read())
            {
                nom = reader.GetString(0);
                nb = reader.GetInt32(1);
                Console.WriteLine("Votre recette " + nom + " a été commandée " + nb + " fois.");
                boucle = true;
            }
            if (boucle == false) { Console.WriteLine("\nVous n'avez pas encore de recettes! Mais vous pouvez en saisir une dès maintenant : tapez 'oui' puis '1'.\n"); }

            connection.Close();
          
        }
        
    }
}


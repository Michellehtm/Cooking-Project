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
    public class Commande
    {
        #region attributs
        private string idCommande;
        private int prixCommande;
        private DateTime date_Commande;
        private string idClient;
        #endregion

        #region constructeur
        public Commande(string idCommande="0", int prixCommande=0,DateTime date_Commande= new DateTime(), string idClient="0")
        {
            this.idCommande = idCommande;
            this.prixCommande = prixCommande;
            this.date_Commande = date_Commande;
            this.idClient = idClient;
        }

        public Commande(MySqlDataReader reader)
        {
            this.idCommande = reader.GetString(0);
            this.prixCommande = reader.GetInt32(1);
            this.date_Commande = reader.GetDateTime(2);
            this.idClient = reader.GetString(3);
        }
        #endregion

        #region proprietes
        public string IdCommande
        {
            get { return this.idCommande; }
            set { this.idCommande = value; }
        }

        public int PrixCommande
        {
            get { return this.prixCommande; }
            set { this.prixCommande = value; }
        }

        public DateTime Date_Commande
        {
            get { return this.date_Commande; }
            set { this.date_Commande = value; }
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
            return "La commande " + this.idCommande + " d'un total de " + this.prixCommande + " a été passée le " + this.date_Commande;
        }

        #endregion

        #region BDD -> C#

        public List<Commande> Commandes() //on crée une liste contenant toutes les commandes de la base de donnée
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from Commande order by idCommande;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            List<Commande> commandes = new List<Commande>();

            while (reader.Read())
            {
                commandes.Add(new Commande(reader));
            }

            connection.Close();
            return commandes;

        }

        #endregion

        public void DecrementationSolde()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select solde_cook from Client where idClient = @id;";
            command.Parameters.AddWithValue("@id", this.idClient);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int s = 0;
            while (reader.Read()) { s = reader.GetInt32(0); }
            reader.Close();
            if (s > this.prixCommande)
            {
                s = s - this.prixCommande;
                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "UPDATE Client SET solde_cook = @sol WHERE idClient = @id;";
                command2.Parameters.AddWithValue("@sol", s);
                command2.Parameters.AddWithValue("@id", this.IdClient);
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();
                Console.WriteLine("\n Le payement a été effectué avec succès. Merci!");
            }
            else 
            { 
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Oops il y a une erreur... Etes-vous vraiment CdR ? Il se peut que votre solde soit insuffisant donc nous allons proceder à un payement classique. Le total de votre commande a été converti à " + this.prixCommande + " euros.");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            connection.Close();
        }
    }

}

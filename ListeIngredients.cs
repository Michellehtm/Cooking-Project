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
    public class ListeIngredients
    {
        #region attributs
        private string idProduit, idRecette;
        private int unite_quantite;
        #endregion

        #region constructeur
        public ListeIngredients(string idProduit = "0", string idRecette = "0", int unite_quantite = 0)
        {
            this.idProduit = idProduit;
            this.idRecette = idRecette;
            this.unite_quantite = unite_quantite;
        }

        public ListeIngredients(MySqlDataReader reader)
        {
            this.idProduit = reader.GetString(0);
            this.idRecette = reader.GetString(1);
            this.unite_quantite = reader.GetInt32(2);
        }
        #endregion

        #region proprietes

        public string IdProduit
        {
            get { return this.idProduit; }
            set { this.idProduit = value; }
        }

        public string IdRecette
        {
            get { return this.idRecette; }
            set { this.idRecette = value; }
        }

        public int Unite_Quantite
        {
            get { return this.unite_quantite; }
            set { this.unite_quantite = value; }
        }
        #endregion

        #region affichage

        public override string ToString()
        {
            return "La recette" + this.idRecette + " necessite les ingrédients " + this.idProduit + " en quantité " + this.unite_quantite;
        }

        #endregion

        #region BDD -> C#

        public List<ListeIngredients> ListeIngredient() //on crée une liste contenant tous les fournisseurs de la base de donnée
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from listeIngredients;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            List<ListeIngredients> listeIngredients = new List<ListeIngredients>();

            while (reader.Read())
            {
                listeIngredients.Add(new ListeIngredients(reader));
            }

            connection.Close();
            return listeIngredients;

        }

        #endregion

        public void DecrementationStock(string idRec, int q)
        {
            foreach (ListeIngredients l in this.ListeIngredient())
            {
                string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select stock_actuel from produit natural join listeingredients where idRecette=@p1 and idProduit=@p2;";
                command.Parameters.AddWithValue("@p1", idRec);
                MySqlDataReader reader;
                string ingred;
                int quantite;
                if (l.IdRecette == idRec)
                {
                    int stock = 0;
                    ingred = l.IdProduit;
                    quantite = l.Unite_Quantite;
                    command.Parameters.AddWithValue("@p2", ingred);
                    reader = command.ExecuteReader();
                    while (reader.Read()) { stock = reader.GetInt32(0) - (quantite*q); }
                    reader.Close();
                    MySqlCommand command2 = connection.CreateCommand();
                    command2.CommandText = "UPDATE Produit SET stock_actuel = @s WHERE idProduit = @p2;";
                    command2.Parameters.AddWithValue("@s", stock);
                    command2.Parameters.AddWithValue("@p2", ingred);
                    MySqlDataReader reader2;
                    reader2 = command2.ExecuteReader();
                    reader2.Close();
                }
                connection.Close();
            }
        }

        public bool DecrementationStockTest(string idRec,int q)
        {
            bool ok = true;
            foreach (ListeIngredients l in this.ListeIngredient())
            {
                string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select stock_actuel from produit natural join listeingredients where idRecette=@p1 and idProduit=@p2;";
                command.Parameters.AddWithValue("@p1", idRec);
                MySqlDataReader reader;
                string ingred;
                int quantite;
                if (l.IdRecette == idRec)
                {
                    int stock = 0;
                    ingred = l.IdProduit;
                    quantite = l.Unite_Quantite;
                    command.Parameters.AddWithValue("@p2", ingred);
                    reader = command.ExecuteReader();
                    while (reader.Read()) { stock = reader.GetInt32(0) - (quantite*q); }
                    reader.Close();
                    if (stock < 0) { ok = false; }
                }
                connection.Close();
            }
            return ok;
        }

        public ListeIngredients liste(string idr)
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand com = connection.CreateCommand();
            com.CommandText = "SELECT nom_p, idproduit From Produit order by idProduit;";
            MySqlDataReader read;
            read = com.ExecuteReader();
            List<string> ids = new List<string>();
            while (read.Read())
            {
                ids.Add(read.GetString(1));
                Console.WriteLine("Le produit " + read.GetString(0) + " a pour identifiant " + read.GetString(1));
            }
            read.Close();
            string idP = ""; int i = 0;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Veuillez saisir l'identifiant du produit choisi et sa quantité nécessaire pour la recette " + idr + " : ");
                Console.ForegroundColor = ConsoleColor.White;
                idP = Console.ReadLine();
                i = Convert.ToInt32(Console.ReadLine());
            } while (ids.Contains(idP) == false);
            
            ListeIngredients newl = new ListeIngredients(idP, idr, i);

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO ListeIngredients Values (@p1,@p2,@p3)";
            command.Parameters.AddWithValue("@p1", newl.IdProduit);
            command.Parameters.AddWithValue("@p2", newl.IdRecette);
            command.Parameters.AddWithValue("@p3", newl.Unite_Quantite);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();
            connection.Close();

            return newl;
        }
        
    }
}


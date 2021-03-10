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
using System.Globalization;
using System.Xml;

namespace DM_Cooking
{
    public class Program
    {

        #region Accueil Client
        public static void AccueilClient()
        {
            Client c = new Client();

            //Page d'accueil
            int a = 0;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Avez-vous déjà un compte ?");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" 1 : vous identifier \n 2 : créer un compte");
                Console.ForegroundColor = ConsoleColor.White;
                a = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
            } while (a != 1 && a != 2);

            //Creation d'un nouveau compte client
            if (a == 2)
            {
                c.CreationClient();
            }

            //Demande d'identification à la suite des 2 cas
            string idCli = c.IdentificationClient();
            int[] input = new int[] { 1, 2 };
            List<int> choix = new List<int>(input);
            int i = 0;
            string cont = "oui";
            do
            {
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Que voulez-vous faire ?");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" * Connaitre les informations liées à votre statut CdR : tapez 1 \n * Passer une commande : tapez 2");
                    Console.ForegroundColor = ConsoleColor.White;
                    i = int.Parse(Console.ReadLine());
                } while (choix.Contains(i) == false);
                if (i == 1) { c.IdentificationCdR(idCli); }
                //passage d'une commande
                if (i == 2)
                {
                    Console.Clear();
                    string rep0 = "oui";
                    do
                    {
                        string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        connection.Open();

                        //Récupération des données
                        string idCom = GenerationID();
                        string idRec = ChoixRecette();
                        if (idRec != "")
                        {
                            int quantite = ChoixQuantite(idRec);
                            int prix = DetPrix(idRec, quantite);
                            DateTime date = DateTime.Now;

                            Commande com = new Commande(idCom, prix, date, idCli);

                            //Paiement
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine("\n Votre commande constitue un total de " + prix + " cooks. Nous allons procéder au paiement.\nSi vous êtes CdR tapez 'oui' car vous pouvez payer en points cook.");
                            Console.ForegroundColor = ConsoleColor.White;
                            string rep = Console.ReadLine().ToLower();
                            if (rep == "oui") { com.DecrementationSolde(); }

                            //Modification des données de la recete commandée
                            Recette r = new Recette();
                            //r.BDRecette(idRec);
                            r.ModifSiCommande(idRec, quantite);

                            //Decrementation du stock
                            ListeIngredients liste = new ListeIngredients();
                            liste.DecrementationStock(idRec, quantite);

                            //ajout de la commande dans la BD
                            MySqlCommand command = connection.CreateCommand();
                            command.CommandText = "INSERT INTO Commande Values (@p1,@p2,@p3,@p4)";
                            command.Parameters.AddWithValue("@p1", idCom);
                            command.Parameters.AddWithValue("@p2", prix);
                            command.Parameters.AddWithValue("@p3", date);
                            command.Parameters.AddWithValue("@p4", idCli);
                            MySqlDataReader reader;
                            reader = command.ExecuteReader();
                            reader.Close();

                            //Création du nouvel objet de la classe Contient
                            MySqlCommand command3 = connection.CreateCommand();
                            command3.CommandText = "INSERT INTO contient Values (@p1,@p2,@p3)";
                            command3.Parameters.AddWithValue("@p1", idCom);
                            command3.Parameters.AddWithValue("@p2", idRec);
                            command3.Parameters.AddWithValue("@p3", quantite);
                            MySqlDataReader reader3;
                            reader3 = command3.ExecuteReader();
                            reader3.Close();


                            connection.Close();
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine("Tout a été finalisé! Récapitulons :\n" + com + "\nVous recevrez votre commande dans les plus brefs délais! Voulez-vous passer une autre commande ? (oui/non) \n");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            rep0 = Console.ReadLine().ToLower();
                        }
                        else { rep0 = "non"; }
                        
                    }
                    while (rep0 == "oui");
                }
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Voulez-vous faire autre chose en tant que client ? (oui/non)");
                Console.ForegroundColor = ConsoleColor.White;
                cont = Console.ReadLine().ToLower();
                Console.Clear();
            } while (cont == "oui");

            //Environment.Exit(0);
        }
        #endregion

        #region Passage Commande

        public static string GenerationID()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select idCommande from Commande order by idCommande;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            List<string> ids = new List<string>();

            while (reader.Read())
            {
                ids.Add(reader.ToString());
            }
            reader.Close();
            Random rand = new Random();
            string id = "COM0001";
            do
            {
                id = "COM0000";
                int nbr = rand.Next(1000, 10000);
                id = id.Replace("0000", nbr.ToString());
            }
            while (ids.Contains(id));
            connection.Close();
            return id;
        }

        public static string ChoixRecette()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Bonjour et bienvenue dans le menu.");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Voici les recettes disponibles : \n");
            

            Recette r = new Recette();
            ListeIngredients l = new ListeIngredients();
            List<string> id = new List<string>();
            
            foreach (Recette rec in r.Recettes())
            { 
                if (l.DecrementationStockTest(rec.IdRecette,1))
                {
                    id.Add(rec.IdRecette);
                    Console.WriteLine(rec + "\n");
                }
            }

            string idr = "";
            if (id.Count() != 0)
            {
                do
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Quel plat vous ferait-il plaisir aujourd'hui ? Saisissez l'id de la recette");
                    Console.ForegroundColor = ConsoleColor.White;
                    idr = Console.ReadLine();
                } while (id.Contains(idr) == false);
            }
            else { Console.WriteLine("Oops, il n'y a plus aucune recette disponible... On vous attend après le restock"); }
            return idr;
        }

        public static int ChoixQuantite(string IDR)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Combien de ce produit desirez vous commander ?");
            Console.ForegroundColor = ConsoleColor.White;
            int q = Convert.ToInt32(Console.ReadLine());
            ListeIngredients l = new ListeIngredients();
            do
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Oops le stock est insuffisant pour commander autant de cette recette... Choisissez une quantité plus faible...");
                Console.ForegroundColor = ConsoleColor.White;
                q = Convert.ToInt32(Console.ReadLine());
            } while (l.DecrementationStockTest(IDR, q) == false);

            return q;
        }

        public static int DetPrix(string a, int b)
        {
            int prix = 0;
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select prix_vente from Recette Where idRecette = @id;";
            command.Parameters.AddWithValue("@id", a);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read()) { int s = reader.GetInt32(0); prix = s * b; }
            return prix;
        }

        #endregion

        #region Gestionnaire de Cooking
        public static void CdrSemaine()
        {
            DateTime dateajd = DateTime.Now;
            Commande c = new Commande();
            List<Commande> com = c.Commandes();

            // Permet de récupérer le numéro de la semaine de la date actuelle (Now) ATTENTION ON A IMPORTE SYSTEM.GLOBALIZATION
            Calendar calendrier = CultureInfo.InvariantCulture.Calendar;
            int semaine = calendrier.GetWeekOfYear(dateajd, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // On crée une liste avec toutes les commandes qui ont été passées dans la même semaine que Now
            List<Commande> com2 = new List<Commande>();
            foreach (Commande commande in com)
            {
                if ((commande.Date_Commande.Year == dateajd.Year) && (calendrier.GetWeekOfYear(commande.Date_Commande, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)==semaine))
                {
                    com2.Add(commande);
                }
            }
            //On n'effectue la recherche que si la liste n'est pas vide
            List<string> ids = new List<string>();
            if (com2.Any())
            {
                foreach (Commande commande in com2)
                {
                    string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "select idClient from client natural join recette natural join contient where idcommande = @id ;";
                    comm.Parameters.AddWithValue("@id", commande.IdCommande);
                    MySqlDataReader read;
                    read = comm.ExecuteReader();
                    while (read.Read())
                    {
                        ids.Add(read.GetString(0));
                    }
                    read.Close();
                    connection.Close();
                }
                var query = ids.GroupBy(val => val).OrderByDescending(grp => grp.Count()).ThenBy(grp => grp.Key);
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Voici les ventes de la semaine : \n");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                int a = 0;
                List<string> cdrsem = new List<string>();
                foreach (var item in query)
                {
                    Console.WriteLine("- {0} a vendu {1} commande(s)\n", item.Key, item.Count());
                    if (item.Count() == a) { a = item.Count(); cdrsem.Add(item.Key); }
                    if (item.Count() > a) { a = item.Count(); cdrsem.Clear(); cdrsem.Add(item.Key); }
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (cdrsem.Count() == 1) { foreach (string s in cdrsem) { Console.WriteLine("\nLe CdR de la semaine est le client : " + s); } }
                else
                {
                    Console.WriteLine("On a plusieurs gagnants cette semaine: \n");
                    foreach(string s in cdrsem) { Console.WriteLine(s); }
                }
            }
            else { Console.WriteLine("Il n'y a pas eu de ventes cette semaine désolé... revenez plus tard!"); }
        }

        public static void Top5_recettes()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom_R, type,nb_commande, prenom_C, nom_C FROM Recette NATURAL JOIN  Client ORDER BY nb_commande DESC LIMIT 5 ;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int a = 1;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Liste des 5 recettes les plus commandées :\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            while (reader.Read())
            {
                Console.WriteLine(a + "- " + reader.GetString(0) + " est une recette de type : " + reader.GetString(1) + " et commandée " + reader.GetString(2) + " fois! \nC'est une recette de : " + reader.GetString(3) + " " + reader.GetString(4) + "\n");
                a += 1;
            }
            reader.Close();
            connection.Close();
        }

        public static string CdROr()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select idClient, nom_C, prenom_C from recette natural join client where nb_commande=(select max(nb_commande) from recette);";


            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string id = "";
            string nom = "";
            string prenom = "";
            while (reader.Read())
            {
                id = reader.GetString(0);
                nom = reader.GetString(1);
                prenom = reader.GetString(2);
            }
            reader.Close();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Le CdR d'or est : " + nom + " " + prenom + " dont l'identifiant est " + id);
            Console.ForegroundColor = ConsoleColor.White;

            return id;

        }

        public static void BestRecettes(string id)
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT idRecette, nom_R, type FROM Recette natural join client WHERE idClient=@id ORDER BY nb_commande DESC LIMIT 5 ;";
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int a = 1;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("TOP 5 de ses recettes les plus commandées :\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            while (reader.Read())
            {
                Console.WriteLine(a + "- " + reader.GetString(1) + " est une recette de type : " + reader.GetString(2) + " et d'id " + reader.GetString(0) + ". \n");
                a += 1;
            }

            reader.Close();
            connection.Close();
        }

        public static void TableaudeBordSemaine()
                {
                    int[] input = new int[] { 1, 2, 3 };
                    List<int> rep = new List<int>(input);
                    int rep0 = 0;
                    do
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("Que voulez-vous faire?");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(" * connaitre le CdR de la semaine : tapez 1 \n * connaitre le top 5 des recettes : tapez 2 \n * connaitre le CdR d'or : tapez 3 ");
                        Console.ForegroundColor = ConsoleColor.White;
                        rep0 = int.Parse(Console.ReadLine());
                        Console.Clear();
                    } while (rep.Contains(rep0) == false);

                    if (rep0 == 1) { CdrSemaine(); }
                    else
                    {
                        if (rep0 == 2) { Top5_recettes(); }
                        else
                        {
                            if (rep0 == 3) { BestRecettes(CdROr()); }
                        }
                    }
                }

        public static void MajQuantite()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            DateTime dateajrd = new DateTime(2020, 4, 6);
            DateTime datelimite = dateajrd.AddDays(-30);

            List<string> ids = new List<string>();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select produit.idproduit from produit left join (select listeingredients.idproduit , max(date_commande) as date_commande from commande natural join contient natural join listeingredients group by idProduit) as tab on tab.idproduit=produit.idproduit where date_commande is null or date_commande < @date ;";
            command.Parameters.AddWithValue("@date", datelimite);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read()) { ids.Add(reader.GetString(0)); }
            reader.Close();

            Console.WriteLine("Voici les produits qui n'ont pas été utilisés depuis plus de 30jours :\n");
            foreach (string id in ids) { Console.WriteLine("- " + id + "\n"); }

            string go = "";
            do
            {
                Console.WriteLine("\nNous allons procéder à la modification des stocks. Tapez 'ok' pour donner le feu vert, 'stop' sinon");
                go = Console.ReadLine().ToLower();

            } while (go != "ok" && go != "stop");

            if (go == "ok")
            {
                foreach (string id in ids)
                {
                    MySqlCommand command2 = connection.CreateCommand();
                    command2.CommandText = "update produit set stock_min=Replace(stock_min, stock_min, stock_min/2), stock_max=Replace(stock_max, stock_max, stock_max/2) where idProduit = @id;";
                    command2.Parameters.AddWithValue("@id", id);
                    MySqlDataReader reader2;
                    reader2 = command2.ExecuteReader();
                    reader2.Close();
                }
                Console.WriteLine("Le stock a été mis à jour avec succès!");
            }

            connection.Close();
        }

        public static bool Write_XML()
        {
            bool ok = false;
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select idFournisseur, nom_F,num_tel_F, idproduit, nom_P, stock_max, stock_actuel from Produit natural join fournit_stock natural join fournisseur where stock_actuel<stock_min order by fournit_stock.idFournisseur;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            XmlDocument docXml = new XmlDocument();


            //création élément racine
            XmlElement racine = docXml.CreateElement("Commande");
            docXml.AppendChild(racine);

            //création et insertion de l'en-tête XML
            XmlDeclaration xmldec1 = docXml.CreateXmlDeclaration("1.0", "UTF-8", "no");
            docXml.InsertBefore(xmldec1, racine);

            //Commentaire en XML
            XmlComment com = docXml.CreateComment("Liste des produits dont le stock actuel est inférieur au stock minimal");
            docXml.InsertAfter(com, racine);


            while (reader.Read())
            {

                XmlElement autreBalise1 = docXml.CreateElement("Fournisseur");
                XmlElement idF = docXml.CreateElement("idFournisseur");
                idF.InnerText = " " + reader.GetString(0) + " ";
                autreBalise1.AppendChild(idF);
                XmlElement nomF = docXml.CreateElement("NomFournisseur");
                nomF.InnerText = " " + reader.GetString(1) + " ";
                autreBalise1.AppendChild(nomF);
                XmlElement num = docXml.CreateElement("num_tel");
                num.InnerText = " " + reader.GetString(2) + " ";
                autreBalise1.AppendChild(num);

                XmlElement autreBalise2 = docXml.CreateElement("Produit");
                autreBalise1.AppendChild(autreBalise2);
                XmlElement idP = docXml.CreateElement("idProduit");
                idP.InnerText = " " + reader.GetString(3) + " ";
                autreBalise2.AppendChild(idP);
                XmlElement nom = docXml.CreateElement("nomProduit");
                nom.InnerText = " " + reader.GetString(4) + " ";
                autreBalise2.AppendChild(nom);
                XmlElement q = docXml.CreateElement("quantité_à_commander");
                q.InnerText = " " + Convert.ToString(reader.GetInt32(5) - reader.GetInt32(6)) + " ";
                autreBalise2.AppendChild(q);


                racine.AppendChild(autreBalise1);
                ok = true;
            }
            reader.Close();
            if (ok)
            {
                //docXml.InnerText = "<!-- Liste des produits dont la quantité commandée à chaque commande d'une recette est égale au stock max – le stock actuel -->";

                docXml.Save("listeproduits.xml");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("fichier listeproduitst.xml créé");
                Console.WriteLine("Vous pouvez le retouver dans le dossier bin/debug du projet Visual Studio.");
            }
            else { Console.WriteLine("Il n'y a aucun produit dont la quantité commandée à chaque commande d'une recette est égale au stock max – le stock actuel\nLe document listeproduits.xml n'a donc pas été créé/mis à jour pour cette semaine."); }
            return ok;
        }

        public static void Read_XML(bool ok)
        {
            if (ok)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("listeproduits.xml");
                XmlElement racine = doc.DocumentElement;

                XmlTextReader reader = new XmlTextReader("listeproduits.xml");
                int a = 0;
                Console.WriteLine("Veuillez trouver ci-dessous la liste des produits à commander et chez quel fournisseur vous pourrez les trouver: \n ");
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "idFournisseur") { a = 1; break; }
                            if (reader.Name == "NomFournisseur") { a = 2; break; }
                            if (reader.Name == "num_tel") { a = 3; break; }
                            if (reader.Name == "idProduit") { a = 4; break; }
                            if (reader.Name == "nomProduit") { a = 5; break; }
                            if (reader.Name == "quantité_à_commander") { a = 6; break; }
                            else { break; }
                        case XmlNodeType.Text:
                            if (a == 1) { Console.Write(" - Produit à commander au fournisseur (" + reader.Value); break; }
                            if (a == 2) { Console.Write(") " + reader.Value); break; }
                            if (a == 3) { Console.Write(" à contacter au " + reader.Value); break; }
                            if (a == 4) { Console.Write(". L'id du produit est : " + reader.Value); break; }
                            if (a == 5) { Console.Write(", il s'agit de " + reader.Value); break; }
                            if (a == 6) { Console.Write(" dont la quantité à commander est de : " + reader.Value + "kg." + "\n"); a = 0; break; }
                            else { break; }
                    }
                }
            }
        }

        public static void SuppressionRecette(string idR)
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM listeingredients WHERE idRecette=@id;";
            command.Parameters.AddWithValue("@id", idR);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            MySqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "DELETE FROM contient WHERE idRecette = @id;";
            command1.Parameters.AddWithValue("@id", idR);
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            reader1.Close();

            MySqlCommand command2 = connection.CreateCommand();
            command2.CommandText = "DELETE FROM prepare WHERE idRecette = @id;";
            command2.Parameters.AddWithValue("@id", idR);
            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            reader2.Close();

            MySqlCommand command3 = connection.CreateCommand();
            command3.CommandText = "DELETE FROM Recette WHERE idRecette = @id;";
            command3.Parameters.AddWithValue("@id", idR);
            MySqlDataReader reader3;
            reader3 = command3.ExecuteReader();
            reader3.Close();

            Console.WriteLine("Suppression de la recette sélectionnée effectuée.");



            connection.Close();
        }

        public static void SuppressionCuisinier()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand comm = connection.CreateCommand();
            comm.CommandText = "select * from client where idcuisinier != '0' order by idCuisinier;";
            MySqlDataReader read;
            read = comm.ExecuteReader();

            List<Client> cuis = new List<Client>();
            List<string> idcuisiniers = new List<string>();
            while (read.Read())
            {
                cuis.Add(new Client(read));
                idcuisiniers.Add(read.GetString(9));
            }
            read.Close();

            foreach (Client c in cuis) { Console.WriteLine(c + "\n"); }

            string idCuis = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Veuillez saisir l'id C**** du cuisinier que vous voulez supprimer :");
                Console.ForegroundColor = ConsoleColor.White;
                idCuis = Console.ReadLine();
            } while (idcuisiniers.Contains(idCuis) == false);
            string idC = idCuis.Replace("C", "A");

            //afin de supprimer un cuisinier (de la table cuisinier) et ses recettes,
            //il est nécessaire de supprimer toutes les données concernant ces recettes et l'idCuisinier car elles sont considérées comme clés étrangères
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM prepare WHERE idCuisinier =@id;";
            command.Parameters.AddWithValue("@id", idCuis);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            List<string> idr_supp = new List<string>();
            MySqlCommand command0 = connection.CreateCommand();
            command0.CommandText = "SELECT idRecette FROM Recette WHERE idClient=@id;";
            command0.Parameters.AddWithValue("@id", idC);
            MySqlDataReader reader0;
            reader0 = command0.ExecuteReader();
            while (reader0.Read())
            {
                idr_supp.Add(reader0.GetString(0));
            }
            reader0.Close();


            foreach (string rec in idr_supp)
            {

                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "DELETE FROM listeingredients WHERE idRecette=@i;";
                command2.Parameters.AddWithValue("@i", rec);
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();

                MySqlCommand command3 = connection.CreateCommand();
                command3.CommandText = "DELETE FROM contient WHERE idRecette=@i;";
                command3.Parameters.AddWithValue("@i", rec);
                MySqlDataReader reader3;
                reader3 = command3.ExecuteReader();
                reader3.Close();

                MySqlCommand command4 = connection.CreateCommand();
                command4.CommandText = "DELETE FROM prepare WHERE idRecette=@i;";
                command4.Parameters.AddWithValue("@i", rec);
                MySqlDataReader reader4;
                reader4 = command4.ExecuteReader();
                reader4.Close();
            }

            MySqlCommand command5 = connection.CreateCommand();
            command5.CommandText = "DELETE FROM Recette WHERE idClient=@id;";
            command5.Parameters.AddWithValue("@id", idC);
            MySqlDataReader reader5;
            reader5 = command5.ExecuteReader();
            reader5.Close();

            MySqlCommand command6 = connection.CreateCommand();
            command6.CommandText = "UPDATE client SET idCuisinier='0' WHERE idClient=@id;";
            command6.Parameters.AddWithValue("@id", idC);
            MySqlDataReader reader6;
            reader6 = command6.ExecuteReader();
            reader6.Close();

            MySqlCommand command7 = connection.CreateCommand();
            command7.CommandText = "DELETE FROM Cuisinier WHERE idCuisinier=@id;";
            command7.Parameters.AddWithValue("@id", idCuis);
            MySqlDataReader reader7;
            reader7 = command7.ExecuteReader();
            reader7.Close();

            Client c0 = new Client();
            c0 = c0.BDClient(idC);

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Suppression des recettes associées au cuisinier " + idCuis + " effectuée.");
            Console.BackgroundColor = ConsoleColor.Black;
            string i = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Voulez-vous que le cuisinier " + c0.Nom_C + " " + c0.Prenom_C + " d'ID " + idCuis + " reste client chez Cooking ? (oui/non)");
                Console.ForegroundColor = ConsoleColor.White;
                i = Console.ReadLine().ToLower();
            } while (i != "oui" && i != "non");
            if (i == "oui")
            {
                MySqlCommand command8 = connection.CreateCommand();
                command8.CommandText = "UPDATE client SET idCuisinier='0', statut_CdR=false,nbr_recette_com_CdR=0 WHERE idClient =@id;";
                command8.Parameters.AddWithValue("@id", idC);
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Le cuisinier " + idCuis + " ne possède plus le statut de cuisinier chez Cooking mais il reste client.");
                Console.BackgroundColor = ConsoleColor.Black;
                MySqlDataReader reader8;
                reader8 = command8.ExecuteReader();


                reader8.Close();
            }
            else
            {
                MySqlCommand command9 = connection.CreateCommand();
                command9.CommandText = "DELETE FROM client where idClient =@id;";
                command9.Parameters.AddWithValue("@id", idC);
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Le cuisinier-client " + idCuis + " ne fait plus parti de l'entreprise Cooking.");
                Console.BackgroundColor = ConsoleColor.Black;
                MySqlDataReader reader9;
                reader9 = command9.ExecuteReader();

                reader9.Close();
            }

            connection.Close();
        }
        
        public static void GestionnaireCooking()
        {
            Console.WriteLine("Bienvenue sur le gestionnaire de Cooking ! ");

            string fin = "non";
            do
            {
                int[] input = new int[] { 1, 2, 3, 4 };
                List<int> rep = new List<int>(input);
                int rep0 = 0;
                do
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Que voulez-vous faire?");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" * Accéder au tableau de bord de la semaine : tapez 1 \n * Faire le réapprovisionnement hebdomadaire des produits : tapez 2 \n * Supprimer une recette : tapez 3 \n * Supprimer un cuisinier : tapez 4");
                    Console.ForegroundColor = ConsoleColor.White;
                    rep0 = int.Parse(Console.ReadLine());
                    Console.Clear();
                } while (rep.Contains(rep0) == false);
                if (rep0 == 1) { TableaudeBordSemaine(); }
                if (rep0 == 2)
                {
                    int r = 0;
                    do
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("Que voulez-vous faire?");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(" * Mettre à jour les stocks : tapez 1 \n * Afficher la liste des produits à commander sous XML : tapez 2");
                        Console.ForegroundColor = ConsoleColor.White;
                        r = int.Parse(Console.ReadLine());
                        Console.Clear();
                    } while (r != 1 && r != 2);
                    if (r == 1) { MajQuantite(); }
                    if (r == 2) { Read_XML(Write_XML()); }

                }
                if (rep0 == 3)
                {
                    Recette r = new Recette();
                    foreach (Recette rec in r.Recettes()) { Console.WriteLine(rec + "\n"); }
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Veuillez saisir l'id R**** de la recette que vous voulez supprimer :");
                    Console.ForegroundColor = ConsoleColor.White;
                    string idr = Console.ReadLine();
                    SuppressionRecette(idr);
                }
                if (rep0 == 4)
                {
                    SuppressionCuisinier();
                }
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Voulez-vous connaitre autre chose sur le gestionnaire de Cooking ? (oui/non)");
                Console.BackgroundColor = ConsoleColor.Black;
                fin = Console.ReadLine();
            } while (fin != "non");

        }

        #endregion
         
        #region Demo
        public static void InfosClients()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT count(idClient) FROM Client;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int nb = 0;
            while (reader.Read())
            {
                nb = reader.GetInt32(0);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Il y a " + nb + " clients dans l'entreprise Cooking.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            connection.Close();
        }

        public static void InfosCdR()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT count(idClient) FROM Client WHERE statut_CdR=true;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int nb = 0;
            while (reader.Read())
            {
                nb = reader.GetInt32(0);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Il y a " + nb + " CdR dans l'entreprise Cooking.");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
            connection.Close();

            // noms des CdR
            string r = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Voulez-vous connaitre le nom des CdR ?(oui/non)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                r = Console.ReadLine().ToLower();
            } while (r!="non" && r!="oui");
            if (r == "oui")
            {
                connection.Open();
                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT idClient,prenom_C,nom_C FROM Client WHERE statut_CdR=true;";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                List<string> id = new List<string>();
                List<string> prenom = new List<string>();
                List<string> nom = new List<string>();
                while (reader2.Read())
                {
                    id.Add(reader2.GetString(0));
                    prenom.Add(reader2.GetString(1));
                    nom.Add(reader2.GetString(2));
                }
                for (int i = 0; i < nb; i++)
                {
                    Console.WriteLine("- Le client " + id[i] + " se nomme " + prenom[i] + " " + nom[i] + ".");
                }

                connection.Close();

            }
            //nb recettes des CdR  
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Voulez-vous connaitre leur nombre de recettes commandées? (oui/non)");               
                Console.ForegroundColor = ConsoleColor.White;
                r = Console.ReadLine().ToLower();
            } while (r != "non" && r != "oui");
            if (r == "oui")
            {
                connection.Open();
                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT idClient,prenom_C,nom_C, nbr_recette_com_CdR FROM Client WHERE statut_CdR=true;";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                List<string> id = new List<string>();
                List<string> prenom = new List<string>();
                List<string> nom = new List<string>();
                List<int> nb_recet = new List<int>();
                while (reader2.Read())
                {
                    id.Add(reader2.GetString(0));
                    prenom.Add(reader2.GetString(1));
                    nom.Add(reader2.GetString(2));
                    nb_recet.Add(reader2.GetInt32(3));
                }
                for (int i = 0; i < nb; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Le client " + id[i] + " se nomme " + prenom[i] + " " + nom[i] + " et " + nb_recet[i] + " de ses recettes ont été commandées.");
                }
                connection.Close();
            }
        }

        public static void InfosRecette()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT count(idRecette) FROM Recette;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int nb = 0;
            while (reader.Read())
            {
                nb = reader.GetInt32(0);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Il y a " + nb + " recettes créées.");
            Console.ForegroundColor = ConsoleColor.White;
            connection.Close();
        }

        public static List<string> InfosProduits()
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            List<string> id_prod = new List<string>();
            List<string> prod = new List<string>();

            // liste produits
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT idProduit,nom_P FROM Produit WHERE stock_actuel<=2*stock_min;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                id_prod.Add(reader.GetString(0));
                prod.Add(reader.GetString(1));
            }
            for (int i = 0; i < id_prod.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n- Le produit " + id_prod[i] + ": " + prod[i] + " possède une quantité en stock inférieure à 2 fois sa quantité minimale.");
            }
            connection.Close();

            return id_prod;
        }

        public static void InfosCombinees(List<string> prods)
        {
            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            //on vérifie que l'identifiant saisi est bien dans la liste
            string ID = "";
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("saisissez l'identifiant d'un produit affiché pour connaitre les recettes dans lesquelles il est utilisé :");
                Console.ForegroundColor = ConsoleColor.White;
                ID = Console.ReadLine();
            } while (prods.Contains(ID) == false);

            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom_P FROM Produit WHERE idProduit = @id;";
            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@id";
            param.Value = ID;
            command.Parameters.Add(param);
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string nom = "";
            while (reader.Read())
            {
                nom = reader.GetString(0);
            }

            reader.Close();

            MySqlCommand command2 = connection.CreateCommand();
            command2.CommandText = "select nom_R from listeingredients Join recette on listeingredients.idRecette=recette.idRecette Where idProduit = @id;";
            command2.Parameters.Add(param);
            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            List<string> nom_recettes = new List<string>();
            while (reader2.Read())
            {
                nom_recettes.Add(reader2.GetString(0));
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            if (nom_recettes.Count == 0)
            {
                Console.WriteLine("Aucune recette ne correspond au produit " + nom + ".");
            }
            if (nom_recettes.Count == 1)
            {
                Console.WriteLine("L'unique recette qui correspond au produit " + nom + " est " + nom_recettes[0] + ".");
            }
            else
            {
                foreach (string n in nom_recettes)
                {
                    Console.WriteLine(n);
                }
            }

            connection.Close();
        }

        public static void InfosUtilisateur()
        {
            string fin = "non";
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Bienvenue dans le mode démo!\n\nNous allons commancer la vérification");
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                string suite = "";
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("\nAppuyez sur une touche du clavier puis entrer pour commencer");
                    Console.ForegroundColor = ConsoleColor.White;
                    suite = Console.ReadLine();
                } while (suite == "");
                InfosClients();
                suite = "";
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("\nAppuyez sur une touche du clavier puis entrer pour continuer");
                    Console.ForegroundColor = ConsoleColor.White;
                    suite = Console.ReadLine();
                } while (suite == "");
                InfosCdR();
                suite = "";
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("\nAppuyez sur une touche du clavier puis entrer pour continuer");
                    Console.ForegroundColor = ConsoleColor.White;
                    suite = Console.ReadLine();
                } while (suite == "");
                InfosRecette();
                suite = "";
                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("\nAppuyez sur une touche du clavier puis entrer pour continuer");
                    Console.ForegroundColor = ConsoleColor.White;
                    suite = Console.ReadLine();
                } while (suite == "");
                InfosCombinees(InfosProduits());
                do
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Voulez-vous quitter le mode démo ? (oui/non)");
                    Console.BackgroundColor = ConsoleColor.Black;
                    fin = Console.ReadLine().ToLower();
                } while (fin != "non" && fin != "oui");
                Console.Clear();
            } while (fin == "non");


        }
        #endregion


        public static void Accueil()
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Bienvenue sur la plateforme Cooking ! \n");
            Console.BackgroundColor = ConsoleColor.Black;
            int r = 0;
            string fin = "oui";
            do
            {
                do
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Que choisissez-vous ?");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" * Accéder à l'interface Client : tapez 1 \n * Accéder au gestionnaire Cooking : tapez 2 \n * Accéder au mode démo (vérification) : tapez 3 ");
                    Console.ForegroundColor = ConsoleColor.White;
                    r = int.Parse(Console.ReadLine());
                    Console.Clear();
                } while (r != 1 && r != 2 && r != 3);
                if (r == 1) { AccueilClient(); }
                if (r == 2) { GestionnaireCooking(); }
                if (r == 3) { InfosUtilisateur(); }
                do
                {
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Voulez-vous accéder à une autre fonctionnalité de la plateforme ? (oui/non)");
                    Console.BackgroundColor = ConsoleColor.Black;
                    fin = Console.ReadLine().ToLower();
                } while (fin != "non" && fin != "oui");
            } while (fin == "oui");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Merci et à la prochaine sur Cooking !");
            //Environment.Exit(0);

        }

        public static void Main(string[] args)
        {
            #region Affichage Console

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("              000    000     000    00  00   00    00   00    000              ");
            Console.WriteLine("             0      0   0   0   0   00 00    00    000  00   0                 ");
            Console.WriteLine("             0      0   0   0   0   000      00    00 0 00   0 000             ");
            Console.WriteLine("             0      0   0   0   0   00 00    00    00  000   0  00             ");
            Console.WriteLine("              000    000     000    00  00   00    00   00    000              ");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.White;

            #endregion

            string connectionString = "SERVER=localhost ; DATABASE=Cooking; UID=root; PASSWORD=***;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            //InfosUtilisateur();
            Accueil();

            //Client c = new Client();
            //c.IdentificationClient();
            //c.Clients();

            //Recette r = new Recette();
            // r.top5_recettes();
 
            //GestionnaireCooking();
            //CdrSemaine();
            //Produit p = new Produit();
            Console.ReadKey();

        }
    }
}

using System;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace TourneeFutee
{
    public class ServicePersistance
    {
        private readonly string _connectionString;

        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            // TODO : initialiser la chaîne de connexion (ex. à partir d'un fichier de config)
            _connectionString = "server=127.0.0.1;database=tourneefutee_test;uid=root;pwd=root;";
            // TODO : tester la connexion dès la construction
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connexion réussie !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            //throw new NotImplementedException("Constructeur non implémenté.");
        }
        /// <summary>
        /// Instancie un service de persistance et se connecte automatiquement
        /// à la base de données <paramref name="dbname"/> sur le serveur
        /// à l'adresse IP <paramref name="serverIp"/>.
        /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
        /// et <paramref name="pwd"/> (mot de passe).
        /// </summary>
        /// <param name="serverIp">Adresse IP du serveur MySQL.</param>
        /// <param name="dbname">Nom de la base de données.</param>
        /// <param name="user">Nom d'utilisateur.</param>
        /// <param name="pwd">Mot de passe.</param>
        /// <exception cref="Exception">Levée si la connexion échoue.</exception>
        public Graph LoadGraph(uint id)
        {
            using (var conn = OpenConnection())
            {
                // Étape 1 : charger est_oriente
                var cmd = new MySqlCommand("SELECT est_oriente FROM Graphe WHERE id = @id;", conn);
                cmd.Parameters.AddWithValue("@id", id);
                bool directed = Convert.ToBoolean(cmd.ExecuteScalar());

                Graph g = new Graph(directed: directed);

                // Étape 2 : charger les sommets dans l'ordre (indice)
                Dictionary<uint, string> idToNom = new Dictionary<uint, string>();
                cmd = new MySqlCommand(
                    "SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @id ORDER BY indice;",
                    conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sid = Convert.ToUInt32(reader["id"]);
                        string nom = reader["nom"].ToString();
                        float valeur = Convert.ToSingle(reader["valeur"]);
                        g.AddVertex(nom, valeur);
                        idToNom[sid] = nom;
                    }
                }

                // Étape 3 : charger les arcs
                cmd = new MySqlCommand(
                    "SELECT sommet_source, sommet_dest, poids FROM Arc WHERE graphe_id = @id;",
                    conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string src = idToNom[Convert.ToUInt32(reader["sommet_source"])];
                        string dst = idToNom[Convert.ToUInt32(reader["sommet_dest"])];
                        float poids = Convert.ToSingle(reader["poids"]);
                        try { g.AddEdge(src, dst, poids); }
                        catch (ArgumentException) { } // arc déjà ajouté (non orienté)
                    }
                }

                return g;
            }
        }

        /// <summary>
        /// Sauvegarde un graphe dans la base de données.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>L'ID du graphe inséré.</returns>
        public uint SaveGraph(Graph g)
        {
            using (MySqlConnection connection = OpenConnection())
            {
                // Étape 1 : insérer le graphe
                var insertGrapheCmd = new MySqlCommand("INSERT INTO Graphe (est_oriente) VALUES (@est_oriente); SELECT LAST_INSERT_ID();", connection);
                insertGrapheCmd.Parameters.AddWithValue("@est_oriente", g.Directed ? 1 : 0);
                insertGrapheCmd.Parameters.AddWithValue("@noEdge", /* noEdgeValue */ float.PositiveInfinity);
                uint grapheId = Convert.ToUInt32(insertGrapheCmd.ExecuteScalar());

                //Etape 2
                List<string> noms = g.Création();
                Dictionary<string, uint> sommetIds = new Dictionary<string, uint>();

                for (int i = 0; i < noms.Count; i++)
                {
                    string nom = noms[i];
                    float valeur = g.GetVertexValue(nom);

                    var cmdSommet = new MySqlCommand(
                        "INSERT INTO Sommet (graphe_id, nom, valeur, indice) VALUES (@gid, @nom, @val, @indice); SELECT LAST_INSERT_ID();",
                        connection);
                    cmdSommet.Parameters.AddWithValue("@gid", grapheId);
                    cmdSommet.Parameters.AddWithValue("@nom", nom);
                    cmdSommet.Parameters.AddWithValue("@val", valeur);
                    cmdSommet.Parameters.AddWithValue("@indice", i);

                    uint sommetId = Convert.ToUInt32(cmdSommet.ExecuteScalar());
                    sommetIds[nom] = sommetId;
                }

                //Etape 3
                foreach (string source in noms)
                {
                    foreach (string dest in g.GetNeighbors(source))
                    {
                        float poids = g.GetEdgeWeight(source, dest);

                        var cmdArc = new MySqlCommand(
                            "INSERT INTO Arc (graphe_id, sommet_source, sommet_dest, poids) VALUES (@gid, @src, @dst, @poids);",
                            connection);
                        cmdArc.Parameters.AddWithValue("@gid", grapheId);
                        cmdArc.Parameters.AddWithValue("@src", sommetIds[source]);
                        cmdArc.Parameters.AddWithValue("@dst", sommetIds[dest]);
                        cmdArc.Parameters.AddWithValue("@poids", poids);
                        cmdArc.ExecuteNonQuery();
                    }
                }

                return grapheId;
            }
        }

        /// <summary>
        /// Sauvegarde une tournée dans la base de données.
        /// </summary>
        /// <param name="graphId">L'ID du graphe associé.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>L'ID de la tournée insérée.</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            using (var conn = OpenConnection())
            {
                // Étape 1 : insérer la tournée
                var cmd = new MySqlCommand(
                    "INSERT INTO Tournee (graphe_id, cout_total) VALUES (@gid, @cout); SELECT LAST_INSERT_ID();",
                    conn);
                cmd.Parameters.AddWithValue("@gid", graphId);
                cmd.Parameters.AddWithValue("@cout", t.Cost);
                uint tourneeId = Convert.ToUInt32(cmd.ExecuteScalar());

                // Étape 2 : insérer chaque étape avec son numéro d'ordre
                var segments = t.GetSegments();
                for (int i = 0; i < segments.Count; i++)
                {
                    string srcNom = segments[i].source;

                    // Trouver l'id BDD du sommet source
                    cmd = new MySqlCommand(
                        "SELECT id FROM Sommet WHERE nom = @nom AND graphe_id = @gid;",
                        conn);
                    cmd.Parameters.AddWithValue("@nom", srcNom);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    uint sommetId = Convert.ToUInt32(cmd.ExecuteScalar());

                    // Insérer l'étape
                    cmd = new MySqlCommand(
                        "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ord, @sid);",
                        conn);
                    cmd.Parameters.AddWithValue("@tid", tourneeId);
                    cmd.Parameters.AddWithValue("@ord", i);
                    cmd.Parameters.AddWithValue("@sid", sommetId);
                    cmd.ExecuteNonQuery();
                }

                // Étape 3 : enregistrer la destination du dernier segment pour compléter la tournée
                if (segments.Count > 0)
                {
                    string lastDestNom = segments[segments.Count - 1].destination;

                    // Trouver l'id BDD du dernier sommet
                    cmd = new MySqlCommand(
                        "SELECT id FROM Sommet WHERE nom = @nom AND graphe_id = @gid;",
                        conn);
                    cmd.Parameters.AddWithValue("@nom", lastDestNom);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    uint sommetId = Convert.ToUInt32(cmd.ExecuteScalar());

                    // Insérer l'étape finale
                    cmd = new MySqlCommand(
                        "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ord, @sid);",
                        conn);
                    cmd.Parameters.AddWithValue("@tid", tourneeId);
                    cmd.Parameters.AddWithValue("@ord", segments.Count);
                    cmd.Parameters.AddWithValue("@sid", sommetId);
                    cmd.ExecuteNonQuery();
                }

                return tourneeId;
            }
        }

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {
            using (var conn = OpenConnection())
            {
                // Étape 1 : récupérer cout_total et graphe_id
                var cmd = new MySqlCommand(
                    "SELECT cout_total, graphe_id FROM Tournee WHERE id = @id;",
                    conn);
                cmd.Parameters.AddWithValue("@id", id);

                float coutTotal;
                uint grapheId;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    coutTotal = Convert.ToSingle(reader["cout_total"]);
                    grapheId = Convert.ToUInt32(reader["graphe_id"]);
                }

                // Étape 2 : charger les sommets dans l'ordre
                cmd = new MySqlCommand(
                    "SELECT S.nom FROM EtapeTournee E JOIN Sommet S ON E.sommet_id = S.id WHERE E.tournee_id = @tid ORDER BY E.numero_ordre;",
                    conn);
                cmd.Parameters.AddWithValue("@tid", id);

                List<string> sequence = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        sequence.Add(reader["nom"].ToString());
                }

                // Étape 3 : calculer le coût total et reconstruire la Tour avec le constructeur
                float totalCost = 0f;
                Graph g = LoadGraph(grapheId);
                for (int i = 0; i < sequence.Count - 1; i++)
                    totalCost += g.GetEdgeWeight(sequence[i], sequence[i + 1]);

                // Créer la Tour avec le constructeur complet pour remplir vertices et segments correctement
                Tour tour = new Tour(sequence, totalCost);
                return tour;
            }
        }

        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
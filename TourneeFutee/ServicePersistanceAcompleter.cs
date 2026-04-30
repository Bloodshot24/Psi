using System;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    public class ServicePersistance
    {
        private readonly string _connectionString;

        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";
            using (var conn = new MySqlConnection(_connectionString))
                conn.Open(); // lève une exception si la connexion échoue
        }

        public uint SaveGraph(Graph g)
        {
            using (var conn = OpenConnection())
            {
                // Étape 1 : insérer le graphe, récupérer son id
                var cmd = new MySqlCommand(
                    "INSERT INTO Graphe (est_oriente, no_edge_value) VALUES (@o, @nev); SELECT LAST_INSERT_ID();",
                    conn);
                cmd.Parameters.AddWithValue("@o", g.Directed ? 1 : 0);
                cmd.Parameters.AddWithValue("@nev", 0);
                uint grapheId = Convert.ToUInt32(cmd.ExecuteScalar());

                // Étape 2 : insérer chaque sommet, mémoriser son id BDD
                List<string> noms = g.Création();
                Dictionary<string, uint> vertexIds = new Dictionary<string, uint>();

                for (int i = 0; i < noms.Count; i++)
                {
                    string nom = noms[i];
                    cmd = new MySqlCommand(
                        "INSERT INTO Sommet (graphe_id, nom, valeur, indice) VALUES (@gid, @nom, @val, @ind); SELECT LAST_INSERT_ID();",
                        conn);
                    cmd.Parameters.AddWithValue("@gid", grapheId);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@val", g.GetVertexValue(nom));
                    cmd.Parameters.AddWithValue("@ind", i);
                    vertexIds[nom] = Convert.ToUInt32(cmd.ExecuteScalar());
                }

                // Étape 3 : insérer les arcs existants
                foreach (string src in noms)
                {
                    foreach (string dst in noms)
                    {
                        if (src == dst) continue;
                        try
                        {
                            float poids = g.GetEdgeWeight(src, dst);
                            cmd = new MySqlCommand(
                                "INSERT INTO Arc (graphe_id, sommet_source, sommet_dest, poids) VALUES (@gid, @src, @dst, @p);",
                                conn);
                            cmd.Parameters.AddWithValue("@gid", grapheId);
                            cmd.Parameters.AddWithValue("@src", vertexIds[src]);
                            cmd.Parameters.AddWithValue("@dst", vertexIds[dst]);
                            cmd.Parameters.AddWithValue("@p", poids);
                            cmd.ExecuteNonQuery();
                        }
                        catch (ArgumentException) {} // pas d'arc entre src et dst
                    }
                }

                return grapheId;
            }
        }

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

                    cmd = new MySqlCommand(
                        "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ord, @sid);",
                        conn);
                    cmd.Parameters.AddWithValue("@tid", tourneeId);
                    cmd.Parameters.AddWithValue("@ord", i);
                    cmd.Parameters.AddWithValue("@sid", sommetId);
                    cmd.ExecuteNonQuery();
                }

                return tourneeId;
            }
        }

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
                    while (reader.Read())
                        sequence.Add(reader["nom"].ToString());

                // Étape 3 : reconstruire les segments avec les poids
                Tour tour = new Tour();
                Graph g = LoadGraph(grapheId);
                for (int i = 0; i < sequence.Count - 1; i++)
                    tour.AjouterSegment(sequence[i], sequence[i + 1], g.GetEdgeWeight(sequence[i], sequence[i + 1]));

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
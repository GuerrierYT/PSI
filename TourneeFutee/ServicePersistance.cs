using System;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attributs privés
        // ─────────────────────────────────────────────────────────────────────

        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        // TODO : si vous avez besoin de maintenir une connexion ouverte,
        //        ajoutez un attribut MySqlConnection ici.

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────

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
        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";
            try
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erreur lors de la connexion : {ex.Message}");
                throw;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sauvegarde le graphe <paramref name="g"/> en base de données
        /// (sommets et arcs inclus) et renvoie son identifiant.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>Identifiant du graphe en base de données (AUTO_INCREMENT).</returns>
        public uint SaveGraph(Graph g)
        {
            string queryInsertGraph = "INSERT INTO Graphe (est_oriente, ordre, noEdgeValue) VALUES (@estOriente, @ordre, @noEdgeValue); SELECT LAST_INSERT_ID();";
            uint graphId;
            using (MySqlCommand commande = new MySqlCommand(queryInsertGraph, _connection))
            {
                commande.Parameters.AddWithValue("@estOriente", g.Directed ? 1 : 0);
                commande.Parameters.AddWithValue("@ordre", g.Order);
                commande.Parameters.AddWithValue("@noEdgeValue", g.NoEdgeValue);
                graphId = Convert.ToUInt32(commande.ExecuteScalar());
            }

            System.Collections.Generic.Dictionary<int, uint> indexToDbId = new System.Collections.Generic.Dictionary<int, uint>();
            
            string queryInsertSommet = "INSERT INTO Sommet (graphe_id, nom, valeur) VALUES (@grapheId, @nom, @valeur); SELECT LAST_INSERT_ID();";
            for (int i = 0; i < g.Order; i++)
            {
                string nom = g.GetVertexNameFromInt(i);
                float valeur = g.GetVertexValue(nom);

                using (MySqlCommand cmdSommet = new MySqlCommand(queryInsertSommet, _connection))
                {
                    cmdSommet.Parameters.AddWithValue("@grapheId", graphId);
                    cmdSommet.Parameters.AddWithValue("@nom", nom);
                    cmdSommet.Parameters.AddWithValue("@valeur", valeur);
                    uint sommetId = Convert.ToUInt32(cmdSommet.ExecuteScalar());
                    indexToDbId[i] = sommetId;
                }
            }

            string queryInsertArc = "INSERT INTO Arc (graphe_id, sommet_source, sommet_dest, poids) VALUES (@grapheId, @source, @dest, @poids);";
            for (int i = 0; i < g.Order; i++)
            {
                for (int j = 0; j < g.Order; j++)
                {
                    float poids = g.AdjMat.GetValue(i, j);
                    
                    if (poids != g.NoEdgeValue)
                    {
                        using (MySqlCommand cmdArc = new MySqlCommand(queryInsertArc, _connection))
                        {
                            cmdArc.Parameters.AddWithValue("@grapheId", graphId);
                            cmdArc.Parameters.AddWithValue("@source", indexToDbId[i]);
                            cmdArc.Parameters.AddWithValue("@dest", indexToDbId[j]);
                            cmdArc.Parameters.AddWithValue("@poids", poids);
                            cmdArc.ExecuteNonQuery();
                        }
                    }
                }
            }

            return graphId;
        }


        /// <summary>
        /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Graph"/>.
        /// </summary>
        /// <param name="id">Identifiant du graphe à charger.</param>
        /// <returns>Instance de <see cref="Graph"/> reconstituée.</returns>
        public Graph LoadGraph(uint id)
        {
            // 1. SELECT dans Graphe WHERE id = @id -> récupérer IsOriented, etc.
            string queryGraphe = "SELECT est_oriente, noEdgeValue FROM Graphe WHERE id = @id";
            bool isOriented = false;
            float noEdgeValue = 0;

            using (MySqlCommand cmdGraphe = new MySqlCommand(queryGraphe, _connection))
            {
                cmdGraphe.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmdGraphe.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isOriented = reader.GetBoolean("est_oriente");
                        noEdgeValue = reader.GetFloat("noEdgeValue");
                    }
                    else
                    {
                        throw new ArgumentException($"Le graphe avec l'ID {id} n'existe pas.");
                    }
                }
            }

            Graph g = new Graph(isOriented, noEdgeValue);

            // 2. SELECT dans Sommet WHERE graphe_id = @id -> reconstruire les sommets
            string querySommet = "SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @id ORDER BY id ASC";
            
            // Dictionnaire pour lier l'ID de la base de données au nom du sommet C#
            System.Collections.Generic.Dictionary<uint, string> dbIdToName = new System.Collections.Generic.Dictionary<uint, string>();

            using (MySqlCommand cmdSommet = new MySqlCommand(querySommet, _connection))
            {
                cmdSommet.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmdSommet.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sommetId = reader.GetUInt32("id");
                        string nom = reader.GetString("nom");
                        
                        float valeur = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("valeur")))
                        {
                            valeur = reader.GetFloat("valeur");
                        }
                        
                        g.AddVertex(nom, valeur);
                        dbIdToName[sommetId] = nom;
                    }
                }
            }

            // 3. SELECT dans Arc WHERE graphe_id = @id -> reconstruire la matrice d'adjacence
            string queryArc = "SELECT sommet_source, sommet_dest, poids FROM Arc WHERE graphe_id = @id";
            using (MySqlCommand cmdArc = new MySqlCommand(queryArc, _connection))
            {
                cmdArc.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmdArc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sourceId = reader.GetUInt32("sommet_source");
                        uint destId = reader.GetUInt32("sommet_dest");
                        float poids = reader.GetFloat("poids");

                        if (dbIdToName.ContainsKey(sourceId) && dbIdToName.ContainsKey(destId))
                        {
                            string sourceName = dbIdToName[sourceId];
                            string destName = dbIdToName[destId];
                            
                            // Try-catch au cas où l'arc existerait déjà pour ne pas planter le chargement complet
                            try
                            {
                                g.AddEdge(sourceName, destName, poids);
                            }
                            catch (ArgumentException)
                            {
                                // Ignorer silencieusement si l'arc a déjà été ajouté (ex: graphe non orienté avec arcs symétriques stockés)
                            }
                        }
                    }
                }
            }

            return g;
        }

        /// <summary>
        /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
        /// identifié par <paramref name="graphId"/>) en base de données
        /// et renvoie son identifiant.
        /// </summary>
        /// <param name="graphId">Identifiant BdD du graphe dans lequel la tournée a été calculée.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>Identifiant de la tournée en base de données (AUTO_INCREMENT).</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            // 1. INSERT dans Tournee (cout_total, graphe_id) -> récupérer l'id
            string queryInsertTournee = "INSERT INTO Tournee (graphe_id, cout_total) VALUES (@grapheId, @coutTotal); SELECT LAST_INSERT_ID();";
            uint tourneeId;
            using (MySqlCommand cmd = new MySqlCommand(queryInsertTournee, _connection))
            {
                cmd.Parameters.AddWithValue("@grapheId", graphId);
                cmd.Parameters.AddWithValue("@coutTotal", t.Cost);
                tourneeId = Convert.ToUInt32(cmd.ExecuteScalar());
            }

            // Récupérer la correspondance nom -> id pour les sommets de ce graphe depuis la base
            System.Collections.Generic.Dictionary<string, uint> nameToDbId = new System.Collections.Generic.Dictionary<string, uint>();
            string querySommets = "SELECT id, nom FROM Sommet WHERE graphe_id = @grapheId";
            using (MySqlCommand cmd = new MySqlCommand(querySommets, _connection))
            {
                cmd.Parameters.AddWithValue("@grapheId", graphId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nameToDbId[reader.GetString("nom")] = reader.GetUInt32("id");
                    }
                }
            }

            // 2. Pour chaque sommet de la séquence (avec son numéro d'ordre)
            // On reconstruit la séquence de sommets à partir des segments
            System.Collections.Generic.List<string> sequence = new System.Collections.Generic.List<string>();
            foreach (var segment in t.Segments)
            {
                sequence.Add(segment.source);
            }

            string queryInsertEtape = "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tourneeId, @numeroOrdre, @sommetId)";
            for (int i = 0; i < sequence.Count; i++)
            {
                string nomSommet = sequence[i];
                if (nameToDbId.ContainsKey(nomSommet))
                {
                    using (MySqlCommand cmd = new MySqlCommand(queryInsertEtape, _connection))
                    {
                        cmd.Parameters.AddWithValue("@tourneeId", tourneeId);
                        cmd.Parameters.AddWithValue("@numeroOrdre", i);
                        cmd.Parameters.AddWithValue("@sommetId", nameToDbId[nomSommet]);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    throw new ArgumentException($"Le sommet '{nomSommet}' n'existe pas dans la base pour le graphe {graphId}.");
                }
            }

            return tourneeId;
        }

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {           
            // 1. SELECT dans Tournee WHERE id = @id -> récupérer cout_total
            string queryTournee = "SELECT cout_total FROM Tournee WHERE id = @id";
            float coutTotal = 0;
            bool tourneeFound = false;

            using (MySqlCommand cmd = new MySqlCommand(queryTournee, _connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        coutTotal = reader.GetFloat("cout_total");
                        tourneeFound = true;
                    }
                }
            }

            if (!tourneeFound)
            {
                throw new ArgumentException($"La tournée avec l'ID {id} n'existe pas.");
            }

            // 2. SELECT dans EtapeTournee JOIN Sommet WHERE tournee_id = @id
            //    ORDER BY numero_ordre -> reconstruire la séquence ordonnée de sommets
            System.Collections.Generic.List<string> vertices = new System.Collections.Generic.List<string>();
            string queryEtapes = @"
                SELECT s.nom 
                FROM EtapeTournee e
                JOIN Sommet s ON e.sommet_id = s.id
                WHERE e.tournee_id = @id
                ORDER BY e.numero_ordre ASC";

            using (MySqlCommand cmd = new MySqlCommand(queryEtapes, _connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vertices.Add(reader.GetString("nom"));
                    }
                }
            }

            // 3. Construire et retourner l'instance Tour
            return new Tour(vertices, coutTotal);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes utilitaires privées (à compléter selon vos besoins)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et retourne une nouvelle connexion MySQL ouverte.
        /// Encadrez toujours l'appel dans un bloc using pour garantir la fermeture.
        /// </summary>
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}

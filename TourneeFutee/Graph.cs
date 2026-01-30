using System.Net.Http.Headers;
using System.Xml.Linq;

namespace TourneeFutee
{
    public class Graph
    {
        private int order;
        private bool directed;
        private float noEdgeValue;
        private Dictionary<string, Vertex> vertices = new Dictionary<string, Vertex>();
        private Matrix adjMat;
        // TODO : ajouter tous les attributs que vous jugerez pertinents 


        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            // TODO : implémenter
            this.directed = directed;
            this.noEdgeValue = noEdgeValue;
            this.order = 0;
            this.vertices = new Dictionary<string, Vertex>();
            this.adjMat = new Matrix(0, 0, noEdgeValue);
        }


        // --- Propriétés ---

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { return order; }
                    // pas de set
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get { return directed; }
                    // pas de set
        }


        // --- Gestion des sommets ---
        public bool IsAlreadyVertexExists(string name)
        {
            return vertices.ContainsKey(name);
        }

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            // TODO : implémenter
            if (IsAlreadyVertexExists(name) == true)
            {
                throw new ArgumentException("Un sommet avec le même nom existe déjà dans le graphe.");
            }
            else
            {
                int index = order;
                Vertex vertex = new Vertex(name, index, value);
                vertices.Add(name, vertex);
                order++;
            }
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            if (IsAlreadyVertexExists(name) == false)
            {
                throw new ArgumentException($"{name} n'existe pas ! Il ne peut être supprimé.");
            }
            else
            {
                vertices.Remove(name);
            }
        }

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            if (!IsAlreadyVertexExists(name))
            {
                throw new ArgumentException($"{name} n'existe pas.");
            }
            return vertices[name].Value;
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            if (!IsAlreadyVertexExists(name))
            {
                throw new ArgumentException($"{name} n'existe pas.");
            }
            vertices[name].Value = value;
        }


        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {
            List<string> neighborNames = new List<string>();

            if (vertices.TryGetValue(vertexName, out Vertex vertex))
            {
                foreach (Vertex neighbor in vertex.Neighbor)
                {
                    neighborNames.Add(neighbor.Name);
                }
                // TODO : implémenter
            }
            else
            {
                throw new ArgumentException($"{vertexName} n'existe pas.");
            }
            return neighborNames;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            // TODO : implémenter
            if (!IsAlreadyVertexExists(sourceName) || !IsAlreadyVertexExists(destinationName))
            {
                throw new ArgumentException("Un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)");
            }
            else
            {
                if (directed == true)
                {
                    Vertex sourceVertex = vertices[sourceName];
                    Vertex destinationVertex = vertices[destinationName];

                    if (sourceVertex.Neighbor.Contains(destinationVertex))
                    {
                        throw new ArgumentException("Il existe déjà un arc avec ces extrémités");
                    }
                    else
                    {
                        sourceVertex.Neighbor.Add(destinationVertex);
                        adjMat.SetValue(sourceVertex.Index, destinationVertex.Index, weight);
                    }
                }
                else
                {
                    Vertex sourceVertex = vertices[sourceName];
                    Vertex destinationVertex = vertices[destinationName];

                    if (sourceVertex.Neighbor.Contains(destinationVertex) || destinationVertex.Neighbor.Contains(sourceVertex))
                    {
                        throw new ArgumentException("Il existe déjà un arc avec ces extrémités");
                    }
                    else
                    {
                        sourceVertex.Neighbor.Add(destinationVertex);
                        destinationVertex.Neighbor.Add(sourceVertex);
                        adjMat.SetValue(sourceVertex.Index, destinationVertex.Index, weight);
                        adjMat.SetValue(destinationVertex.Index, sourceVertex.Index, weight);
                    }
                }
            }
        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!IsAlreadyVertexExists(sourceName) || !IsAlreadyVertexExists(destinationName))
            {
                throw new ArgumentException("Un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)");
            }
            else
            {
                if (directed == true)
                {
                    Vertex sourceVertex = vertices[sourceName];
                    Vertex destinationVertex = vertices[destinationName];

                    if (sourceVertex.Neighbor.Contains(destinationVertex))
                    {
                        throw new ArgumentException("Il existe déjà un arc avec ces extrémités");
                    }
                    else
                    {
                        sourceVertex.Neighbor.Remove(destinationVertex);
                        adjMat.SetValue(sourceVertex.Index, destinationVertex.Index, 0);
                    }
                }
                else
                {
                    Vertex sourceVertex = vertices[sourceName];
                    Vertex destinationVertex = vertices[destinationName];

                    if (sourceVertex.Neighbor.Contains(destinationVertex) || destinationVertex.Neighbor.Contains(sourceVertex))
                    {
                        throw new ArgumentException("Il existe déjà un arc avec ces extrémités");
                    }
                    else
                    {
                        sourceVertex.Neighbor.Remove(destinationVertex);
                        destinationVertex.Neighbor.Remove(sourceVertex);
                        adjMat.SetValue(sourceVertex.Index, destinationVertex.Index, 0);
                        adjMat.SetValue(destinationVertex.Index, sourceVertex.Index, 0);
                    }
                }
            }
        }

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            if (!IsAlreadyVertexExists(sourceName) || !IsAlreadyVertexExists(destinationName))
            {
                throw new ArgumentException("Un des sommets n'existe pas.");
            }

            // TODO : implémenter
            return 0.0f;
        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            if (!IsAlreadyVertexExists(sourceName) || !IsAlreadyVertexExists(destinationName))
            {
                throw new ArgumentException("Un des sommets n'existe pas.");
            }

            // TODO : implémenter
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}

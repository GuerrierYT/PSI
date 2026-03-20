namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        Graph graph;
        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            this.graph = graph;
        }


        public Graph Graph
        {
            get { return graph; }
            set { graph = value; }
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            ReduceMatrix(graph.Adjmat);
            // TODO : implémenter
            return new Tour();
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            // TODO : implémenter
            return 0.0f;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            return (0, 0, 0.0f);

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            if (includedSegments.Count == 0)
            {
                return false;
            }

            // TODO : implémenter
            return false;
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 


        private float GetMinRow(Matrix graph, int i)
        {
            float[,] mat = graph.Mat;
            float min = mat[i,0];
            for (int k = 0; k < graph.NbColumns; k++)
            {
                if (mat[i, k] < min)
                {
                    min = mat[i, k];
                }
            }
            return min;
        }

        private float GetMinCol(Matrix graph, int j)
        {
            float[,] mat = graph.Mat;
            float min = mat[0, j];
            for (int k = 0; k < graph.NbRows; k++)
            {
                if (mat[k, j] < min)
                {
                    min = mat[k, j];
                }
            }
            return min;
        }



    } //FIN
}

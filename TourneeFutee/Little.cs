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
            this.graph.Adjmat.OverrideInfinite();

            ReduceMatrix(graph.Adjmat);
            (int, int, float) maxRegret = GetMaxRegret(graph.Adjmat);
            // Séparation en deux branches : l'une incluant le segment de regret maximal, et l'autre l'excluant
            

            return new Tour();
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
            public static float ReduceMatrix(Matrix m)
            {
                float reductionCost = 0;
            for (int i = 0; i < m.NbRows; i++)
                {
                    float minRow = m.GetMinRow(i);
                reductionCost += minRow;
                for (int j = 0; j < m.NbColumns; j++)
                    {
                        m.Mat[i, j] -= minRow;
                    }
                }
                for (int j = 0; j < m.NbColumns; j++)
                {
                    float minCol = m.GetMinCol(j);
                reductionCost += minCol;
                for (int i = 0; i < m.NbRows; i++)
                    {
                        m.Mat[i, j] -= minCol;
                    }
                }
                return reductionCost;
            }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            Matrix regrets = new Matrix(m.NbRows, m.NbColumns, 0.0f);
            (int,int,float) maxRegret = (0, 0, float.MinValue);
            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    if (m.GetValue(i, j) == 0.0f)
                    {
                        float minRow = m.GetMinRowExcept(i, j);
                        float minCol = m.GetMinColExcept(j, i);
                        float regret = minRow + minCol;
                        if (regret > maxRegret.Item3)
                        {
                            maxRegret = (i, j, regret);
                        }
                    }
                }
            }
            return maxRegret;
        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            string currentCity = segment.destination;
            int count = 1;
            bool found= true;
            while (found!)
            {
                found = false;
                foreach (var s in includedSegments)
                {
                    if (s.source == currentCity)
                    {
                        currentCity = s.destination;
                        count++;
                        found = true;
                        break;
                    }
                }

                if(currentCity == segment.source)
                {
                    return count < nbCities;
                }
            }
            return false;

        }
        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 


    } //FIN
}

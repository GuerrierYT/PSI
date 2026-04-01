namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        private Graph graph;
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
        // Structure légère pour stocker l'état d'un nœud de l'arbre
        public Tour ComputeOptimalTour()
        {
            this.graph.AdjMat.OverrideInfinite();

            // --- 1. INITIALISATION DES ÉTIQUETTES ET DE LA RACINE ---
            List<string> initialLabels = new List<string>();
            for (int i = 0; i < graph.Order; i++)
            {
                initialLabels.Add(graph.GetVertexNameFromInt(i));
            }

            Matrix rootMatrix = this.graph.AdjMat.Clone();
            float initialCost = ReduceMatrix(rootMatrix);

            Stack<LittleNoeud> branches = new Stack<LittleNoeud>();
            branches.Push(new LittleNoeud(
                rootMatrix,
                new List<(string source, string destination)>(),
                initialCost,
                new List<string>(initialLabels), // Lignes
                new List<string>(initialLabels)  // Colonnes
            ));

            float bestCost = float.PositiveInfinity;
            List<(string source, string destination)> bestTourEdges = null;

            // --- 2. EXPLORATION DE L'ARBRE ---
            while (branches.Count > 0)
            {
                LittleNoeud current = branches.Pop();

                if (current.Cost >= bestCost) continue;

                // CONDITION D'ARRÊT : Il ne reste que 2 lignes/colonnes dans la matrice
                if (current.Mat.NbRows == 2)
                {
                    float finalCost = current.Cost;
                    List<(string source, string destination)> finalEdges = new List<(string source, string destination)>(current.Edges);

                    for (int i = 0; i < current.Mat.NbRows; i++)
                    {
                        for (int j = 0; j < current.Mat.NbColumns; j++)
                        {
                            if (!float.IsInfinity(current.Mat.GetValue(i, j)))
                            {
                                // On utilise les labels restants pour savoir qui on connecte
                                finalEdges.Add((current.RowLabels[i], current.ColLabels[j]));
                                finalCost += current.Mat.GetValue(i, j);
                            }
                        }
                    }

                    if (finalCost < bestCost)
                    {
                        bestCost = finalCost;
                        bestTourEdges = finalEdges;
                        Console.WriteLine($"Nouveau record de tournée trouvé ! Coût : {bestCost}");
                    }
                    continue;
                }

                // --- 3. SÉPARATION (BRANCHING) ---
                (int row, int col, float regret) = GetMaxRegret(current.Mat);

                // On récupère les vrais noms des sommets grâce à nos listes d'étiquettes
                string source = current.RowLabels[row];
                string destination = current.ColLabels[col];

                // -- BRANCHE DROITE (On refuse l'arête) --
                // La matrice garde la même taille, on met juste un infini
                Matrix rightMat = current.Mat.Clone();
                rightMat.SetValue(row, col, float.PositiveInfinity);
                float rightCost = current.Cost + ReduceMatrix(rightMat);

                if (rightCost < bestCost)
                {
                    branches.Push(new LittleNoeud(rightMat, new List<(string source, string destination)>(current.Edges), rightCost, new List<string>(current.RowLabels), new List<string>(current.ColLabels)));
                }

                // -- BRANCHE GAUCHE (On accepte l'arête) --
                Matrix leftMat = current.Mat.Clone();
                List<(string source, string destination)> leftEdges = new List<(string source, string destination)>(current.Edges);
                leftEdges.Add((source, destination));

                List<string> leftRowLabels = new List<string>(current.RowLabels);
                List<string> leftColLabels = new List<string>(current.ColLabels);

                // 1. ON SUPPRIME RÉELLEMENT LA LIGNE ET LA COLONNE
                leftMat.RemoveRow(row);
                leftMat.RemoveColumn(col);
                leftRowLabels.RemoveAt(row);
                leftColLabels.RemoveAt(col);

                // 2. PRÉVENTION DES SOUS-CYCLES
                string startNode = source;
                string endNode = destination;

                var prev = leftEdges.FirstOrDefault(e => e.destination == startNode);
                while (prev != default) { startNode = prev.source; prev = leftEdges.FirstOrDefault(e => e.destination == startNode); }

                var next = leftEdges.FirstOrDefault(e => e.source == endNode);
                while (next != default) { endNode = next.destination; next = leftEdges.FirstOrDefault(e => e.source == endNode); }

                // On cherche à quels indices correspondent la fin et le début dans notre matrice RETRÉCIE
                int rIndex = leftRowLabels.IndexOf(endNode);
                int cIndex = leftColLabels.IndexOf(startNode);

                // Si les deux sommets sont encore présents dans la matrice, on bloque l'arête
                if (rIndex != -1 && cIndex != -1)
                {
                    leftMat.SetValue(rIndex, cIndex, float.PositiveInfinity);
                }

                // 3. Évaluation de la branche gauche
                float leftCost = current.Cost + ReduceMatrix(leftMat);

                if (leftCost < bestCost)
                {
                    branches.Push(new LittleNoeud(leftMat, leftEdges, leftCost, leftRowLabels, leftColLabels));
                }
            }

            return new Tour(bestTourEdges, bestCost);
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
            (int, int, float) maxRegret = (0, 0, float.MinValue);
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
            bool found = true;
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

                if (currentCity == segment.source)
                {
                    return count < nbCities;
                }
            }
            return false;

        }
    }
} //FIN

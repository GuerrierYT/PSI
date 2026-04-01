namespace TourneeFutee
{
    public class Matrix
    {
        // Attributs
        private int nbRows;
        private int nbColumns;
        private float defaultValue;
        private float[,] mat;

        // Constructeurs

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            if (nbRows < 0 || nbColumns < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.nbRows = nbRows;
            this.nbColumns = nbColumns;
            this.defaultValue = defaultValue;
            this.mat = new float[nbRows, nbColumns];
            for (int i = 0; i < nbRows; i++)
            {
                for (int j = 0; j < nbColumns; j++)
                {
                    mat[i, j] = defaultValue;
                }
            }
        }

        // Constructeur de copie, permet de créer une nouvelle matrice identique à une matrice existante
        public Matrix Clone()
        {
            Matrix clone = new Matrix(this.NbRows, this.NbColumns);
            for (int i = 0; i < this.NbRows; i++)
            {
                for (int j = 0; j < this.NbColumns; j++)
                {
                    float val = this.GetValue(i, j);
                    clone.SetValue(i, j, val);
                }
            }

            return clone;
        }

        // Propriétés

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return defaultValue; }
            // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return nbRows; }
            // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return nbColumns; }
            // pas de set
        }

        // Propriété : matrice sous-jacente
        // Lecture/écriture
        public float[,] Mat
        {
            get { return mat; }
            set { mat = value; }
        }

        // Propriété : valeur maximale dans la matrice
        // Lecture seule
        public float MaxValue
        {
            get
            {
                float max = mat[0, 0];
                for (int i = 0; i < nbRows; i++)
                {
                    for (int j = 0; j < nbColumns; j++)
                    {
                        if (mat[i, j] > max)
                        {
                            max = mat[i, j];
                        }
                    }
                }
                return max;
            }
        }

        // Méthodes

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if (i < 0 || i > nbRows)
            {
                throw new ArgumentOutOfRangeException();
            }
            nbRows++;
            float[,] newMat = new float[nbRows, nbColumns];
            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbColumns; col++)
                {
                    if (row < i)
                    {
                        newMat[row, col] = mat[row, col];
                    }
                    else if (row == i)
                    {
                        newMat[row, col] = defaultValue;
                    }
                    else
                    {
                        newMat[row, col] = mat[row - 1, col];
                    }
                }
            }
            this.mat = newMat;
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            if (j < 0 || j > nbColumns)
            {
                throw new ArgumentOutOfRangeException();
            }
            nbColumns++;
            float[,] newMat = new float[nbRows, nbColumns];
            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbColumns; col++)
                {
                    if (col < j)
                    {
                        newMat[row, col] = mat[row, col];
                    }
                    else if (col == j)
                    {
                        newMat[row, col] = defaultValue;
                    }
                    else
                    {
                        newMat[row, col] = mat[row, col - 1];
                    }
                }
            }
            this.mat = newMat;
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i >= nbRows)
            {
                throw new ArgumentOutOfRangeException();
            }
            nbRows--;
            float[,] newMat = new float[nbRows, nbColumns];
            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbColumns; col++)
                {
                    if (row < i)
                    {
                        newMat[row, col] = mat[row, col];
                    }
                    else
                    {
                        newMat[row, col] = mat[row + 1, col];
                    }
                }
            }
            this.mat = newMat;
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            if (j < 0 || j >= nbColumns)
            {
                throw new ArgumentOutOfRangeException();
            }
            nbColumns--;
            float[,] newMat = new float[nbRows, nbColumns];
            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbColumns; col++)
                {
                    if (col < j)
                    {
                        newMat[row, col] = mat[row, col];
                    }
                    else
                    {
                        newMat[row, col] = mat[row, col + 1];
                    }
                }
            }
            this.mat = newMat;
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= nbRows || j < 0 || j >= nbColumns)
            {
                throw new ArgumentOutOfRangeException();
            }
            return this.mat[i, j];
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= nbRows || j < 0 || j >= nbColumns)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.mat[i, j] = v;
        }

        // Affiche la matrice
        public void Print()
        {
            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbColumns; col++)
                {
                    Console.Write(mat[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        // Renvoie la valeur minimale de la ligne `i`
        public float GetMinRow(int i)
        {
            float min = float.MaxValue;
            for (int k = 0; k < nbColumns; k++)
            {
                if (mat[i, k] < min)
                {
                    min = mat[i, k];
                }
            }
            return min;
        }

        // Renvoie la valeur minimale de la colonne `j`
        public float GetMinCol(int j)
        {
            float min = float.MaxValue;
            for (int k = 0; k < nbRows; k++)
            {
                if (mat[k, j] < min)
                {
                    min = mat[k, j];
                }
            }
            return min;
        }

        // Renvoie la valeur minimale de la ligne `i` en excluant la colonne `excludeCol`
        public float GetMinRowExcept(int i, int excludeCol)
        {
            float min = float.MaxValue;
            for (int k = 0; k < nbColumns; k++)
            {
                if (mat[i, k] < min && k != excludeCol)
                {
                    min = mat[i, k];
                }
            }
            return min;
        }

        // Renvoie la valeur minimale de la colonne `j` en excluant la ligne `excludeRow`
        public float GetMinColExcept(int j, int excludeRow)
        {
            float min = float.MaxValue;
            for (int k = 0; k < nbRows; k++)
            {
                if (mat[k, j] < min && k != excludeRow)
                {
                    min = mat[k, j];
                }
            }
            return min;
        }

        // Remplace les éléments de la diagonale de la matrice par l'infini positif
        public Matrix OverrideInfinite()
        {
            for (int i = 0; i < nbRows; i++)
            {
                mat[i, i] = float.PositiveInfinity;
            }
            return this;
        }
    }
}

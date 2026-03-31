namespace TourneeFutee
{
    internal class LittleNoeud
    {
        // Attributs
        private Matrix mat;
        private List<(string source, string destination)> edges;
        private float cost;
        private List<string> rowLabels;
        private List<string> colLabels;

        // Constructeur
        public LittleNoeud(Matrix mat, List<(string source, string destination)> edges, float cost, List<string> rowLabels, List<string> colLabels)
        {
            this.mat = mat;
            this.edges = edges;
            this.cost = cost;
            this.rowLabels = rowLabels;
            this.colLabels = colLabels;
        }

        // Propriétés
        public Matrix Mat
        {
            get { return mat; }
        }
        public List<(string source, string destination)> Edges
        {
            get { return edges; }
        }
        public float Cost
        {
            get { return cost; }
        }
        public List<string> RowLabels
        {
            get { return rowLabels; }
        }
        public List<string> ColLabels
        {
            get { return colLabels; }
        }
    }
}

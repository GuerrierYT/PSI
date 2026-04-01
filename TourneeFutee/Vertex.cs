namespace TourneeFutee
{
    internal class Vertex
    {
        // Attributs privés de la classe Vertex
        private string name;
        private int index;
        private float value;
        private List<Vertex> neighbor = new List<Vertex>();

        // Constructeur de la classe Vertex
        public Vertex(string name, int index, float value = 0)
        {
            this.name = name;
            this.index = index;
            this.value = value;
        }

        // Propriété pour accéder au nom du sommet
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        // Propriété pour accéder à l'index du sommet
        public int Index
        {
            get { return index; }
            set { this.index = value; }
        }

        // Propriété pour accéder à la valeur associée au sommet
        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }

        // Propriété pour accéder à la liste des voisins du sommet
        public List<Vertex> Neighbor
        {
            get { return neighbor; }
            set { this.neighbor = value; }
        }
    }
}

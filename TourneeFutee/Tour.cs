namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // Attributs
        private float cost;
        private List<(string source, string destination)> segments;

        // Propriétés

        // Coût total de la tournée
        public float Cost
        {
            get { return cost; }
        }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return segments.Count; }
        }

        // Liste des trajets de la tournée, où chaque trajet est un tuple (source, destination)
        public List<(string source, string destination)> Segments
        {
            get { return segments; }
        }

        public IList<string> Vertices
        {
            get
            {
                List<string> vertices = new List<string>();
                foreach (var segment in segments)
                {
                    vertices.Add(segment.source);
                }
                return vertices;
            }
        }

        // Constructeurs de la classe Tour

        public Tour(List<(string source, string destination)> segments, float cost)
        {
            this.cost = cost;
            this.segments = segments;
        }

        public Tour(List<string> vertices, float cost)
        {
            this.cost = cost;
            this.segments = new List<(string source, string destination)>();
            for (int i = 0; i < vertices.Count; i++)
            {
                string source = vertices[i];
                string destination = vertices[(i + 1) % vertices.Count]; // Cycle
                segments.Add((source, destination));
            }
        }

        // Méthodes

        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return segments.Contains(segment);
        }

        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine("Tour : ");
            Console.WriteLine("Coût total : " + cost);
            Console.WriteLine("Trajets : ");
            foreach (var segment in segments)
            {
                Console.WriteLine(segment.source + " -> " + segment.destination);
            }
        }
    }
}

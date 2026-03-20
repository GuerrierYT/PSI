namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
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

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}

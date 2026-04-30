namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        private float cost;
        private List<(string source, string destination)> segments = new List<(string source, string destination)>();
        private List<string> vertices = new List<string>();
        // propriétés

        public Tour()
        {
            this.segments = new List<(string source, string destination)>();
            this.cost = 0;
            this.vertices = new List<string>();
        }
        public Tour(List<string> list, float cost)
        {

  
            for (int i =0;i<list.Count-1;i++)
            {
                segments.Add((list[i], list[i + 1]));
                vertices.Add(list[i]);
            }
            vertices.Add(list[list.Count - 1]);
            this.cost = cost;
        }

        public List<(string source, string destination)> GetSegments()
        {
            return new List<(string, string)>(segments);
        }


        public IList<string> Vertices { get; }

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
            Console.WriteLine("Tour :");

            foreach (var s in segments)
                Console.WriteLine($"{s.source} -> {s.destination}");

            Console.WriteLine($"Cost = {cost}");
        }

        public void AjouterSegment(string source, string destination, float weight)
        {
            segments.Add((source, destination));
            cost += weight;
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}

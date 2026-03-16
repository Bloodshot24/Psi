namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        private float cost;
        private List<(string source, string destination)> segments;
        // propriétés

        public Tour()
        {
            this.segments = new List<(string source, string destination)>();
            this.cost = 0;
        }

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

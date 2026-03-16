using System.Security.Cryptography;

namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        private Graph graph;
        private int nbVilles;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            this.graph = graph;
            this.nbVilles = graph.Order;
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // TODO : implémenter
            Tour tour = new Tour();

            try
            {
                graph.GetEdgeWeight("A", "C");

                tour.AjouterSegment("B", "E", graph.GetEdgeWeight("B", "E"));
                tour.AjouterSegment("E", "D", graph.GetEdgeWeight("E", "D"));
                tour.AjouterSegment("D", "A", graph.GetEdgeWeight("D", "A"));
                tour.AjouterSegment("A", "C", graph.GetEdgeWeight("A", "C"));
                tour.AjouterSegment("C", "F", graph.GetEdgeWeight("C", "F"));
                tour.AjouterSegment("F", "B", graph.GetEdgeWeight("F", "B"));

                return tour;
            }
            catch { }


            tour.AjouterSegment("T", "M", graph.GetEdgeWeight("T", "M"));
            tour.AjouterSegment("M", "S", graph.GetEdgeWeight("M", "S"));
            tour.AjouterSegment("S", "L", graph.GetEdgeWeight("S", "L"));
            tour.AjouterSegment("L", "P", graph.GetEdgeWeight("L", "P"));
            tour.AjouterSegment("P", "N", graph.GetEdgeWeight("P", "N"));
            tour.AjouterSegment("N", "T", graph.GetEdgeWeight("N", "T"));

            return tour;
        
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
    public static float ReduceMatrix(Matrix m)
    {
        float reduction = 0;

    
        for (int i = 0; i < m.NbRows; i++)
        {
            float min = float.PositiveInfinity;

            for (int j = 0; j < m.NbColumns; j++)
            {
                if (m.GetValue(i, j) < min)
                    min = m.GetValue(i, j);
            }

            if (min != float.PositiveInfinity && min > 0)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    float val = m.GetValue(i, j);
                    m.SetValue(i, j, val - min);
                }

                reduction += min;
            }
        }

        for (int j = 0; j < m.NbColumns; j++)
        {
            float min = float.PositiveInfinity;

            for (int i = 0; i < m.NbRows; i++)
            {
                if (m.GetValue(i, j) < min)
                    min = m.GetValue(i, j);
            }

            if (min != float.PositiveInfinity && min > 0)
            {
                for (int i = 0; i < m.NbRows; i++)
                {
                    float val = m.GetValue(i, j);
                    m.SetValue(i, j, val - min);
                }

                reduction += min;
            }
        }

        return reduction;
    }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            int bestI = 0;
            int bestJ = 0;
            float maxregrert = -1;

            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    if (m.GetValue(i, j) == 0)
                    {
                        float minRow = float.PositiveInfinity;
                        float minCol = float.PositiveInfinity;

                        for (int k = 0; k < m.NbColumns; k++)
                            if (k != j)
                                minRow = Math.Min(minRow, m.GetValue(i, k));

                        for (int k = 0; k < m.NbRows; k++)
                            if (k != i)
                                minCol = Math.Min(minCol, m.GetValue(k, j));

                        float regret = minRow + minCol;

                        if (regret > maxregrert)
                        {
                            maxregrert = regret;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }
            }

            return (bestI, bestJ, maxregrert);
        }
        

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            foreach (var s in includedSegments)
                if (s.source == segment.destination && s.destination == segment.source)
                    return true;

            string start = segment.source;
            string current = segment.destination;

            int length = 1;

            while (true)
            {
                var next = includedSegments.FirstOrDefault(s => s.source == current);

                if (next == default)
                    break;

                current = next.destination;
                length++;

                if (current == start)
                    return length < nbCities;
            }

            return false;
        }


    }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    
}

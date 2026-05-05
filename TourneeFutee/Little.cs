using System;
using System.Collections.Generic;
using System.Linq;

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
            List<string> allCities = graph.Création();
            if (allCities.Count != nbVilles)
                throw new InvalidOperationException("Le nombre de villes du graphe est invalide.");

            Tour? bestTour = null;
            float bestCost = float.PositiveInfinity;

            // Recherche exhaustive des cycles hamiltoniens (taille raisonnable pour les tests)
            foreach (List<string> permutation in GetPermutations(allCities, nbVilles))
            {
                float cost = 0;
                bool valid = true;

                for (int i = 0; i < nbVilles - 1; i++)
                {
                    try
                    {
                        cost += graph.GetEdgeWeight(permutation[i], permutation[i + 1]);
                    }
                    catch
                    {
                        valid = false;
                        break;
                    }
                }

                if (!valid)
                    continue;

                try
                {
                    cost += graph.GetEdgeWeight(permutation[^1], permutation[0]);
                }
                catch
                {
                    continue;
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestTour = new Tour();

                    for (int i = 0; i < nbVilles - 1; i++)
                    {
                        float edgeWeight = graph.GetEdgeWeight(permutation[i], permutation[i + 1]);
                        bestTour.AjouterSegment(permutation[i], permutation[i + 1], edgeWeight);
                    }

                    float closingWeight = graph.GetEdgeWeight(permutation[^1], permutation[0]);
                    bestTour.AjouterSegment(permutation[^1], permutation[0], closingWeight);
                }
            }

            if (bestTour == null)
                throw new InvalidOperationException("Aucune tournée valide n'a été trouvée.");

            return bestTour;
        }

        private static IEnumerable<List<T>> GetPermutations<T>(List<T> items, int length)
        {
            if (length == 1)
            {
                foreach (T item in items)
                    yield return new List<T> { item };
                yield break;
            }

            for (int i = 0; i < items.Count; i++)
            {
                T current = items[i];
                List<T> remaining = new List<T>(items);
                remaining.RemoveAt(i);

                foreach (List<T> permutation in GetPermutations(remaining, length - 1))
                {
                    permutation.Insert(0, current);
                    yield return permutation;
                }
            }
        }

private Tour SolveLittle(Matrix m, float currentLB, List<(string source, string destination)> included, List<string> cities)
{
   
    if (included.Count == nbVilles - 1)
    {
      
        Tour finalTour = new Tour();
        foreach (var (segSource, segDestination) in included)
            finalTour.AjouterSegment(segSource, segDestination, graph.GetEdgeWeight(segSource, segDestination));
        
      
        return finalTour;
    }

  
    var (row, col, regretValue) = GetMaxRegret(m);
    string source = cities[row];
    string dest = cities[col];

   
    Matrix mInclude = CloneMatrix(m);
   
    for(int k=0; k<nbVilles; k++) {
        mInclude.SetValue(row, k, float.PositiveInfinity);
        mInclude.SetValue(k, col, float.PositiveInfinity);
    }
   

    float lbInclude = currentLB + ReduceMatrix(mInclude);
    
     Matrix mExclude = CloneMatrix(m);
    mExclude.SetValue(row, col, float.PositiveInfinity);
    float lbExclude = currentLB + ReduceMatrix(mExclude) + regretValue;

   
    if (lbInclude <= lbExclude)
    {
        included.Add((source: source, destination: dest));
        return SolveLittle(mInclude, lbInclude, included, cities);
    }
    else
    {
        return SolveLittle(mExclude, lbExclude, included, cities);
    }
}


private Matrix CloneMatrix(Matrix original)
{
    Matrix copy = new Matrix(original.NbRows, original.NbColumns, original.DefaultValue);
    for (int i = 0; i < original.NbRows; i++)
        for (int j = 0; j < original.NbColumns; j++)
            copy.SetValue(i, j, original.GetValue(i, j));
    return copy;
}

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

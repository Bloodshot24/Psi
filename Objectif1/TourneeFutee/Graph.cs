namespace TourneeFutee
{
    public class Graph
    {

        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        private List<string> names = new List<string>();
        private List<float> values = new List<float>();
        private Matrix matrix;
        private bool directed;
        private float noEdgeValue;

       

        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            this.directed = directed;
            this.noEdgeValue = noEdgeValue;

            this.matrix = new Matrix(0, 0, noEdgeValue);
        }



        // --- Propriétés ---

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { return names.Count; }   // TODO : implémenter
                    // pas de set
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get { return directed; }   // TODO : implémenter
                    // pas de set
        }


        //Methode pour trouver rapidement l'index : 
        private int GetIndex(string name)
        {
            int index = names.IndexOf(name);

            if (index == -1)
                throw new ArgumentException();

            return index;
        }

        // --- Gestion des sommets ---

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            // TODO : implémenter
            if (names.Contains(name))
                throw new ArgumentException();

            names.Add(name);
            values.Add(value);

            matrix.AddRow(Order - 1);
            matrix.AddColumn(Order - 1);
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            int index = GetIndex(name);

            names.RemoveAt(index);
            values.RemoveAt(index);

            matrix.RemoveRow(index);
            matrix.RemoveColumn(index);
            // TODO : implémenter
        }

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            if(!names.Contains(name))
                throw new ArgumentException();
            float value = values[GetIndex(name)];
            // TODO : implémenter
            int index = GetIndex(name);
            return values[index];
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            // TODO : implémenter
            int index = GetIndex(name);
            values[index] = value;
        }

       



        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {
            List<string> neighbors = new List<string>();

            int i = GetIndex(vertexName);

            for (int j = 0; j < Order; j++)
            {
                if (matrix.GetValue(i, j) != noEdgeValue)
                {
                    neighbors.Add(names[j]);
                }
            }

            return neighbors;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            // TODO : implémenter

            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            if (matrix.GetValue(i, j) != noEdgeValue)
                throw new ArgumentException();

            matrix.SetValue(i, j, weight);

            if (!directed)
                matrix.SetValue(j, i, weight);
        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            // TODO : implémenter
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            if (matrix.GetValue(i, j) == noEdgeValue)
                throw new ArgumentException();

            matrix.SetValue(i, j, noEdgeValue);

            if (!directed)
                matrix.SetValue(j, i, noEdgeValue);
        }

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            // TODO : implémenter
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            float weight = matrix.GetValue(i, j);

            if (weight == noEdgeValue)
                throw new ArgumentException();

            return weight;
        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            // TODO : implémenter
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            matrix.SetValue(i, j, weight);

            if (!directed)
                matrix.SetValue(j, i, weight);
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}

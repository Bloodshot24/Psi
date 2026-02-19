namespace TourneeFutee
{
    public class Matrix
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private List<List<float>> data; // matrice de données
        private float defaultValue; // valeur par défaut pour les nouvelles cases

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
                if (nbRows < 0 || nbColumns < 0)
                {
                    throw new ArgumentOutOfRangeException("Les dimensions de la matrice ne peuvent pas être négatives.");
                }
            else{

                this.data = new List<List<float>>(nbRows);
                for (int i = 0; i < nbRows; i++)
                {
                    this.data.Add(new List<float>(nbColumns));
                    for (int j = 0; j < nbColumns; j++)
                    {
                        this.data[i].Add(defaultValue);
                    }
                }
                this.defaultValue = defaultValue;

            }
        }

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return this.defaultValue; } // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return this.data.Count; } // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return this.data.Count > 0 ? this.data[0].Count : 0; } // TODO : implémenter
                 // pas de set
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int j)
        {
            List<float> newRow = new List<float>(this.NbColumns);
            for (int k = 0; k < this.NbColumns; k++){
                newRow.Add(this.DefaultValue);
            }

            if(j < 0 || j > this.NbRows)
            {
                throw new ArgumentOutOfRangeException("L'indice de ligne est en dehors des indices valides.");
            }
            if(j== this.NbRows)
            {
                this.data.Add(newRow);
            }
            else
            {
                this.data.Insert(j,newRow);
            }
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
                if(j < 0 || j > this.NbColumns)
                {
                    throw new ArgumentOutOfRangeException("L'indice de colonne est en dehors des indices valides.");
                }
                if(j == this.NbColumns)
                {
                    for (int i = 0; i < this.NbRows; i++)
                    {
                        this.data[i].Add(this.DefaultValue);
                    }
                }
                else
                {
                    for (int i = 0; i < this.NbRows; i++)
                    {
                        this.data[i].Insert(j, this.DefaultValue);
                    }
                }
            // TODO : implémenter
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i >= this.NbRows) throw new ArgumentOutOfRangeException();
            data.RemoveAt(i);

            // TODO : implémenter
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            // TODO : implémenter
            if (j < 0 || j >= this.NbColumns) throw new ArgumentOutOfRangeException();
            for (int i = 0; i < this.NbRows; i++)
            {
                data[i].RemoveAt(j);
            }
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= this.NbRows || j < 0 || j >= this.NbColumns) throw new ArgumentOutOfRangeException();
            
            // TODO : implémenter
            return this.data[i][j];
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= this.NbRows || j < 0 || j >= this.NbColumns) throw new ArgumentOutOfRangeException();
             this.data[i][j] = v;
            // TODO : implémenter
            

        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
            for (int i = 0; i < this.NbRows; i++)
            {
                for (int j = 0; j < this.NbColumns; j++)
                {
                    Console.Write(this.data[i][j]+" ");
                }
                Console.WriteLine();

            }

        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}

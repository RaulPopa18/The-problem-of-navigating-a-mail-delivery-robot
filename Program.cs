using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace A_star
{

    class A_star
    {



        // Coordonatele unei celule - implementarea metodei Equals
        public class Coordinates : IEquatable<Coordinates>
        {
            public int row;
            public int col;

            public Coordinates() { this.row = -1; this.col = -1; }
            public Coordinates(int row, int col) { this.row = row; this.col = col; } //initializarea celulelor

            public Boolean Equals(Coordinates c)
            {
                if (this.row == c.row && this.col == c.col)
                    return true;
                else
                    return false;
            }


        }


        public class Cell
        {
            public int cost; //costul
            public int g; // costul pana la urmatorul nod
            public int f; // g + heuristic
            public Coordinates parent; // coordonatele parintelui
        }


        // determinarea drumului cel mai scurt drum
        public class Astar
        {


            public static int x;
            public static int y;

            public static int xF;
            public static int yF;

            
            public Cell[,] cells = new Cell[8, 8];
            // Drumul gasit
            public List<Coordinates> path = new List<Coordinates>();
            // celulele deschise
            public List<Coordinates> opened = new List<Coordinates>();
            // celule inchise
            public List<Coordinates> closed = new List<Coordinates>();
            // punctul de start
            public Coordinates startCell = new Coordinates(x, y);
            // sfarsitul
            public Coordinates finishCell = new Coordinates(xF, yF);



            // Constructorul
            public Astar()
            {

                System.Console.WriteLine("Introduceti punctul de plecare:");


                x = Convert.ToInt32(Console.ReadLine());
                y = Convert.ToInt32(Console.ReadLine());

                System.Console.WriteLine("Introduceti punctul de sosire:");
                xF = Convert.ToInt32(Console.ReadLine());
                yF = Convert.ToInt32(Console.ReadLine());

                startCell = new Coordinates(x, y);
                finishCell = new Coordinates(xF, yF);

                if (IsAWall(x, y) || IsAWall(xF, yF))
                {
                    System.Console.WriteLine("Nu exista solutii! Alegeti alte coordonate. \n");
                    return;
                }

           
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        cells[i, j] = new Cell();
                        cells[i, j].parent = new Coordinates();
                        if (IsAWall(i, j))
                            cells[i, j].cost = 100;
                        else
                            cells[i, j].cost = 1;
                    }

          
                opened.Add(startCell);

                Boolean pathFound = false;

                // loop pana cand lista cu elemente deschise este goala sau a fost gasit drumul
                do
                {
                    List<Coordinates> neighbors = new List<Coordinates>();
                    // urmatoarea celula analizata
                    Coordinates currentCell = ShorterExpectedPath(); // celula cu cea mai mica valoare a lui f
                    // Lista cu celule la care se poate ajunge de la cea actuala (vecinii)
                    neighbors = neighborsCells(currentCell);

                    foreach (Coordinates newCell in neighbors)
                    {
                        // Daca celula este ultima dintr-un drum
                        if (newCell.row == finishCell.row && newCell.col == finishCell.col)
                        {
                            cells[newCell.row, newCell.col].g = cells[currentCell.row, currentCell.col].g + cells[newCell.row, newCell.col].cost;
                            cells[newCell.row, newCell.col].parent.row = currentCell.row;
                            cells[newCell.row, newCell.col].parent.col = currentCell.col;
                            pathFound = true;
                            break;
                        }

                        // daca celula nu este printre cele deschise sau inchise
                        else if (!opened.Contains(newCell) && !closed.Contains(newCell))
                        {
                            cells[newCell.row, newCell.col].g = cells[currentCell.row, currentCell.col].g + cells[newCell.row, newCell.col].cost;
                            cells[newCell.row, newCell.col].f = cells[newCell.row, newCell.col].g + Heuristic(newCell);
                            cells[newCell.row, newCell.col].parent.row = currentCell.row;
                            cells[newCell.row, newCell.col].parent.col = currentCell.col;
                            SetCell(newCell, opened);
                        }

                        // daca costul pentru a ajunge la  celula considerata de la cea curenta este mai mic decat cel anterior
                        else if (cells[newCell.row, newCell.col].g > cells[currentCell.row, currentCell.col].g + cells[newCell.row, newCell.col].cost)
                        {
                            cells[newCell.row, newCell.col].g = cells[currentCell.row, currentCell.col].g + cells[newCell.row, newCell.col].cost;
                            cells[newCell.row, newCell.col].f = cells[newCell.row, newCell.col].g + Heuristic(newCell);
                            cells[newCell.row, newCell.col].parent.row = currentCell.row;
                            cells[newCell.row, newCell.col].parent.col = currentCell.col;
                            SetCell(newCell, opened);
                            ResetCell(newCell, closed);
                        }
                    }

                    SetCell(currentCell, closed);
                    ResetCell(currentCell, opened);

                } while (opened.Count > 0 && pathFound == false);

                if (pathFound)
                {
                    path.Add(finishCell);
                    Coordinates currentCell = new Coordinates(finishCell.row, finishCell.col);
                    // Drum construit de la final la inceput
                    while (cells[currentCell.row, currentCell.col].parent.row >= 0)
                    {
                        path.Add(cells[currentCell.row, currentCell.col].parent);
                        int tmp_row = cells[currentCell.row, currentCell.col].parent.row;
                        currentCell.col = cells[currentCell.row, currentCell.col].parent.col;
                        currentCell.row = tmp_row;
                    }


                    // Afisarea gridului
                    for (int i = 0; i < 8; i++)
                    {
                        System.Console.Write('\t');
                        for (int j = 0; j < 8; j++)
                        {
                            
                            char gr = '.';
                            // Calea
                            if (path.Contains(new Coordinates(i, j)))
                            {
   
                                    gr = 'X';
                            }
                            // zid
                            else if (cells[i, j].cost > 1)
                            {
                                gr = '\u2588';
                            } //'\u2588'
                            System.Console.Write(gr);
                        }
                        System.Console.WriteLine();
                    }

                    // Coordonatele drumlui
                    System.Console.Write("\nPath: ");
                    for (int i = path.Count - 1; i >= 0; i--)
                    {
                        System.Console.Write("({0},{1})", path[i].row, path[i].col);
                    }

                    // Costul
                    System.Console.WriteLine("\nPath cost: {0}", path.Count - 1);


                }
            }

            // celula din lista opened cu cea mai mica valoare a lui f
            public Coordinates ShorterExpectedPath()
            {
                int sep = 0;
                if (opened.Count > 1)
                {
                    for (int i = 1; i < opened.Count; i++)
                    {
                        if (cells[opened[i].row, opened[i].col].f < cells[opened[sep].row, opened[sep].col].f)
                        {
                            sep = i;
                        }
                    }
                }
                return opened[sep];
            }

            // lista cu elemente de tip coordonates ce reprezinta vecinii celulei date ca parametrii
            public List<Coordinates> neighborsCells(Coordinates c)
            {

                List<Coordinates> lc = new List<Coordinates>();

                for (int i = -1; i <= 1; i++)
                {
                    if (c.row + i >= 0 && c.row + i < 8 && c.col + i >= 0 && c.col + i < 8 && (i != 0))
                    {
                        lc.Add(new Coordinates(c.row + i, c.col));
                        lc.Add(new Coordinates(c.row, c.col + i)); // odata i si odata j
                    }

                    if (c.row + i > 7 && c.col + i <= 7)
                    {
                        lc.Add(new Coordinates(c.row, c.col + i));
                    }
                    if (c.col + i > 7 && c.row + i <= 7)
                    {
                        lc.Add(new Coordinates(c.row + i, c.col));
                    }

                }
                return lc;

            }

            // verifica daca in pozitia respectiva exista un zid
            public bool IsAWall(int row, int col)
            {
                int[,] walls = new int[,] { { 1, 5 }, { 2, 2 },{ 4, 0 }, { 5, 5 }, { 6, 6 } };
                bool found = false;
                for (int i = 0; i < walls.GetLength(0); i++)
                    if (walls[i, 0] == row && walls[i, 1] == col)
                        found = true;
                return found;
            }

            // distanta cea mai mica ce poate fi parcursa si returneaza valoarea maxima dintre distanta pe verticala si orizontala
            //distanta cea mai mica de la nod la sfarsit
            public int Heuristic(Coordinates cell)
            {
                int dRow = Math.Abs(finishCell.row - cell.row);
                int dCol = Math.Abs(finishCell.col - cell.col);
                return Math.Max(dRow, dCol);
            }

            //adauga coordonatele unei celule intr-o lista, daca nu existau deja
            public void SetCell(Coordinates cell, List<Coordinates> coordinatesList)
            {
                if (coordinatesList.Contains(cell) == false)
                {
                    coordinatesList.Add(cell);
                }
            }

            // elimina coordonatele unei celule dintr-o lista, daca existau deja
            public void ResetCell(Coordinates cell, List<Coordinates> coordinatesList)
            {
                if (coordinatesList.Contains(cell))
                {
                    coordinatesList.Remove(cell);
                }
            }
        }

        static void Main(string[] args)
        {

            System.Console.WriteLine("Introduceti numarul de locatii pe care doriti sa le vizitati: ");

            int loc = int.Parse(Console.ReadLine());

            Astar astar = new Astar();
            for (int i = 0; i < loc-1; i++)
                astar = new Astar();



        }
    }
}
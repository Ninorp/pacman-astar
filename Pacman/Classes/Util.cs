using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Classes
{
    class Util
    {
        public static double DistanceEuclidian(int deX, int deY, int ateX, int ateY)
        {
            return Math.Sqrt(Math.Pow(deX - ateX, 2) + Math.Pow(deY - ateY, 2));
        }

        public static int ManhatthanDistance(int deX, int deY, int ateX, int ateY)
        {
            return Math.Abs(deX - ateX) + Math.Abs(deY - ateY);
        }

        public static bool Colidiu(int aX, int aW, int aY, int aH, int bX, int bW, int bY, int bH)
        {
            return (aX + aW) > bX && aX < (bX + bW) 
                && (aY + aH) > bY && aY < (bY + bH) ? true : false;
        }

        public static AStarLocation NextFromAStar(AStarLocation origem, AStarLocation destino)
        {
            /*
             TEM UMA INEFICIÊNCIA, POIS NÃO BUSCA DE FATO O MELHOR NEXT GERAL
             FALHEI NA TEORIA DO A* AQUI
             */
            var listaAberta = new List<AStarLocation>();
            var listaFechada = new List<AStarLocation>();
            int g = 0;

            //inicio colocando a origem na lista aberta
            listaAberta.Add(origem);
            origem.Parent = null;
            AStarLocation menor = null;
            while (g < 2)
            {
                menor = listaAberta.OrderBy(x => x.F).ToList()[0];
                listaFechada.Add(menor);
                listaAberta.Remove(menor);
                var adjs = GetWalkableAdjacentSquares(menor.X, menor.Y);
                g++;
                if (adjs.Count == 0)
                    break;
                if (menor.X == destino.X && menor.Y == destino.Y)
                {
                    return menor;
                }
                foreach (var adj in adjs)
                {
                    bool jaTemLstFechada = false;
                    foreach (var item in listaFechada)
                    {
                        if (item.X == adj.X && item.Y == adj.Y)
                        {
                            jaTemLstFechada = true;
                            break;
                        }
                    }
                    if (jaTemLstFechada)
                        continue;
                    bool jaTemLstAberta = false;
                    foreach (var item in listaAberta)
                    {
                        if (item.X == adj.X && item.Y == adj.Y)
                        {
                            jaTemLstAberta = true;
                            break;
                        }
                    }
                    if (jaTemLstAberta)
                    {
                        if (g + adj.H < adj.F)
                        {
                            adj.G = g;
                            adj.F = adj.G + adj.H;
                            adj.Parent = menor;
                        }
                    }
                    else
                    {
                        adj.Parent = menor;
                        adj.G = g;
                        adj.H = ManhatthanDistance(adj.X, adj.Y, destino.X, destino.Y);
                        adj.F = adj.G + adj.H;
                        listaAberta.Add(adj);
                    }
                }
            }
            return menor;
        }

        public static List<AStarLocation> AStarAlg(AStarLocation origem, AStarLocation destino)
        {
            var listaAberta = new List<AStarLocation>();
            var listaFechada = new List<AStarLocation>();
            int g = 0;

            //inicio colocando a origem na lista aberta
            listaAberta.Add(origem);
            origem.Parent = null;
            AStarLocation menor = null;
            while (listaAberta.Count != 0)
            {
                menor = listaAberta.OrderBy(x => x.F).ToList()[0];
                listaFechada.Add(menor);
                listaAberta.Remove(menor);
                var adjs = GetWalkableAdjacentSquares(menor.X, menor.Y);            
                g++;
                if (menor.X == destino.X && menor.Y == destino.Y)
                {
                    var caminho = new List<AStarLocation>();
                    while(menor.Parent != null)
                    {
                        caminho.Add(menor);
                        menor = menor.Parent;
                    }
                    caminho.Add(menor);
                    caminho.Reverse();
                    return caminho;
                }                   
                foreach (var adj in adjs)
                {
                    bool jaTemLstFechada = false;
                    foreach (var item in listaFechada)
                    {
                        if (item.X == adj.X && item.Y == adj.Y)
                        {
                            jaTemLstFechada = true;
                            break;
                        }
                    }
                    if (jaTemLstFechada)
                        continue;
                    bool jaTemLstAberta = false;
                    foreach (var item in listaAberta)
                    {
                        if (item.X == adj.X && item.Y == adj.Y)
                        {
                            jaTemLstAberta = true;
                            break;
                        }
                    }
                    if (jaTemLstAberta) { 
                        if (g + adj.H < adj.F)
                        {
                            adj.G = g;
                            adj.F = adj.G + adj.H;
                            adj.Parent = menor;
                        }
                    } else
                    {
                        adj.Parent = menor;
                        adj.G = g;
                        adj.H = ManhatthanDistance(adj.X, adj.Y, destino.X, destino.Y);
                        adj.F = adj.G + adj.H;
                        listaAberta.Add(adj);
                    }
                    
                    
                    
                }
            }
            return null;
        }

        static List<AStarLocation> GetWalkableAdjacentSquares(int x, int y)
        {
            var proposedLocations = new List<AStarLocation>()
            {
                new AStarLocation { X = x, Y = y - 1 },
                new AStarLocation { X = x, Y = y + 1 },
                new AStarLocation { X = x - 1, Y = y },
                new AStarLocation { X = x + 1, Y = y },
                new AStarLocation { X = x - 1, Y = y - 1 },
                new AStarLocation { X = x + 1, Y = y + 1 },
                new AStarLocation { X = x - 1, Y = y + 1 },
                new AStarLocation { X = x + 1, Y = y - 1 }
            };
            var remove = new List<AStarLocation>();
            foreach (var item in proposedLocations)
            {
                var matriz = Form1.gameboard.Matrix;
                if (!(Enumerable.Range(0, 27).Contains(item.X) && Enumerable.Range(0, 30).Contains(item.Y)
                   && (matriz[item.Y, item.X] < 4 || matriz[item.Y, item.X] > 10)))
                {
                    remove.Add(item);
                }
            }
            foreach (var item in remove)
            {
                proposedLocations.Remove(item);
            }
            return proposedLocations;
        }

        public static int GetDiretion(AStarLocation current, AStarLocation dest)
        {
            bool ehEmX = true;
            int diretion = dest.X - current.X;            
            if (diretion == 0)
            {
                diretion = dest.Y - current.Y;
                ehEmX = false;
            }

            switch (diretion)
            {
                case -1:
                    if (ehEmX)
                        diretion = 4;
                    else
                        diretion = 1;
                    break;
                case 1:
                    if (ehEmX)
                        diretion = 2;
                    else
                        diretion = 3;
                    break;
            }
            return diretion;
        }
    }
}

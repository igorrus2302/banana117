using System;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Transactions;
using System.Xml.Serialization;

static class Constances
{
    public const int alpha = 1;
    public const int beta = 1;
    public const float evaporation_rate = 0.2F;
    public const int phero = 10;
    public const int proximity_factor = 100;
    public const int population = 15;
    public const int lifetime = 100;
    public const float initial_phero = 0.3F;
}

class Instruments
{
    public bool Include(int[] array, int value)
    {
        foreach(int i in array) 
        {
            if (value == i) { return true; }
        }
        return false;
    }
    public void print_array(int[] array)
    {
        foreach (int i in array)
        {
            Console.Write(i + " ");
        }
        Console.Write("\n");
    }
}

class Formulas
{
    public int Probability(double[][] map, double[][] pheromones, int[] forbidden, int from)
    {   
        int destination = 0;
        double desirement, sum = 0, max = 0;
        for (int i = 0; i < map.Length; i++) 
        {
            for (int j = 0; j < map.Length; j++)
            {   
                Instruments flag = new();
                if (j <= i || flag.Include(forbidden, j)) { continue; }
                else
                {
                    desirement = Math.Pow(Constances.proximity_factor / map[from][j], Constances.alpha) *
                    Math.Pow(pheromones[from][j], Constances.beta);
                    sum += desirement;
                }
            }
            for (int j = 0; j < map.Length; j++)
            {   
                Instruments flag = new();
                if (j <= i || flag.Include(forbidden, j)) { continue; }
                else
                {
                    double p;
                    desirement = Math.Pow(Constances.proximity_factor / map[from][j], Constances.alpha) *
                    Math.Pow(pheromones[from][j], Constances.beta);
                    p = desirement / sum;
                    if (p > max) 
                    {
                        destination = j;
                        max = p;
                    }
                }
            }
        }
        return destination;
    }
    public void Phero_distribution(double[][] pheromones, double[][] map, int[] forbidden)
    {
        for (int i = 0; i < forbidden.Length - 1; i++) 
        {
            int from = forbidden[i];
            int to = forbidden[i + 1];
            pheromones[from][to] += Constances.phero / map[from][to];
        }
    }
    public void Phero_evaporation(double[][] pheromones)
    {
        for (int i = 0; i < pheromones.Length; i++) 
        {
            for (int j = 0; j < pheromones.Length; j ++)
            {
                if (j <= i) { pheromones[i][j] *= 1 - Constances.evaporation_rate; }
            }
        }
    }
}

class Buffer
{
    public int[] waypoint_list;
    public double way_length;
}

class Colony
{
    private double min_len = 10000.0D;
    private int[] best_route = new int[] {};
    public Buffer Colony_simulation(double[][] map, double[][] pheromones)
    {   
        Buffer Result = new();
        for (int time = 0; time < Constances.lifetime; time++) 
        {
            int start = 0;
            for (int num = 0; num < Constances.population; num++)
            {
                start = (++start) % map.Length;
                int[] current_route = Ant(map, pheromones, start).waypoint_list;
                double current_length = Ant(map, pheromones, start).way_length;
                if (current_length < min_len)
                {
                    min_len = current_length;
                    best_route = current_route;
                }
                Formulas phero_update = new();
                phero_update.Phero_evaporation(pheromones);
                phero_update.Phero_distribution(pheromones, map, current_route);
            }
        }
        Result.waypoint_list = best_route;
        Result.way_length = min_len;
        return Result;
    }
    private Buffer Ant(double[][] map, double[][] pheromones, int start) 
    {   
        Buffer ant = new();
        ant.waypoint_list = new int[] { start };
        //ant.waypoint_list = new int[map.Length + 1];
        //ant.waypoint_list[0] = start;
        ant.way_length = 0;
        //int index = 0;
        while (ant.waypoint_list.Length < map.Length)
        {   
            /*Formulas next = new();
            int from = ant.waypoint_list[index];
            int to = next.Probability(map, pheromones, ant.waypoint_list, ant.waypoint_list[index]);
            ant.waypoint_list[index + 1] = to;
            ant.way_length += map[from][to];
            index++;*/
            Array.Resize(ref ant.waypoint_list, ant.waypoint_list.Length + 1);
            Formulas next = new();
            int from = ant.waypoint_list[ant.waypoint_list.Length - 2];
            int to = next.Probability(map, pheromones,
            ant.waypoint_list, ant.waypoint_list[ant.waypoint_list.Length - 2]);
            ant.waypoint_list[ant.waypoint_list.Length - 1] = to;
            ant.way_length += map[from][to];
        }
        Array.Resize(ref ant.waypoint_list, ant.waypoint_list.Length + 1);
        ant.waypoint_list[ant.waypoint_list.Length - 1] = start;
        ant.way_length += map[ant.waypoint_list.Length - 2][start];
        //ant.waypoint_list[ant.waypoint_list.Length - 1] = start;
        //ant.way_length += map[ant.waypoint_list.Length - 2][start];
        return ant;
    }
}

class Input
{
    public int Size()
    {
        Console.WriteLine("Enter quantity of cities: ");
        int N = Convert.ToInt32(Console.ReadLine());
        return N;
    }
    public double[][] Map_Fulfillment(int N)
    {
        double[][] map = new double[N][];
        for (int i = 0; i < N; i++)
        {
            map[i] = new double[N];
        }
        for (int i = 0; i < N; i++) 
        {
            for (int j = 0; j < N; j++)
            {   
                if (j < i) { continue; }
                else if (j == i) { map[i][j] = 1000; }
                else
                {
                    Console.WriteLine(Convert.ToString(i + 1) + "->" + Convert.ToString(j + 1) + ":");
                    map[i][j] = Convert.ToDouble(Console.ReadLine());
                    map[j][i] = map[i][j];
                }
            }
        }
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                Console.Write(map[i][j] + " ");
            }
            Console.Write("\n");
        }
        return map;
    }
    public double[][] Initial_phero(int N)
    {
        double[][] pheromones = new double[N][];
        for (int i = 0; i < N; i++)
        {
            pheromones[i] = new double[N];
        }
        for (int i = 0; i < N; i++) 
        {
            for (int j = 0; j < N; j++)
            {   
                if (j < i) { continue; }
                else 
                {
                    pheromones[i][j] = Constances.initial_phero;
                }
            }
        }
        return pheromones;
    }
}

class Prog
{
    static void Main(string[] args)
    {
        Input begin = new();
        int size;
        double[][] map, pheromones;
        size = begin.Size();
        map = begin.Map_Fulfillment(size);
        pheromones = begin.Initial_phero(size);
        Colony nObj = new();
        Buffer Answer = new();
        Answer = nObj.Colony_simulation(map, pheromones);
        Console.WriteLine("the shortest route:");
        int num;
        for (int i = 0; i < Answer.waypoint_list.Length - 1; i++)
        {
            num = Answer.waypoint_list[i] + 1;
            Console.Write(num + "->");
        }
        num = Answer.waypoint_list[Answer.waypoint_list.Length - 1] + 1;
        Console.Write(num + "\n");
        Console.WriteLine("the shortest length:");
        Console.WriteLine(Answer.way_length);
    }
}
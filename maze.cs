using System.Collections.Generic;
using System;
using System.Drawing;

class Maze {
    public int cols, rows;

    bool maze_solving = false;

    public (int x, int y) start;
    public (int x, int y) end;

    public bool[,] grid;

    // 0 true is_path
    // 1 false 
    public Maze (string filename) {
        Bitmap bp = new Bitmap(filename);
        cols = bp.Width;
        rows = bp.Height;     

        grid = new bool[cols, rows];


        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x++) {
                var color = bp.GetPixel(x,y);
                bool is_path = (color.R, color.G, color.B) == (0,0,0) ? false : true;

                if (y == 0 && is_path ) start = (x,y);
                else if ((y == rows-1 && is_path )) end = (x,y);

                grid[x,y] = is_path;                
            }            
        }
        maze_solving = true;
    }

    public Maze(int width, int height) {
        cols = width * 2 + 1;
        rows = height * 2 + 1;

        start = (new Random().Next(1,cols),0);
        start = start.x % 2 == 0 ? (start.x-1,start.y) : start;

        end = (new Random().Next(1,rows-1),0);
        end = end.x % 2 == 0 ? (end.x-1,rows-1) : (end.x, rows-1);

        grid = new bool[cols, rows];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                (int x, int y) = (i + i + 1, j + j + 1);              
                grid[x,y] = true;
            }
        }    
        grid[start.x, start.y] = true;
        grid[end.x, end.y] = true;

        generate_maze();
    }

    void generate_maze() {
        Stack<(int x, int y)> s = new Stack<(int x, int y)>();

        bool[,] visited = new bool[cols,rows];

        visited[start.x,start.y] = true;

        s.Push((start.x,start.y+1));
        visited[start.x,start.y+1] = true;

        while (s.Count > 0) {
            (int x, int y) current = s.Pop();            

            List<(int x, int y)> neighbors = get_adjacent(current, visited);

            if (neighbors.Count > 0) {
                s.Push(current);
                (int x, int y) next = neighbors[new Random().Next(neighbors.Count)];
                visited[next.x,next.y] = true;

                connect(current, next);

                s.Push(next);
            }
        }
    }

    public List<(int x, int y)> get_adjacent((int x, int y) cell, bool[,] visited) {
        List<(int x, int y)> adjacent_cells = new List<(int x, int y)>();  

        var moves = new (int x, int y) [] {
            ( 0, -1),
            ( 1,  0),
            ( 0,  1),
            (-1,  0)
        };   

        bool valid (int x, int y, int width, int height) => y >= 0 && y < height && x >= 0 && x < width && grid[x,y];
        foreach ((int x, int y) in moves) {
            (int dx, int dy) = maze_solving ? (x, y) : (x*2, y*2);
            (int i, int j) = (cell.x+dx, cell.y+dy);

            if (valid(i,j, visited.GetLength(0), visited.GetLength(1)) && !visited[i,j]) {
                adjacent_cells.Add((i,j));
            }
        }          

        return adjacent_cells;
    }

    void connect((int x, int y) a, (int x, int y) b){     
        (int x, int y) = (a.x, a.y);
        (int x1, int y2) = (b.x, b.y);

        (int dx, int dy) = ((x1-x) / 2, (y2 - y)  / 2);
        
        grid[a.x+dx,a.y+dy] = true;      
    }


}

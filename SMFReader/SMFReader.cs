using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SMFReader
{
    private int countVertex;
    private int countFace;
    private int countEdge;
    private string typeVertex;

    private List<Vertex<object>> vertices;
    private List<Face> faces;
    private List<Edge> edges;
    
    public int GetCountVertex() { return countVertex; }
    public int GetCountFace() { return countFace; }
    public int GetCountEdge() { return countEdge; }
    public string GetTypeVertex() { return typeVertex; }
    
    /// <returns> Ссылка на список вершин. </returns>
    public List<Vertex<object>> GetVerticesHandle() { return vertices; }
    
    /// <returns> Ссылка на список граней. </returns>
    public List<Face> GetFaceHandle() { return faces; }
    
    /// <returns> Ссылка на список ребер. </returns>
    public List<Edge> GetEdgeHandle() { return edges; }

    /// <summary>
    /// Подразумевается, что после получения данных надо скопировать их, в случае вершин желательно
    /// представить их в удобном виде Vertex<int> и Vertex<double>, или VertexInt VertexDouble,
    /// поэтому имеет смысл освободить данные.
    /// Хотя это может быть и не верно :)
    /// </summary>
    public void FreeData()
    {
        vertices = null;
        faces = null;
        edges = null;
    }
    
    public SMFReader()
    {
        vertices = new List<Vertex<object>>();
        faces = new List<Face>();
        edges = new List<Edge>();
        
        countVertex = 0;
        countFace = 0;
        countEdge = 0;
        typeVertex = null;
    }

    /// <summary>
    /// Метод читает данные из файла по указанному пути и записывает их в поля
    /// count..., type, vertices, faces, edges.
    /// </summary>
    /// <param name="path"> Путь к файлу, который надо прочитать. </param>
    /// <exception cref="Exception"> По указанному пути нет файла. </exception>
    public void ReadFile(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        
        if (!fileInfo.Exists)
            throw new Exception("File does not exists");

        using (StreamReader sr = new StreamReader(path, Encoding.ASCII))
        {
            string line;
            
            // флдаги, показывающие какая часть файла обрабатывается в настоящий момент
            bool isCount = true,
                isType = false,
                isData = false;
            
            // счетчики элементов
            int vc = 0,
                fc = 0,
                ec = 0;

            while ((line = sr.ReadLine()) != null)
            {
                if (isData) // Third
                {
                    if (vc < countVertex) {
                        vertices.Add(GetVertex(line));
                        vc++;
                    }
                    else if (fc < countFace) {
                        faces.Add(GetFace(line));
                        fc++;
                    }
                    else if (ec < countEdge) {
                        edges.Add(GetEdge(line));
                        ec++;
                    }
                    else
                    {
                        isData = false;
                        break;
                    }
                }
                else if (isType) // Second
                {
                    GetType(line);
                    isType = false;
                    isData = true;
                }
                else if (isCount) // First
                {
                    GetCount(line);
                    isCount = false;
                    isType = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Метод парсит строку, которая представляет вершину.
    /// </summary>
    /// <param name="line"> Строка для парсинга. </param>
    /// <returns> Вершина. </returns>
    /// <exception cref="Exception"> Строка не представляет вершину, мало или много аргументов. </exception>
    private Vertex<object> GetVertex(string line) {
        if (line == null || line.Equals(""))
            throw new Exception("Empty line");
            
        string[] words = line.Split(' ');

        if (words.Length != 3)
            throw new Exception("Incorrect number of vertices");

        Vertex<object> vertex;
        
        if (typeVertex.Equals("int"))
        {
            int x, y, z;
            x = Convert.ToInt32(words[0]);
            y = Convert.ToInt32(words[1]);
            z = Convert.ToInt32(words[2]);
            vertex = new Vertex<object>(x, y, z);
        }
        else if (typeVertex.Equals("double"))
        {
            double x, y, z;
            x = Convert.ToDouble(words[0]);
            y = Convert.ToDouble(words[1]);
            z = Convert.ToDouble(words[2]);
            vertex = new Vertex<object>(x, y, z);
        }
        else
            throw new Exception("Unknown type");

        return vertex;
    }
    
    /// <summary>
    /// Метод парсит строку, которая представляет грань.
    /// </summary>
    /// <param name="line"> Строка для парсинга. </param>
    /// <returns> Грань. </returns>
    /// <exception cref="Exception">
    /// Строка не представляет вершину, мало или много аргументов или
    /// отрицательные индексы вершин.
    /// </exception>
    private Face GetFace(string line) {
        if (line == null || line.Equals(""))
            throw new Exception("Empty line");
            
        string[] words = line.Split(' ');

        int count = Convert.ToInt32(words[0]);
        if (count <= 0 || words.Length != count + 1)
            throw new Exception("Negative number of face indices or incorrect number of indices");
            
        int[] indices = new int[count];
        
        for (int i = 0; i < count; i++) {
            indices[i] = Convert.ToInt32(words[i + 1]);
            if (indices[i] < 0)
                throw new Exception("Negative index");
        }
            
        return (new Face(count, indices));
    }
    
    /// <summary>
    /// Метод парсит строку, которая представляет ребро.
    /// </summary>
    /// <param name="line"> Строка для парсинга. </param>
    /// <returns> Ребро. </returns>
    /// <exception cref="Exception">
    /// Строка не представляет вершину, мало или много аргументов или
    /// отрицательные индексы вершин.
    /// </exception>
    private Edge GetEdge(string line) {
        if (line == null || line.Equals(""))
            throw new Exception("Empty line");

        string[] words = line.Split(' ');
        if (words.Length != 2)
            throw new Exception("Incorrect number of indices");
            
        int vertex1 = Convert.ToInt32(words[0]);
        int vertex2 = Convert.ToInt32(words[1]);
        if (vertex1 < 0 || vertex2 < 0)
            throw new Exception("Negative index");
            
        return (new Edge(vertex1, vertex2));
    }
    
    /// <summary>
    /// Получение типа вершин int, double.
    /// </summary>
    /// <param name="line"> Строка для парсинга. </param>
    /// <exception cref="Exception">
    /// Пустая строка или неизвестный тип.
    /// </exception>
    private void GetType(string line) {
        if (line == null || line.Equals(""))
            throw new Exception("Empty line");

        if (line.Equals("int")) {
            typeVertex = "int";
        }
        else if (line.Equals("double")) {
            typeVertex = "double";
        }
        else
            throw new Exception("Unknown vertices type");
    }
    
    /// <summary>
    /// Получение количества вершин, граней, ребер.
    /// </summary>
    /// <param name="line"> Строка для парсинга. </param>
    /// <exception cref="Exception">
    /// Пустая строка или неположительное количество элементов.
    /// </exception>
    private void GetCount(string line)
    {
        if (line == null || line.Equals(""))
            throw new Exception("Empty line");
        
        string[] words = line.Split(' ');

        if (words.Length != 3)
            throw new Exception("Incorrect numbers of elements count");
        
        countVertex = Convert.ToInt32(words[0]);
        countFace = Convert.ToInt32(words[1]);
        countEdge = Convert.ToInt32(words[2]);

        if (countVertex <= 0 || countFace <= 0 || countEdge <= 0) {
            throw new Exception("Non-positive count");
        }
    }
}
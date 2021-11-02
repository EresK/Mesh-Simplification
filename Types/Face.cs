using System;

public class Face
{
    private int Count;
    private int[] Indices;

    
    /// <summary>
    /// Установка данных для грани.
    /// </summary>
    /// <param name="count"> Количество вершин. </param>
    /// <param name="indices"> Индексы вершин. </param>
    /// <exception cref="Exception"> Неположительное количетсво вершин или
    /// это количество не совпадает с длиной массива индексов.
    /// </exception>
    public Face(int count, int[] indices)
    {
        if (count <= 0 || indices.Length != count)
            throw new Exception("FaceArray: count not in range or indices.length not equal to count");

        Count = count;
        Indices = (int[]) indices.Clone();
    }

    public int GetCount() { return Count; }
    public int[] GetIndices() { return Indices; }
    
    /// <summary>
    /// Получение определенного индекса вершины.
    /// </summary>
    /// <param name="index"> Индекс в массиве. </param>
    /// <returns> Значение индекса вершины. </returns>
    /// <exception cref="Exception"> Индекс выходит за границы. </exception>
    public int GetIndex(int index)
    {
        if (index < 0 || index >= Count)
            throw new Exception("Index not in range");

        return Indices[index];
    }
    
    /// <summary>
    /// Установка новых данных для грани.
    /// </summary>
    /// <param name="count"> Количество вершин. </param>
    /// <param name="indices"> Индексы вершин. </param>
    /// <exception cref="Exception"> Неположительное количетсво вершин или
    /// это количество не совпадает с длиной массива индексов.
    /// </exception>
    public void SetNewFace(int count, int[] indices)
    {
        if (count <= 0 || indices.Length != count)
            throw new Exception("Count not in range or indices.length not equal to count");

        Count = count;
        Indices = (int[]) indices.Clone();
    }
}
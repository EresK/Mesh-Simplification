namespace WindowApplication.Types;

public class ModelInfo
{
    public int VerticesBefore;
    public int VerticesAfter;
    public int FacesBefore;
    public int FacesAfter;

    public ModelInfo()
    {
        VerticesBefore = 0;
        VerticesAfter = 0;
        FacesBefore = 0;
        FacesAfter = 0;
    }

    public ModelInfo(int verticesBefore, int verticesAfter, int facesBefore, int facesAfter)
    {
        VerticesBefore = verticesBefore;
        VerticesAfter = verticesAfter;
        FacesBefore = facesBefore;
        FacesAfter = facesAfter;
    }

    public void Clear()
    {
        VerticesBefore = 0;
        VerticesAfter = 0;
        FacesBefore = 0;
        FacesAfter = 0;
    }

    public float GetVerticesSimplification()
    {
        float percent = VerticesBefore / 100f;
        int difference = VerticesBefore - VerticesAfter;
        if (difference > 0)
            return difference / percent;
        return 0;
    }

    public float GetFacesSimplification()
    {
        float percent = FacesBefore / 100f;
        int difference = FacesBefore - FacesAfter;
        if (difference > 0)
            return difference / percent;
        return 0;
    }
}
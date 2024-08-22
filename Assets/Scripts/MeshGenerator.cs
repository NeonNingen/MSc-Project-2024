using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail) {

        // skipIncr = skipIncrement
        int skipIncr = (levelOfDetail==0)?1:levelOfDetail * 2;
        int numVertsPerLine  = meshSettings.numVertsPerLine;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        MeshData meshData = new MeshData(numVertsPerLine, skipIncr, meshSettings.useFlatShading);

        int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        for (int y = 0; y < numVertsPerLine; y++) {
            for (int x = 0; x < numVertsPerLine; x++) {
                bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3  && y > 2 && y < numVertsPerLine - 3 && ((x-2) % skipIncr != 0 || (y - 2) % skipIncr != 0);

                if (isOutOfMeshVertex) {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                } else if (!isSkippedVertex) {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < numVertsPerLine; y++) {
			for (int x = 0; x < numVertsPerLine; x++) {
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3  && y > 2 && y < numVertsPerLine - 3 && ((x-2) % skipIncr != 0 || (y - 2) % skipIncr != 0);

                if (!isSkippedVertex) {
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncr == 0 && (y - 2) % skipIncr == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndicesMap [x, y];
                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    Vector2 vertexPos2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;
                    float height = heightMap [x, y];

                    if (isEdgeConnectionVertex) {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int dstToMainVertexA = ((isVertical)?y - 2: x - 2) % skipIncr;
                        int dstToMainVertexB = skipIncr - dstToMainVertexA;
                        float dstPercentFromAtoB = dstToMainVertexA / (float)skipIncr;

                        float heightMainVertexA = heightMap[(isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y];
                        float heightMainVertexB = heightMap[(isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y];

                        height = heightMainVertexA * (1 -  dstPercentFromAtoB) + heightMainVertexB * dstPercentFromAtoB;
                    }

                    meshData.AddVertex (new Vector3(vertexPos2D.x, height, vertexPos2D.y), percent, vertexIndex);

                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle) {
                        int currentIncr = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncr : 1;

                        int a = vertexIndicesMap [x, y];
                        int b = vertexIndicesMap [x + currentIncr, y];
                        int c = vertexIndicesMap [x, y + currentIncr];
                        int d = vertexIndicesMap [x + currentIncr, y + currentIncr];
                        meshData.AddTriangle (a,d,c);
                        meshData.AddTriangle (d,a,b);
                    }
                }
            }
		}

		meshData.ProcessMesh ();
		return meshData;
	}
}

public class MeshData {
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTrianglesIndex;

    bool useFlatShading;

    public MeshData(int numVertsPerLine, int skipIncr, bool useFlatShading) {
        this.useFlatShading = useFlatShading;

        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncr - 1) * (numVertsPerLine - 5) / skipIncr * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncr + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
        uvs = new Vector2[vertices.Length];

        int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

        outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
    }

    public void AddVertex(Vector3 vertexPos, Vector2 uv, int vertexIndex) {
        if (vertexIndex < 0) {
            outOfMeshVertices[-vertexIndex - 1] = vertexPos;
        } else {
            vertices[vertexIndex] = vertexPos;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c) {
        if (a < 0 || b < 0 || c < 0) {
            outOfMeshTriangles[outOfMeshTrianglesIndex] = a;
            outOfMeshTriangles[outOfMeshTrianglesIndex+1] = b;
            outOfMeshTriangles[outOfMeshTrianglesIndex+2] = c;
            outOfMeshTrianglesIndex += 3;
        } else {
            triangles[triangleIndex] = a;
            triangles[triangleIndex+1] = b;
            triangles[triangleIndex+2] = c;
            triangleIndex += 3;
        }
    }

    Vector3[] CalculateNormals() {

        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0) vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0) vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0) vertexNormals[vertexIndexC] += triangleNormal;

        }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
        Vector3 pointA = (indexA < 0)?outOfMeshVertices[-indexA-1] : vertices[indexA];
        Vector3 pointB = (indexB < 0)?outOfMeshVertices[-indexB-1] : vertices[indexB];
        Vector3 pointC = (indexC < 0)?outOfMeshVertices[-indexC-1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh() {
        if (useFlatShading) {
            FlatShading();
        } else {
            BakeNormals();
        }
    }

    void BakeNormals() {
        bakedNormals = CalculateNormals();
    }

    void FlatShading() {
        Vector3[] flatShadedVerts = new Vector3[triangles.Length];
        Vector2[] flatShadedUVs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++) {
            flatShadedVerts[i] = vertices[triangles[i]];
            flatShadedUVs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVerts;
        uvs = flatShadedUVs;
    }

    public Mesh CreateMesh()  {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        if (useFlatShading) {
            mesh.RecalculateNormals();
        } else {
            mesh.normals = bakedNormals;
        }
        return mesh;
    }

}
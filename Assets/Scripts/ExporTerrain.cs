// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// C # manual conversion work by Yun Kyu Choi
 
using UnityEngine;
using System;
using System.IO;
using System.Text;
 
enum SaveFormat { Triangles, Quads }
enum SaveResolution { Full=0, Half, Quarter, Eighth, Sixteenth }
 
class ExportTerrain {
   SaveFormat saveFormat = SaveFormat.Triangles;
   SaveResolution saveResolution = SaveResolution.Half;
 
   public TerrainData terrainData;
   public Vector3 terrainPos;
 
   public ExportTerrain(Terrain terrain) {
         terrainData = terrain.terrainData;
         terrainPos = terrain.transform.position;
      }
   
 
   public string Export(string file, float lowestElevation, int resolution, Location location) {

      string fileName = Application.persistentDataPath + $"/TerrainObjects/{file}.obj";
      int w = terrainData.heightmapResolution;
      int h = terrainData.heightmapResolution;
      Vector3 meshScale = terrainData.size;
      int tRes = (int)Mathf.Pow(2, (int)saveResolution );
      meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
      Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
      float[,] tData = terrainData.GetHeights(0, 0, w, h);
 
      w = (w - 1) / tRes + 1;
      h = (h - 1) / tRes + 1;
      Vector3[] tVertices = new Vector3[w * h];
      Vector2[] tUV = new Vector2[w * h];
 
      int[] tPolys;
 
      if (saveFormat == SaveFormat.Triangles)
      {
         tPolys = new int[(w - 1) * (h - 1) * 6];
      }
      else
      {
         tPolys = new int[(w - 1) * (h - 1) * 4];
      }
 
      // Build vertices and UVs
      for (int y = 0; y < h; y++) {
         for (int x = 0; x < w; x++) {
        tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(x, tData[x * tRes, y * tRes], y)) + terrainPos;
        tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
         }
      }

      // Calculate bounds of the mesh
      Bounds meshBounds = new Bounds(tVertices[0], Vector3.zero);
      for (int i = 0; i < tVertices.Length; i++) {
         meshBounds.Encapsulate(tVertices[i]);
      }

      // Adjust the position of each vertex to set the pivot at the bottom along the Y-axis
      float minY = meshBounds.min.y;
      for (int i = 0; i < tVertices.Length; i++) {
         tVertices[i] -= new Vector3(meshBounds.center.x, minY, meshBounds.center.z);
      }
 
      int  index = 0;
      if (saveFormat == SaveFormat.Triangles)
      {
         // Build triangle indices: 3 indices into vertex array for each triangle
         for (int y = 0; y < h - 1; y++)
         {
            for (int x = 0; x < w - 1; x++)
            {
               // For each grid cell output two triangles
               tPolys[index++] = (y * w) + x;
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = (y * w) + x + 1;
 
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = ((y + 1) * w) + x + 1;
               tPolys[index++] = (y * w) + x + 1;
            }
         }
      }
      else
      {
         // Build quad indices: 4 indices into vertex array for each quad
         for (int y = 0; y < h - 1; y++)
         {
            for (int x = 0; x < w - 1; x++)
            {
               // For each grid cell output one quad
               tPolys[index++] = (y * w) + x;
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = ((y + 1) * w) + x + 1;
               tPolys[index++] = (y * w) + x + 1;
            }
         }
      }
 
      // Export to .obj
      StreamWriter sw = new(fileName);
      try
      {
 
         sw.WriteLine("# Unity terrainData OBJ File");
         sw.WriteLine("# Size:" + terrainData.size.x);
         sw.WriteLine("# Highest Elevation:" + terrainData.size.y);
         sw.WriteLine("# Lowest Elevation:" + lowestElevation);
         sw.WriteLine("# Resolution:" + resolution);
         sw.WriteLine("# Location:" + location.lat + "," + location.lng);
         // Write vertices
         System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
         for (int i = 0; i < tVertices.Length; i++)
         {
            StringBuilder sb = new("v ", 20);
            // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
            // Which is important when you're exporting huge terrainDatas.
            sb.Append(tVertices[i].x.ToString()).Append(" ").
               Append(tVertices[i].y.ToString()).Append(" ").
               Append(tVertices[i].z.ToString());
            sw.WriteLine(sb);
         }
         // Write UVs
         for (int i = 0; i < tUV.Length; i++)
         {
            StringBuilder sb = new("vt ", 22);
            sb.Append(tUV[i].x.ToString()).Append(" ").
               Append(tUV[i].y.ToString());
            sw.WriteLine(sb);
         }
         if (saveFormat == SaveFormat.Triangles)
         {
            // Write triangles
            for (int i = 0; i < tPolys.Length; i += 3)
            {
               StringBuilder sb = new("f ", 43);
               sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                  Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                  Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
               sw.WriteLine(sb);
            }
         }
         else
         {
            // Write quads
            for (int i = 0; i < tPolys.Length; i += 4)
            {
               StringBuilder sb = new("f ", 57);
               sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                  Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                  Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                  Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
               sw.WriteLine(sb);
            }
         }
      }
      catch(Exception err)
      {
         Debug.Log("Error saving file: " + err.Message);
      }
      sw.Close();
      terrainData = null;

      return fileName;
   }

}
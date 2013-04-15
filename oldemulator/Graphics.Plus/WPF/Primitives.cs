using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Eurosim.Graphics.WPF
{        //Создаем Meshes
    internal class Primitives
    {

        public static MeshGeometry3D CreateSphereMesh(double Radius, int Slices)
        {
            var mesh = new MeshGeometry3D();
            int Stacks = Slices;
            // Fill the mesh.Positions, mesh.Normals, and mesh.Textures collections.
            for (int stack = 0; stack <= Stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / Stacks;
                double y = Radius * Math.Sin(phi);
                double scale = -Radius * Math.Cos(phi);

                for (int slice = 0; slice <= Slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / Slices;
                    double x = scale * Math.Sin(theta);
                    double z = scale * Math.Cos(theta);

                    Vector3D normal = new Vector3D(x, y, z);
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add((Point3D) normal);
                }
            }

            // Fill the mesh.TriangleIndices collection.
            for (int stack = 0; stack < Stacks; stack++)
            {
                int top = (stack + 0) * (Slices + 1);
                int bot = (stack + 1) * (Slices + 1);

                for (int slice = 0; slice < Slices; slice++)
                {
                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add(top + slice);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(top + slice + 1);
                    }

                    if (stack != Stacks - 1)
                    {
                        mesh.TriangleIndices.Add(top + slice + 1);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(bot + slice + 1);
                    }
                }
            }
            return mesh;
        }


        public static MeshGeometry3D CreateBoxMesh(double xsize, double ysize, double zsize,
                                                    int Slices)
        {
            var vertices = new Point3DCollection();
            var normals = new Vector3DCollection();
            var indices = new Int32Collection();
            var textures = new PointCollection();
            var Stacks = Slices;
            var Layers = Slices;
            double x, y, z;
            var Height = ysize;
            var Width = xsize;
            var Depth = zsize;
            int indexBase = 0;
            var mesh = new MeshGeometry3D();
            //var offsetVector = new Vector3D();



            // Front side.
            // -----------
            z = Depth;

            // Fill the vertices, normals, textures collections.
            for (int stack = 0; stack <= Stacks; stack++)
            {
                y = Height/2 - stack*Height/Stacks;

                for (int slice = 0; slice <= Slices; slice++)
                {
                    x = -Width/2 + slice*Width/Slices;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(x, y, 0)); //???
                    textures.Add(new Point((double) slice/Slices,
                                           (double) stack/Stacks));
                }
            }

            // Fill the indices collection.
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    indices.Add((stack + 0)*(Slices + 1) + slice);
                    indices.Add((stack + 1)*(Slices + 1) + slice);
                    indices.Add((stack + 0)*(Slices + 1) + slice + 1);

                    indices.Add((stack + 0)*(Slices + 1) + slice + 1);
                    indices.Add((stack + 1)*(Slices + 1) + slice);
                    indices.Add((stack + 1)*(Slices + 1) + slice + 1);
                }
            }

            // Rear side.
            // -----------
            indexBase = vertices.Count;
            z = 0;

            // Fill the vertices, normals, textures collections.
            for (int stack = 0; stack <= Stacks; stack++)
            {
                y = Height/2 - stack*Height/Stacks;

                for (int slice = 0; slice <= Slices; slice++)
                {
                    x = Width/2 - slice*Width/Slices;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(x, y, 0));
                    textures.Add(new Point((double) slice/Slices,
                                           (double) stack/Stacks));
                }
            }

            // Fill the indices collection.
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    indices.Add(indexBase + (stack + 0)*(Slices + 1) + slice);
                    indices.Add(indexBase + (stack + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (stack + 0)*(Slices + 1) + slice + 1);

                    indices.Add(indexBase + (stack + 0)*(Slices + 1) + slice + 1);
                    indices.Add(indexBase + (stack + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (stack + 1)*(Slices + 1) + slice + 1);
                }
            }

            // Left side.
            // -----------
            indexBase = vertices.Count;
            x = -Width/2;

            // Fill the vertices, normals, textures collections.
            for (int stack = 0; stack <= Stacks; stack++)
            {
                y = Height/2 - stack*Height/Stacks;

                for (int layer = 0; layer <= Layers; layer++)
                {
                    z = layer*Depth/Layers;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(0, y, z));
                    textures.Add(new Point((double) layer/Layers,
                                           (double) stack/Stacks));
                }
            }

            // Fill the indices collection.
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int layer = 0; layer < Layers; layer++)
                {
                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer + 1);

                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer + 1);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer + 1);
                }
            }

            // Right side.
            // -----------
            indexBase = vertices.Count;
            x = Width/2;

            // Fill the vertices, normals, textures collections.
            for (int stack = 0; stack <= Stacks; stack++)
            {
                y = Height/2 - stack*Height/Stacks;

                for (int layer = 0; layer <= Layers; layer++)
                {
                    z = Depth - layer*Depth/Layers;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(0, y, z));
                    textures.Add(new Point((double) layer/Layers,
                                           (double) stack/Stacks));
                }
            }

            // Fill the indices collection.
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int layer = 0; layer < Layers; layer++)
                {
                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer + 1);

                    indices.Add(indexBase + (stack + 0)*(Layers + 1) + layer + 1);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer);
                    indices.Add(indexBase + (stack + 1)*(Layers + 1) + layer + 1);
                }
            }

            // Top side.
            // -----------
            indexBase = vertices.Count;
            y = Height/2;

            // Fill the vertices, normals, textures collections.
            for (int layer = 0; layer <= Layers; layer++)
            {
                z = layer*Depth/Layers;

                for (int slice = 0; slice <= Slices; slice++)
                {
                    x = -Width/2 + slice*Width/Slices;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(x, 0, z));
                    textures.Add(new Point((double) slice/Slices,
                                           (double) layer/Layers));
                }
            }

            // Fill the indices collection.
            for (int layer = 0; layer < Layers; layer++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice + 1);

                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice + 1);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice + 1);
                }
            }

            // Bottom side.
            // -----------
            indexBase = vertices.Count;
            y = -Height/2;

            // Fill the vertices, normals, textures collections.
            for (int layer = 0; layer <= Layers; layer++)
            {
                z = Depth - layer*Depth/Layers;

                for (int slice = 0; slice <= Slices; slice++)
                {
                    x = -Width/2 + slice*Width/Slices;
                    Point3D point = new Point3D(x, y, z);
                    vertices.Add(point);

                    normals.Add(point - new Point3D(x, 0, z));
                    textures.Add(new Point((double) slice/Slices,
                                           (double) layer/Layers));
                }
            }

            // Fill the indices collection.
            for (int layer = 0; layer < Layers; layer++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice + 1);

                    indices.Add(indexBase + (layer + 0)*(Slices + 1) + slice + 1);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice);
                    indices.Add(indexBase + (layer + 1)*(Slices + 1) + slice + 1);
                }
            }
            mesh.Positions = vertices;
            mesh.TriangleIndices = indices;
            mesh.Freeze();
            return mesh;
        }

        

        public static MeshGeometry3D CreateCylinderMesh(double rbottom, double rtop,
                                                        double Height)
        {
            int ThetaDiv = 32;
            var mesh = new MeshGeometry3D();
            double h = Height/2;
            var pt = new Point3D(0, 0, 2*h);
            var pb = new Point3D(0, 0, 0);
            var pts = new Point3D[ThetaDiv];
            var pbs = new Point3D[ThetaDiv];
            var Center = new Point3D();
            for (int i = 0; i < ThetaDiv; i++)
            {
                pts[i] = GetPosition(rtop, i*360/(ThetaDiv - 1), 2*h, Center);
                pbs[i] = GetPosition(rbottom, i*360/(ThetaDiv - 1), 0, Center);
            }
            //starting
            var startPoints = new Point3DCollection
                                  {
                                      pt,
                                      pts[0],
                                      pts[1],
                                      //top
                                      pb,
                                      pbs[1],
                                      pbs[0],
                                      //bottom
                                      pts[0],
                                      pbs[0],
                                      pbs[1],
                                      pts[1]
                                  }; //side
            var startIndeces = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 6};
            for (int i = 0; i < startPoints.Count; i++)
            {
                mesh.Positions.Add(startPoints[i]);
                //mesh.TriangleIndices.Add(indeces[i]);
            }

            foreach (var idx in startIndeces)
            {
                mesh.TriangleIndices.Add(idx);
            }
            var startTextures = new double[]
                                    {
                                        0, 0, 0, 0.24, 1/ThetaDiv, 0.24,
                                        0, 1, 1/ThetaDiv, 0.76, 0, 0.76,
                                        0, 0.25, 0, 0.75, 1/ThetaDiv, 0.75, 1/ThetaDiv, 0.25
                                    };
            for (int i = 0; i < startTextures.Length; i += 2)
                mesh.TextureCoordinates.Add(new Point(startTextures[i], startTextures[i + 1]));

            int prevtop = 2;
            int prevbot = 4;
            int prevtopside = 9;
            int prevbotside = 8;
            for (int i = 1; i < ThetaDiv - 1; i++)
            {
                int vertcount = 10 + 4*i;
                // Top surface:
                mesh.Positions.Add(pts[i + 1]);
                mesh.TextureCoordinates.Add(new Point(i*(ThetaDiv + 1), 0.24));
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(prevtop);
                mesh.TriangleIndices.Add(vertcount);

                // Bottom surface:
                mesh.Positions.Add(pbs[i + 1]);
                mesh.TextureCoordinates.Add(new Point(i*(ThetaDiv + 1), 0.76));
                mesh.TriangleIndices.Add(3); //center
                mesh.TriangleIndices.Add(vertcount + 1); //latest
                mesh.TriangleIndices.Add(prevbot); //previous

                // Outer surface:
                mesh.Positions.Add(pbs[i + 1]);
                mesh.Positions.Add(pts[i + 1]);
                mesh.TextureCoordinates.Add(new Point(i*(ThetaDiv + 1), 0.75));
                mesh.TextureCoordinates.Add(new Point(i*(ThetaDiv + 1), 0.25));
                mesh.TriangleIndices.Add(prevtopside);
                mesh.TriangleIndices.Add(prevbotside);
                mesh.TriangleIndices.Add(vertcount + 2);
                mesh.TriangleIndices.Add(vertcount + 2);
                mesh.TriangleIndices.Add(vertcount + 3);
                mesh.TriangleIndices.Add(prevtopside);

                prevtop = vertcount;
                prevbot = vertcount + 1;
                prevbotside = vertcount + 2;
                prevtopside = vertcount + 3;
            }
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(ThetaDiv - 1);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3); //center
            mesh.TriangleIndices.Add(5); //latest
            mesh.TriangleIndices.Add(ThetaDiv - 1);


            mesh.TriangleIndices.Add(prevtopside);
            mesh.TriangleIndices.Add(prevbotside);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(prevtop);
            mesh.Freeze();
            return mesh;
        }

        private static Point3D GetPosition(double radius,
                                           double theta, double z, Point3D Center)
        {
            var pt = new Point3D();
            double sn = Math.Sin(theta*Math.PI/180);
            double cn = Math.Cos(theta*Math.PI/180);
            pt.X = -radius*sn;
            pt.Y = radius*cn;
            pt.Z = z;
            pt += (Vector3D) Center;
            return pt;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Eurosim.Graphics.WPF
{
	internal static class Primitives
	{
		public static MeshGeometry3D CreateSphereMesh(double radius, int slices)
		{
			var mesh = new MeshGeometry3D();
			int stacks = slices;
			// Fill the mesh.Positions, mesh.Normals, and mesh.Textures collections.
			for(int stack = 0; stack <= stacks; stack++)
			{
				double phi = Math.PI / 2 - stack * Math.PI / stacks;
				double y = radius * Math.Sin(phi);
				double scale = -radius * Math.Cos(phi);
				for(int slice = 0; slice <= slices; slice++)
				{
					double theta = slice * 2 * Math.PI / slices;
					double x = scale * Math.Sin(theta);
					double z = scale * Math.Cos(theta);
					var normal = new Vector3D(x, y, z);
					mesh.Normals.Add(normal);
					mesh.Positions.Add((Point3D)normal);
				}
			}
			// Fill the mesh.TriangleIndices collection.
			for(int stack = 0; stack < stacks; stack++)
			{
				int top = (stack + 0) * (slices + 1);
				int bot = (stack + 1) * (slices + 1);
				for(int slice = 0; slice < slices; slice++)
				{
					if(stack != 0)
					{
						mesh.TriangleIndices.Add(top + slice);
						mesh.TriangleIndices.Add(bot + slice);
						mesh.TriangleIndices.Add(top + slice + 1);
					}
					if(stack != stacks - 1)
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
		                                           int slices)
		{
			var vertices = new Point3DCollection();
			var normals = new Vector3DCollection();
			var indices = new Int32Collection();
			var textures = new PointCollection();
			int stacks = slices;
			int layers = slices;
			double x, y;
			//var offsetVector = new Vector3D();
			// Front side.
			// -----------
			double z = zsize;
			// Fill the vertices, normals, textures collections.
			for(int stack = 0; stack <= stacks; stack++)
			{
				y = ysize / 2 - stack * ysize / stacks;
				for(int slice = 0; slice <= slices; slice++)
				{
					x = -xsize / 2 + slice * xsize / slices;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(x, y, 0)); //???
					textures.Add(new Point((double)slice / slices,
					                       (double)stack / stacks));
				}
			}
			// Fill the indices collection.
			for(int stack = 0; stack < stacks; stack++)
			{
				for(int slice = 0; slice < slices; slice++)
				{
					indices.Add((stack + 0) * (slices + 1) + slice);
					indices.Add((stack + 1) * (slices + 1) + slice);
					indices.Add((stack + 0) * (slices + 1) + slice + 1);
					indices.Add((stack + 0) * (slices + 1) + slice + 1);
					indices.Add((stack + 1) * (slices + 1) + slice);
					indices.Add((stack + 1) * (slices + 1) + slice + 1);
				}
			}
			// Rear side.
			// -----------
			int indexBase = vertices.Count;
			z = 0;
			// Fill the vertices, normals, textures collections.
			for(int stack = 0; stack <= stacks; stack++)
			{
				y = ysize / 2 - stack * ysize / stacks;
				for(int slice = 0; slice <= slices; slice++)
				{
					x = xsize / 2 - slice * xsize / slices;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(x, y, 0));
					textures.Add(new Point((double)slice / slices,
					                       (double)stack / stacks));
				}
			}
			// Fill the indices collection.
			for(int stack = 0; stack < stacks; stack++)
			{
				for(int slice = 0; slice < slices; slice++)
				{
					indices.Add(indexBase + (stack + 0) * (slices + 1) + slice);
					indices.Add(indexBase + (stack + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (stack + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (stack + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (stack + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (stack + 1) * (slices + 1) + slice + 1);
				}
			}
			// Left side.
			// -----------
			indexBase = vertices.Count;
			x = -xsize / 2;
			// Fill the vertices, normals, textures collections.
			for(int stack = 0; stack <= stacks; stack++)
			{
				y = ysize / 2 - stack * ysize / stacks;
				for(int layer = 0; layer <= layers; layer++)
				{
					z = layer * zsize / layers;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(0, y, z));
					textures.Add(new Point((double)layer / layers,
					                       (double)stack / stacks));
				}
			}
			// Fill the indices collection.
			for(int stack = 0; stack < stacks; stack++)
			{
				for(int layer = 0; layer < layers; layer++)
				{
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer + 1);
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer + 1);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer + 1);
				}
			}
			// Right side.
			// -----------
			indexBase = vertices.Count;
			x = xsize / 2;
			// Fill the vertices, normals, textures collections.
			for(int stack = 0; stack <= stacks; stack++)
			{
				y = ysize / 2 - stack * ysize / stacks;
				for(int layer = 0; layer <= layers; layer++)
				{
					z = zsize - layer * zsize / layers;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(0, y, z));
					textures.Add(new Point((double)layer / layers,
					                       (double)stack / stacks));
				}
			}
			// Fill the indices collection.
			for(int stack = 0; stack < stacks; stack++)
			{
				for(int layer = 0; layer < layers; layer++)
				{
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer + 1);
					indices.Add(indexBase + (stack + 0) * (layers + 1) + layer + 1);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer);
					indices.Add(indexBase + (stack + 1) * (layers + 1) + layer + 1);
				}
			}
			// Top side.
			// -----------
			indexBase = vertices.Count;
			y = ysize / 2;
			// Fill the vertices, normals, textures collections.
			for(int layer = 0; layer <= layers; layer++)
			{
				z = layer * zsize / layers;
				for(int slice = 0; slice <= slices; slice++)
				{
					x = -xsize / 2 + slice * xsize / slices;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(x, 0, z));
					textures.Add(new Point((double)slice / slices,
					                       (double)layer / layers));
				}
			}
			// Fill the indices collection.
			for(int layer = 0; layer < layers; layer++)
			{
				for(int slice = 0; slice < slices; slice++)
				{
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice + 1);
				}
			}
			// Bottom side.
			// -----------
			indexBase = vertices.Count;
			y = -ysize / 2;
			// Fill the vertices, normals, textures collections.
			for(int layer = 0; layer <= layers; layer++)
			{
				z = zsize - layer * zsize / layers;
				for(int slice = 0; slice <= slices; slice++)
				{
					x = -xsize / 2 + slice * xsize / slices;
					var point = new Point3D(x, y, z);
					vertices.Add(point);
					normals.Add(point - new Point3D(x, 0, z));
					textures.Add(new Point((double)slice / slices,
					                       (double)layer / layers));
				}
			}
			// Fill the indices collection.
			for(int layer = 0; layer < layers; layer++)
			{
				for(int slice = 0; slice < slices; slice++)
				{
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (layer + 0) * (slices + 1) + slice + 1);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice);
					indices.Add(indexBase + (layer + 1) * (slices + 1) + slice + 1);
				}
			}
			var mesh = new MeshGeometry3D
			           	{
			           		Positions = vertices,
			           		TriangleIndices = indices,
			           		TextureCoordinates = textures
			           	};
			mesh.Freeze();
			return mesh;
		}

		public static MeshGeometry3D CreateCylinderMesh(double rbottom, double rtop,
		                                                double height)
		{
			const int thetaDiv = 32;
			var pt = new Point3D(0, 0, height);
			var pb = new Point3D(0, 0, 0);
			var topPoints = new Point3D[thetaDiv];
			var bottomPoints = new Point3D[thetaDiv];
			for(int i = 0; i < thetaDiv; i++)
			{
				topPoints[i] = GetCirclePosition(rtop, i * 360.0 / (thetaDiv - 1), height);
				bottomPoints[i] = GetCirclePosition(rbottom, i * 360.0 / (thetaDiv - 1), 0);
			}
			var vertices = new List<Point3D>
			               	{
			               		pt,
			               		topPoints[0],
			               		topPoints[1],
			               		//top
			               		pb,
			               		bottomPoints[1],
			               		bottomPoints[0],
			               		//bottom
			               		topPoints[0],
			               		bottomPoints[0],
			               		bottomPoints[1],
			               		topPoints[1]
			               	};
			var indices = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 6};
			var textureCoordinates = new List<Point>();
			var startTextures = new[]
			                    	{
			                    		0, 0, 0, 0.24, 1.0 / thetaDiv, 0.24,
			                    		0, 1, 1.0 / thetaDiv, 0.76, 0, 0.76,
			                    		0, 0.25, 0, 0.75, 1.0 / thetaDiv, 0.75, 1.0 / thetaDiv, 0.25
			                    	};
			for(int i = 0; i < startTextures.Length; i += 2)
				textureCoordinates.Add(new Point(startTextures[i], startTextures[i + 1]));
			int prevtop = 2;
			int prevbot = 4;
			int prevtopside = 9;
			int prevbotside = 8;
			for(int i = 1; i < thetaDiv - 1; i++)
			{
				int vertcount = 10 + 4 * i;
				// Top surface:
				vertices.Add(topPoints[i + 1]);
				textureCoordinates.Add(new Point(i * (thetaDiv + 1), 0.24));
				indices.Add(0);
				indices.Add(prevtop);
				indices.Add(vertcount);
				// Bottom surface:
				vertices.Add(bottomPoints[i + 1]);
				textureCoordinates.Add(new Point(i * (thetaDiv + 1), 0.76));
				indices.Add(3); //center
				indices.Add(vertcount + 1); //latest
				indices.Add(prevbot); //previous
				// Outer surface:
				vertices.Add(bottomPoints[i + 1]);
				vertices.Add(topPoints[i + 1]);
				textureCoordinates.Add(new Point(i * (thetaDiv + 1), 0.75));
				textureCoordinates.Add(new Point(i * (thetaDiv + 1), 0.25));
				indices.Add(prevtopside);
				indices.Add(prevbotside);
				indices.Add(vertcount + 2);
				indices.Add(vertcount + 2);
				indices.Add(vertcount + 3);
				indices.Add(prevtopside);
				prevtop = vertcount;
				prevbot = vertcount + 1;
				prevbotside = vertcount + 2;
				prevtopside = vertcount + 3;
			}
			indices.Add(0);
			indices.Add(thetaDiv - 1);
			indices.Add(1);
			indices.Add(3); //center
			indices.Add(5); //latest
			indices.Add(thetaDiv - 1);
			indices.Add(prevtopside);
			indices.Add(prevbotside);
			indices.Add(7);
			indices.Add(7);
			indices.Add(6);
			indices.Add(prevtop);
			var mesh = new MeshGeometry3D
			           	{
			           		TextureCoordinates = new PointCollection(textureCoordinates),
			           		Positions = new Point3DCollection(vertices),
			           		TriangleIndices = new Int32Collection(indices),
			           		/*Normals = new Vector3DCollection(normals)*/
			           	};
			mesh.Freeze();
			return mesh;
		}

		private static Point3D GetCirclePosition(double radius,
		                                           double theta, double z)
		{
			double sn = Math.Sin(theta * Math.PI / 180);
			double cn = Math.Cos(theta * Math.PI / 180);
			double x = -radius * sn;
			double y = radius * cn;
			return new Point3D(x, y, z);
		}
	}
}
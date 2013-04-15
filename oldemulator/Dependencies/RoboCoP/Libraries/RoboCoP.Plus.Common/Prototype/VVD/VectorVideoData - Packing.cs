using AIRLab.Thornado;
using System.Collections.Generic;

namespace RoboCoP.Plus.Common {
    partial class VectorVideoData {
        #region Куда-то в торнадо

        int PackObject<T>(T obj, IList<T> list, LogicErrorList errors, string objectName, string collectionName) {
            if(obj == null) return -1;
            var ind = list.IndexOf(obj);
            if(ind == -1)
                errors.Add(LogicErrorLevel.Error, "Объект " + objectName + " не найден в коллекции " + collectionName);
            return ind;

        }

        T UnpackObject<T>(int index, IList<T> list, LogicErrorList errors, string reference, string collectionName) {
            if(index == -1) return default(T);
            if(index < 0 || index >= list.Count)
                errors.Add(LogicErrorLevel.Error, "При восстановлении объекта " + reference + " индекс " + index.ToString() + " не найден в коллекции " + collectionName);
            return list[index];
        }

        int[] PackCollection<T>(IList<T> toPack, IList<T> list, LogicErrorList errors, string objectsName, string collectionName) {
            var result = new int[toPack.Count];
            for(int i = 0; i < result.Length; i++)
                result[i] = PackObject<T>(toPack[i], list, errors, objectsName + "[" + i.ToString() + "]", collectionName);
            return result;
        }

        void UnpackCollection<T>(IList<T> toUnpack, int[] indices, IList<T> list, LogicErrorList errors, string objectsName, string collectionName) {
            if(indices == null) indices = new int[0];
            for(int i = 0; i < indices.Length; i++)
                toUnpack.Add(UnpackObject<T>(indices[i], list, errors, objectsName + "[" + i.ToString() + "]", collectionName));
        }

        #endregion


        void Unpack(LogicErrorList list) {
            for(int i = 0; i < Edges.Count; i++) {
                Edges[i].Point1 = UnpackObject(Edges[i].Point1Index, Vertices, list, "Edge[" + i.ToString() + "].Point1", "Vertices");
                Edges[i].Point2 = UnpackObject(Edges[i].Point2Index, Vertices, list, "Edge[" + i.ToString() + "].Point2", "Vertices");
            }
            for(int i = 0; i < Bodies.Count; i++) {
                UnpackCollection(Bodies[i].Vertices, Bodies[i].PointsIndices, Vertices, list, "Bodies[" + i.ToString() + "].Vertices", "Vertices");
                UnpackCollection(Bodies[i].Edges, Bodies[i].EdgesIndices, Edges, list, "Bodies[" + i.ToString() + "].Edges", "Edges");
            }
        }

        void Pack(LogicErrorList list) {
            for(int i = 0; i < Edges.Count; i++) {
                Edges[i].Point1Index = PackObject(Edges[i].Point1, Vertices, list, "Edge[" + i.ToString() + "].Point1", "Vertices");
                Edges[i].Point2Index = PackObject(Edges[i].Point2, Vertices, list, "Edge[" + i.ToString() + "].Point2", "Vertices");
            }
            for(int i = 0; i < Bodies.Count; i++) {
                Bodies[i].PointsIndices = PackCollection(Bodies[i].Vertices, Vertices, list, "Bodies[" + i.ToString() + "].Vertices", "Vertices");
                Bodies[i].EdgesIndices = PackCollection(Bodies[i].Edges, Edges, list, "Bodies[" + i.ToString() + "].Edges", "Edges");
            }
        }


        public void CheckSelfConsistancy(LogicErrorList list) {
            if(list.Type == LogicErrorType.AfterIO)
                Unpack(list);
            if(list.Type == LogicErrorType.BeforeIO)
                Pack(list);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
   
      

    public class TensorResolver<T,A> : TypeResolver
    {
        bool correspondsToArray;
        int correspondingArrayRank;
        int intermediateRank;

        public TensorResolver()
        {
            if (TensorIntermediateType<T>.IsOrdinal(typeof(A)))
            {
                correspondsToArray = false;
                intermediateRank = TensorIntermediateType<T>.GetOrdinalIndex(typeof(A));
            }
            else
            {
                correspondsToArray = true;
                correspondingArrayRank = typeof(A).GetArrayRank();
            }
        }

   
        public override object Touch(object obj, FieldAddress tail)
        {
            if (!(obj is Array)) return null;
            var array = obj as Array;
            var desired = tail.Elements.Take(array.Rank).Select(z => Formats.Int.Parse(z)).ToArray();
            var actual = Enumerable.Range(0, array.Rank).Select(a => array.GetLength(a)).ToArray();
            bool make = false;
            for (int i = 0; i < desired.Length; i++)
                if (actual[i] <= desired[i])
                {
                    actual[i] = desired[i] + 1;
                    make = true;
                }
            if (!make) return null;

            var newArray = Array.CreateInstance(typeof(T), actual);
            var indexes = GetIndexes(Enumerable.Range(0, array.Rank).Select(a => array.GetLength(a)).ToArray()).ToList();
            foreach (var i in indexes)
                newArray.SetValue(array.GetValue(i.ToArray()), i.ToArray());
            return newArray;
        }

        private static IEnumerable<List<int>> GetIndexes(params int[] Length)
        {
            if (Length.Length == 1)
                for (int i = 0; i < Length[0]; ++i)
                    yield return new List<int>() { i };
            else
            {
                for (int i = 0; i < Length[0]; ++i)
                {
                    int[] arr2 = new int[Length.Length - 1];
                    for (int j = 1; j < Length.Length; ++j)
                        arr2[j - 1] = Length[j];
                    foreach (var add in GetIndexes(arr2))
                    {
                        var lst = new List<int>() { i };
                        lst.AddRange(add);
                        yield return lst;
                    }
                }
            }
        }

        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            Array array = null;
            int rank = -1;

            if (correspondsToArray)
            {
                array = (Array)obj;
                rank = 0;
            }
            else
            {
                var tit = (TensorIntermediateType<T>.Base)obj;
                array = tit.Array;
                rank = tit.ptr;
            }

            foreach (var str in Enumerable.Range(0, array.GetLength(rank)))
                yield return str.ToString();

        }

        public override object GetElement(object obj, string sub)
        {
            if (correspondsToArray)
            {
                if (correspondingArrayRank == 1)
                    return ((Array)obj).GetValue(int.Parse(sub));
                else
                {
                    var ord = TensorIntermediateType<T>.CreateOrdinal(correspondingArrayRank - 1);
                    ord.Array = (Array)obj;
                    ord.args = new int[ord.Array.GetType().GetArrayRank()];
                    ord.ptr = 1;
                    ord.args[0] = int.Parse(sub);
                    return ord;
                }
            }
            else
            {
                var tit=(TensorIntermediateType<T>.Base)obj;
                tit.args[tit.ptr] = int.Parse(sub);
                if (intermediateRank == 1)
                    return tit.Array.GetValue(tit.args);
                else
                    return TensorIntermediateType<T>.NextOrdinal(tit, int.Parse(sub));
            }
        }


        public override void SetElement(object obj, string sub, object value)
        {
            if (correspondsToArray)
            {
                if (correspondingArrayRank == 1)
                {
                    ((Array)obj).SetValue(value, int.Parse(sub));
                    return;
                }
            }
            else
            {
                if (intermediateRank == 1)
                {
                    var tit = (TensorIntermediateType<T>.Base)obj;
                    tit.args[tit.ptr] = int.Parse(sub);
                    tit.Array.SetValue(value, tit.args);
                    return;
                }
            }
            throw new Exception("Cannot set intermediate level of tensor");
        }

      
        public override IEnumerable<string> GetDefinedSubaddresses()
        {
            yield return "*";
        }

        public override Type GetDefinedType(string sub)
        {
            if (correspondsToArray)
            {
                if (correspondingArrayRank == 1) return typeof(T);
                else return TensorIntermediateType<T>.GetOrdinalType(correspondingArrayRank - 1);
            }
            else
            {
                if (intermediateRank == 1) return typeof(T);
                else return TensorIntermediateType<T>.GetOrdinalType(intermediateRank - 1);
            }
        }

        void LinearizeMold(List<Tuple<int[], TextMold>> result, List<int> indexes, TextMold mold, int left, LogicErrorList list, ContextDependedParser cdp)
        {
            if (left == 0) result.Add(new Tuple<int[], TextMold>(indexes.ToArray(), mold));
            else
            {
                if (cdp != null)
                    cdp(mold, TensorIntermediateType<T>.GetOrdinalType(left), list);
                foreach (var e in mold.Nodes.Keys)
                {
                    indexes.Add(int.Parse(e));
                    LinearizeMold(result, indexes, mold.Nodes[e], left - 1, list, cdp);
                    indexes.RemoveAt(indexes.Count - 1);
                }
            }
        }
            

        
        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            if (!correspondsToArray) throw new Exception("Cannot parse mold in intermediate tensor");
            var lin=new List<Tuple<int[],TextMold>>();
            LinearizeMold(lin, new List<int>(), mold, correspondingArrayRank, errors, cdp);
            var maxRanks = new int[correspondingArrayRank];
            foreach (var e in lin)
                for (int i = 0; i < maxRanks.Length; i++)
                    if (e.Item1[i] >= maxRanks[i]) maxRanks[i] = e.Item1[i]+1;
            var array = Array.CreateInstance(typeof(T), maxRanks);
            foreach (var e in lin)
                array.SetValue(ParseMold(typeof(T), e.Item2, errors, cdp), e.Item1);
            return array;


        }
    }
}

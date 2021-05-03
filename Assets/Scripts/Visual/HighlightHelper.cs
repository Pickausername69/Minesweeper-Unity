using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class HighlightHelper
{
    /// <summary>
    /// Key: new elements, Value: removed elements
    /// </summary>
    public static Vector2Int[] GetNewTilesSafe(List<Vector2Int> originals, Vector2Int[] updaters)
    {
        NativeHashMap<int2, int> o = new NativeHashMap<int2, int>(originals.Count, Allocator.TempJob);
        for (int i = 0; i < originals.Count; i++)
        {
            o.Add(new int2(originals[i].x, originals[i].y), 0);
        }


        NativeArray<int2> u = new NativeArray<int2>(updaters.Length, Allocator.TempJob);
        for (int i = 0; i < updaters.Length; i++)
        {
            u[i] = new int2(updaters[i].x, updaters[i].y);
        }

        NativeArray<int> cindex = new NativeArray<int>(updaters.Length, Allocator.TempJob);
        NativeArray<int2> na = new NativeArray<int2>(updaters.Length ,Allocator.TempJob);
        SafeDistinctNoRemove j = new SafeDistinctNoRemove
        {
            cindex=cindex,
            original = o,
            updated = u,
            newupdated = na
        };
        JobHandle jobhandle=j.Schedule(updaters.Length, updaters.Length / 16);
        jobhandle.Complete();


        List<Vector2Int> newtiles = new List<Vector2Int>();
        int2 current;
        for (int i = 0; i < cindex.Length; i++)
        {
            if (cindex[i]==1)
            {
                current = na[i];
                newtiles.Add(new Vector2Int(current.x, current.y));
            }
        }
        cindex.Dispose(jobhandle);
        o.Dispose(jobhandle);
        u.Dispose(jobhandle);
        na.Dispose(jobhandle);
        //j.newupdated.Dispose();
        //j.original.Dispose();
        //j.updated.Dispose();

        return newtiles.ToArray();
    }

    /// <summary>
    /// get new elements from invalid
    /// (too slow to write to nativa array)
    /// </summary>
    /// <param name="originals"></param>
    /// <param name="updaters"></param>
    /// <param name="rowlength"></param>
    /// <param name="jobnumber"></param>
    /// <returns></returns>
    public static Vector2Int[] GetNewTilesSameOrder(List<Vector2Int> originals, Vector2Int[] updaters, int rowlength, int jobnumber=8)
    {
        if (updaters.Length < 8) { jobnumber = 1; }
        NativeArray<int2> o = new NativeArray<int2>(originals.Count, Allocator.TempJob);
        for (int i = 0; i < originals.Count; i++)
        {
            o[i] = new int2(originals[i].x, originals[i].y);
        }
        NativeArray<int2> u = new NativeArray<int2>(updaters.Length, Allocator.TempJob);
        for (int i = 0; i < updaters.Length; i++)
        {
            u[i] = new int2(updaters[i].x, updaters[i].y);
        }

        NativeList<int2>[] result = new NativeList<int2>[jobnumber];
        NativeArray<JobHandle> jobhandles = new NativeArray<JobHandle>(jobnumber, Allocator.Temp);
        DistinctWhenOrderedByAscAndNoRemove[] j = new DistinctWhenOrderedByAscAndNoRemove[jobnumber];

        int singlelength = updaters.Length / jobnumber;
        int lastlength = updaters.Length % jobnumber;
        for (int i = 0; i < jobnumber - 1; i++)
        {
            result[i] = new NativeList<int2>(Allocator.TempJob);
            j[i]=new DistinctWhenOrderedByAscAndNoRemove
            {
            maxindex = singlelength * (i + 1),
            rowlength = rowlength,
            oindex = singlelength * i,
            uindex = singlelength * i,
            result = result[i],
            original = o,
            updated = u       
            };
            jobhandles[i]=j[i].Schedule();
        }
        //last index
        result[jobnumber - 1] = new NativeList<int2>(Allocator.TempJob);
        j[jobnumber - 1] = new DistinctWhenOrderedByAscAndNoRemove
        {
            maxindex = updaters.Length,
            rowlength = rowlength,
            oindex = singlelength * (jobnumber - 1),
            uindex = singlelength * (jobnumber - 1),
            result = result[jobnumber - 1],
            original = o,
            updated = u
        };
        jobhandles[jobnumber - 1]=j[jobnumber - 1].Schedule();
        JobHandle.CompleteAll(jobhandles);

        List<Vector2Int> newtiles = new List<Vector2Int>();
        for (int i = 0; i < jobnumber; i++)
        {
            for (int k = 0; k < result[i].Length; k++)
            {
                
                int2 current = result[i][k];
                newtiles.Add(new Vector2Int(current.x, current.y));
            }
        }

        //dispose
        o.Dispose();
        u.Dispose();
        for (int i = 0; i < jobnumber; i++)
        {
            result[i].Dispose();
        }

        return newtiles.ToArray();
    }
    [BurstCompile]
    private struct SafeDistinctNoRemove : IJobParallelFor
    {
        public NativeArray<int> cindex;
        [ReadOnly] public NativeHashMap<int2,int> original;
        [ReadOnly]public NativeArray<int2> updated;
        public NativeArray<int2> newupdated;
        public void Execute(int index)
        {
            if (original.ContainsKey(updated[index]))
            {
                //original.Remove(updated[index]);
            }
            else
            {
                //Debug.Log(index);
                newupdated[index]=updated[index];
                cindex[index]++;
            }
        }
    }
    [BurstCompile]
    private struct DistinctWhenOrderedByAscAndNoRemove : IJob
    {
        [ReadOnly]public int maxindex;
        [ReadOnly]public int rowlength;
        public int oindex;
        public int uindex;
        public NativeList<int2> result;

        [ReadOnly] public NativeArray<int2> original;
        [ReadOnly] public NativeArray<int2> updated;

        public void Execute()
        {
            int maxo = original.Length;
            int2 currentu;
            int2 currento;
            while (maxindex > uindex)
            {
                currentu = updated[uindex];
                if (oindex >= maxo)
                {
                    break;
                }
                currento = original[oindex];


                int value = (currentu.x + currentu.y * rowlength) - (currento.x + currento.y * rowlength);
                //original and updated are the same
                if (value == 0)
                {
                    oindex++;
                    uindex++;
                }
                //when can't find th current updated
                else if (value >= 0)
                {
                    result.Add(currentu);
                    oindex++;
                }
                else
                {
                    uindex++;
                }
            }
        }
    }

    [BurstCompile]
    public struct Additive : IJob
    {
        public NativeArray<int2> original;
        public NativeArray<int2> updated;

        public NativeList<int2> result;
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

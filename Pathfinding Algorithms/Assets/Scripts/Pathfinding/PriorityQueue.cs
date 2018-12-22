using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// PriorityQueue class uses object type T that implements the IComparable interface
public class PriorityQueue<T> where T:IComparable<T>  {

    List<T> data;
    public int Count { get {  return data.Count; }}

    /// <summary>
    /// Constructor
    /// </summary>
    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    /// <summary>
    /// Add an item to the queue
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(T item)
    {
        data.Add(item);

        int childIndex = data.Count - 1;

        while(childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if(data[childIndex].CompareTo(data[parentIndex]) >= 0)
            {
                break;
            }

            T tmp = data[childIndex];
            data[childIndex] = data[parentIndex];
            data[parentIndex] = tmp;

            childIndex = parentIndex;
        }
    }

    /// <summary>
    /// Get the first item in the queue and then re-order the queue
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        int lastIndex = data.Count - 1;
        int parentIndex = 0;

        T frontItem = data[0];

        // re-order
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);
        lastIndex--;

        while (true)
        {
            int childIndex = parentIndex * 2 + 1;
            if(childIndex > lastIndex)
            {
                break;
            }

            int rightChild = childIndex + 1;

            if(rightChild <= lastIndex && data[rightChild].CompareTo(data[childIndex]) < 0)
            {
                childIndex = rightChild;
            }

            if(data[parentIndex].CompareTo(data[childIndex]) <= 0)
            {
                break;
            }

            // swap
            T tmp = data[parentIndex];
            data[parentIndex] = data[childIndex];
            data[childIndex] = tmp;

            parentIndex = childIndex;
        } 

        return frontItem;
    }

    /// <summary>
    /// return the next item in the queue
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        T frontItem = data[0];
        return frontItem;
    }

    /// <summary>
    /// Returns whether the data contains the item passed
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return data.Contains(item);
    }

    /// <summary>
    /// Return data as a list
    /// </summary>
    /// <returns></returns>
    public List<T> ToList()
    {
        return data;
    }

}

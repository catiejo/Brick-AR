using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//used under the No Fucking License License from:
//http://stackoverflow.com/questions/6464531/the-most-efficient-implementation-of-uniquequeue-and-uniquereplacementqueue-coll
public class UniqueQueue<T> : IEnumerable<T> {
	private HashSet<T> hashSet;
	private Queue<T> queue;


	public UniqueQueue() {
		hashSet = new HashSet<T>();
		queue = new Queue<T>();
	}


	public int Count {
		get {
			return queue.Count;
		}
	}

	public bool isEmpty() {
		return Count == 0;
	}

	public void Clear() {
		hashSet.Clear();
		queue.Clear();
	}


	public bool Contains(T item) {
		return hashSet.Contains(item);
	}

	public void Enqueue(T item) {
		if (hashSet.Add(item)) {
			queue.Enqueue(item);
		}
	}

	public T Dequeue() {
		T item = queue.Dequeue();
		hashSet.Remove(item);
		return item;
	}


	public T Peek() {
		return queue.Peek();
	}


	public IEnumerator<T> GetEnumerator() {
		return queue.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		return queue.GetEnumerator();
	}
}
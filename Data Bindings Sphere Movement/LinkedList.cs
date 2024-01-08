using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    public class LinkedList<T>
    {
        private int length = 0;

        private LinkedListNode<T> head = null;

        public void Add(T item)
        {
            LinkedListNode<T> node = new LinkedListNode<T>(item);

            if(head == null)
            {
                head = node;
            }
            else
            {
                LinkedListNode<T> lastNode = FindAtIndex(length - 1);
                lastNode.Next = node;
                node.Previous = lastNode;
            }

            length++;
        }

        public T RemoveAtIndex(int index)
        {
            T item = default;

            if(head != null)
            {
                LinkedListNode<T> node = FindAtIndex(index);
                item = node.Data;
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;

                length--;
            }

            return item;
        }

        public T[] AllData()
        {
            T[] data = new T[length];
            LinkedListNode<T> current;

            if (head != null)
            {
                current = head;
                for (int i = 0; i < length; i++)
                {
                    data[i] = current.Data;
                    current = current.Next;
                }
            }
            else
            {
                data = null;
            }

            return data;
        }

        private LinkedListNode<T> FindAtIndex(int index)
        {
            LinkedListNode<T> node = null;
            LinkedListNode<T> current = head;

            if (index < length)
            {
                for(int i = 0;i< index; i++)
                {
                    current = current.Next;
                }
                node = current;
            }

            return node;
        }

        public T FindDataAtIndex(int index)
        {
            T data = default;
            if(index < length)
            {
                data = FindAtIndex(index).Data;
            }

            return data;
        }

        public int Length
        {
            get { return length; }
        }


    }
}

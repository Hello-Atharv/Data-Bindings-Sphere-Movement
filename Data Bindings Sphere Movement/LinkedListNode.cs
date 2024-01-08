using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    public class LinkedListNode<T>
    {
        private LinkedListNode<T> next = null;
        private LinkedListNode<T> prev = null;
        private T data;

        public LinkedListNode(T data)
        {
            this.data = data;
        }

        public LinkedListNode<T> Next
        {
            get { return next; }
            set { next = value; }
        }

        public LinkedListNode<T> Previous
        {
            get { return prev; }
            set { prev = value; }
        }

        public T Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}

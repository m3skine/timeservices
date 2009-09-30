namespace Instinct_
{
    /// <summary>
    /// LinkChain
    /// </summary>
    public class LinkChain<T>
        where T : LinkNode<T>
    {
        public int Count;
        public T Head;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkChain&lt;T&gt;"/> class.
        /// </summary>
        public LinkChain()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkChain&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="head">The head.</param>
        public LinkChain(T value)
        {
            Count = 1;
            Head = value;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="value">The value.</param>
        public void AddFirst(T value)
        {
            var head = Head;
            if ((value.NextLink != null) || (head == value))
            {
                //+ paranoia
                throw new System.InvalidOperationException();
            }
            Count++;
            value.NextLink = head;
            Head = value;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Head = null;
        }

        //    /// <summary>
        //    /// Merges the specified change chain.
        //    /// </summary>
        //    /// <param name="changeChain">The change chain.</param>
        //    public void Merge(LinkChain<T> destination)
        //    {
        //        destination.Count += Count;
        //        Head = destination.Head;
        //        AddFirst(destination.Head);
        //        Clear();
        //    }
    }
}
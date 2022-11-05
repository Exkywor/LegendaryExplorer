using System.Collections;
using System.Collections.Generic;

namespace LegendaryExplorerCore.Unreal.BinaryConverters
{
    public class TreeNode<TData, TChild> : IEnumerable<TChild>
    {
        public readonly TData Data;
        public readonly List<TChild> Children;

        public TreeNode(TData data)
        {
            Data = data;
            Children = new List<TChild>();
        }

        public void Add(TChild item) => Children.Add(item);

        public IEnumerator<TChild> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Deconstruct(out TData data, out List<TChild> children)
        {
            data = Data;
            children = Children;
        }
    }
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public readonly T Data;
        public readonly List<TreeNode<T>> Children;

        public TreeNode(T data)
        {
            Data = data;
            Children = new List<TreeNode<T>>();
        }

        public void Add(TreeNode<T> item) => Children.Add(item);

        public IEnumerator<TreeNode<T>> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

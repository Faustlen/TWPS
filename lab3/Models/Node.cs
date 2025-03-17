using System.Collections.Generic;

namespace lab3.Models
{
    public class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();

        public Node(string name)
        {
            Name = name;
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
        }
    }
}
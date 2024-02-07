using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth_2
{
    public class FPNode
    {
        public string Item { get; set; }
        public int Count { get; set; }
        public FPNode Parent { get; set; }
        public Dictionary<string, FPNode> Children { get; set; }

        public FPNode(string item, int count, FPNode parent)
        {
            Item = item;
            Count = count;
            Parent = parent;
            Children = new Dictionary<string, FPNode>();
        }
    }

    public class FPGrowth
    {
        private Dictionary<string, FPNode> headerTable;

        public List<HashSet<string>> FindFrequentPatterns(List<HashSet<string>> transactions, int minSupport)
        {
            var root = BuildTree(transactions, minSupport);
            var frequentPatterns = new List<HashSet<string>>();
            MineTree(headerTable, minSupport, new HashSet<string>(), frequentPatterns, root);
            return frequentPatterns;
        }

        private FPNode BuildTree(List<HashSet<string>> transactions, int minSupport)
        {
            headerTable = new Dictionary<string, FPNode>();
            foreach (var transaction in transactions)
            {
                foreach (var item in transaction)
                {
                    if (headerTable.ContainsKey(item))
                    {
                        headerTable[item].Count++;
                    }
                    else
                    {
                        headerTable[item] = new FPNode(item, 1, null);
                    }
                }
            }

            headerTable = headerTable
                .Where(kv => kv.Value.Count >= minSupport)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            if (headerTable.Count == 0)
            {
                return null;
            }

            var root = new FPNode(null, 0, null);

            foreach (var transaction in transactions)
            {
                var filteredTransaction = transaction
                    .Where(item => headerTable.ContainsKey(item))
                    .OrderByDescending(item => headerTable[item].Count)
                    .ToList();
                InsertTree(filteredTransaction, root, headerTable);
            }

            return root;
        }

        private void InsertTree(List<string> transaction, FPNode node, Dictionary<string, FPNode> headerTable)
        {
            if (transaction.Count > 0)
            {
                var item = transaction[0];
                if (node.Children.ContainsKey(item))
                {
                    var child = node.Children[item];
                }
                else
                {
                    var child = new FPNode(item, 0, node);
                    node.Children[item] = child;
                    UpdateHeaderTable(item, child, headerTable);
                }

                node.Children[item].Count += 1;
                InsertTree(transaction.GetRange(1, transaction.Count - 1), node.Children[item], headerTable);
            }
        }

        private void UpdateHeaderTable(string item, FPNode node, Dictionary<string, FPNode> headerTable)
        {
            if (headerTable[item].Count == 0)
            {
                headerTable[item].Count = node.Count;
            }
            else
            {
                var currentNode = node;
                while (currentNode != null)
                {
                    currentNode = currentNode.Parent;
                }
            }
        }

        private void MineTree(Dictionary<string, FPNode> headerTable, int minSupport, HashSet<string> prefix, List<HashSet<string>> frequentPatterns, FPNode node)
        {
            foreach (var itemNode in headerTable.OrderByDescending(kv => kv.Value.Count))
            {
                var item = itemNode.Key;
                var newPrefix = new HashSet<string>(prefix);
                newPrefix.Add(item);
                frequentPatterns.Add(newPrefix);

                var conditionalTree = GetConditionalTree(item, headerTable, node);
                if (conditionalTree != null)
                {
                    var conditionalHeaderTable = new Dictionary<string, FPNode>();
                    foreach (var transaction in conditionalTree)
                    {
                        foreach (var i in transaction)
                        {
                            if (headerTable.ContainsKey(i))
                            {
                                conditionalHeaderTable[i] = headerTable[i];
                            }
                        }
                    }

                    conditionalHeaderTable = conditionalHeaderTable
                        .Where(kv => kv.Value.Count >= minSupport)
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                    if (conditionalHeaderTable.Count > 0)
                    {
                        MineTree(conditionalHeaderTable, minSupport, newPrefix, frequentPatterns, node);
                    }
                }
            }
        }

        private List<List<string>> GetConditionalTree(string item, Dictionary<string, FPNode> headerTable, FPNode node)
        {
            var current = headerTable.ContainsKey(item) ? headerTable[item] : null;
            var conditionalTree = new List<List<string>>();

            while (current != null)
            {
                var path = new List<string>();
                var currentNode = current.Parent;
                while (currentNode != null && currentNode.Item != null)
                {
                    path.Add(currentNode.Item);
                    currentNode = currentNode.Parent;
                }

                if (path.Count > 0)
                {
                    conditionalTree.Add(path);
                }

                current = current.Parent;
            }

            return conditionalTree;
        }
    }

    class Program
    {
        static void Main()
        {
            List<HashSet<string>> transactions = new List<HashSet<string>>
        {
            new HashSet<string> { "a", "b", "c" },
            new HashSet<string> { "a", "f", "g","h" },
            new HashSet<string> { "a", "b", "h", "e" },
            new HashSet<string> { "a", "b", "d" },
            new HashSet<string> { "d", "i", "k" },
            new HashSet<string> { "f", "g" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "d", "i", "k" },
            new HashSet<string> { "f", "g" },
            new HashSet<string> { "d", "i", "k" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "d", "i", "k" },
            new HashSet<string> { "f", "g" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "d", "i", "k" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
            new HashSet<string> { "a", "b", "c","d" },
        };

            int minSupport = 5;

            FPGrowth fpGrowth = new FPGrowth();
            List<HashSet<string>> frequentPatterns = fpGrowth.FindFrequentPatterns(transactions, minSupport);

            Console.WriteLine("Frequent Patterns:");
            foreach (var pattern in frequentPatterns)
            {
                Console.WriteLine(string.Join(", ", pattern));
            }
            Console.ReadLine();
        }
    }
}

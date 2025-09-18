namespace Minesweeper
{
    public class SmarterPermutationBuilderBoardSolver : BaseBoardSolver
    {
        private uint BreakEarlyLogicChain = uint.MaxValue;
        private uint MaxMergablePermutationCount = uint.MaxValue;
        public SmarterPermutationBuilderBoardSolver() : base() { }
        public SmarterPermutationBuilderBoardSolver(Board board, bool verboseLogging = false) : base(board, verboseLogging) { }
        public override IBoardSolver Construct(Board board, bool verboseLogging = false) => new SmarterPermutationBuilderBoardSolver(board, verboseLogging);
        public void SetBreakEarlyLogicChain(uint value)
        {
            BreakEarlyLogicChain = value;
        }
        public void SetMaxMergablePermutationCount(uint value)
        {
            MaxMergablePermutationCount = value;
        }
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            yield return PermutationBuildingAlgorithm;
        }
        private bool PermutationBuildingAlgorithm()
        {
            var time = DateTime.Now;
            if (!ActiveNumbers.Any())
            {
                return false;
            }
            //var regionsDict = new Dictionary<(int x, int y), LogicGraphNode>();
            Dictionary<(int x, int y), List<((int x, int y) pos, uint connectivity)>> Edges = new Dictionary<(int x, int y), List<((int x, int y) pos, uint connectivity)>>();
            foreach (var pos in ActiveNumbers)
            {
                var edges = ConnectedNumbersWithConnectivity(pos);
                Edges[pos] = new List<((int x, int y) pos, uint connectivity)>();
                foreach (var edge in edges)
                {
                    Edges[pos].Add(edge);
                }
            }
            var numbersVisited = new HashSet<(int x, int y)>();
            var numbersInLogic = new List<(int x, int y)>();
            var startNumber = ActiveNumbers.First();
            var front = new List<(int x, int y)> { startNumber };
            var number = front.Last();
            front.RemoveAt(front.Count - 1);
            numbersVisited.Add(number);
//            Edges[number].ForEach(i=>i.connectivity > 1 && )
            //BFS or DFS
            //todo


            /*
            var logicRegion = new List<LogicGraphNode>();
            var regionsDict = new Dictionary<(int x, int y), LogicGraphNode>();
            foreach (var pos in ActiveNumbers)
            {
                var reg = new LogicGraphNode(pos);
                regionsDict[pos] = reg;
                logicRegion.Add(reg);
            }
            foreach (var reg in regionsDict)
            {
                var connectedNumbers = ConnectedNumbersWithConnectivity(reg.Key);
                foreach (var edge in connectedNumbers.Where(i => ActiveNumbers.Contains(i.pos)))
                {
                    reg.Value.ConnectedNodes.Add((edge.connectivity, regionsDict[edge.pos]));
                }
            }
            bool mergedSth = true;
            while (mergedSth)
            {
                mergedSth = false;
                for (int i = logicRegion.Count - 1; i >= 0; i--)
                {
                    var reg = logicRegion[i];
                    var reg2 = reg.ConnectedNodes.Find(i => i.connectivity > 1).node;
                    if (reg2 == null) { continue; }
                    mergedSth = true;
                    logicRegion.RemoveAt(i);
                    logicRegion.Remove(reg2);
                    logicRegion.Add(LogicGraphNode.Merge(reg, reg2));
                    break;
                }
            }
            Console.WriteLine($"Time: {(DateTime.Now - time).TotalMilliseconds}ms");
            var regions = new List<MineRegionPermutationNode>();
            {
                var regDict = new Dictionary<(int x, int y), MineRegionPermutationNode>();
                foreach (var lReg in logicRegion)
                {
                    foreach (var number in lReg.Numbers)
                    {
                        var mrpn = new MineRegionPermutationNode(MineRegionPermutationFromNumber(number));
                        regions.Add(mrpn);
                        regDict.Add(number, mrpn);
                    }
                    foreach (var pos in regDict.Keys)
                    {
                        foreach (var pos2 in ConnectedNumbers(pos).Where(i => regDict.ContainsKey(i)))
                        {
                            regDict[pos].ConnectedNodes.Add(regDict[pos2]);
                        }
                    }
                }
            }*/
            var regions = new List<MineRegionPermutationNode>();
            {
                var regionsDict = new Dictionary<(int x, int y), MineRegionPermutationNode>();
                foreach (var pos in ActiveNumbers)
                {
                    var reg = new MineRegionPermutationNode(MineRegionPermutationFromNumber(pos));
                    regionsDict[pos] = reg;
                    regions.Add(reg);
                }
                foreach (var reg in regionsDict)
                {
                    var connectedNumbers = ConnectedNumbersWithConnectivity(reg.Key).Where(i => i.connectivity > 1).Select(i => i.pos);
                    foreach (var edge in connectedNumbers.Where(i => ActiveNumbers.Contains(i)))
                    {
                        reg.Value.ConnectedNodes.Add(regionsDict[edge]);
                    }
                }
            }
            uint minRegionNumberCount = 1;
            while (minRegionNumberCount < BreakEarlyLogicChain && regions.Where(i => i.ConnectedNodes.Any()).Where(i => i.MineRegionPermutation.PermutationCount < MaxMergablePermutationCount).Where(i => i.ConnectedNodes.Any(j => j.MineRegionPermutation.PermutationCount < MaxMergablePermutationCount)).Any())
            {
                var nodeInd = regions.FindIndex(i => i.ConnectedNodes.Any(j => j.MineRegionPermutation.PermutationCount < MaxMergablePermutationCount) && i.NumbersCombined == minRegionNumberCount);
                if (nodeInd == -1)
                {
                    if (VerboseLogging)
                    {
                        Console.WriteLine($"All regions have more than {minRegionNumberCount} numbers, checking higher number. total regions: {regions.Count}");
                    }
                    minRegionNumberCount++;
                    continue;
                }
                var node = regions[nodeInd];
                regions.RemoveAt(nodeInd);
                node.ConnectedNodes.Sort((a, b) => a.NumbersCombined.CompareTo(b.NumbersCombined));
                var otherNode = node.ConnectedNodes.First(i => i.MineRegionPermutation.PermutationCount < MaxMergablePermutationCount);
                regions.Remove(otherNode);
                var newMineRegionPermutation = node.MineRegionPermutation.Intersection(otherNode.MineRegionPermutation);
                var newConnectedNodes = node.ConnectedNodes.Union(otherNode.ConnectedNodes).Except(new[] { node, otherNode });
                bool foundNewInfo = false;
                foreach (var info in newMineRegionPermutation.GetInformation())
                {
                    if (info.mine)
                    {
                        if (!IsSetMine(info.pos))
                        {
                            foundNewInfo = true;
                            SetMine(info.pos);
                        }
                    }
                    else
                    {
                        foundNewInfo = true;
                        ClickSquare(info.pos);
                    }
                }
                if (foundNewInfo)
                {
                    if (VerboseLogging)
                    {
                        Console.WriteLine($"Current logic had new information, breaking early. Had a total of {newMineRegionPermutation.Squares.Count()} numbers in logic.");
                        PrintCurrentStateBoard();
                    }
                    return true;
                }
                var newNode = new MineRegionPermutationNode(newMineRegionPermutation);
                node.PointerToItself = newNode;
                otherNode.PointerToItself = newNode;
                newNode.ConnectedNodes = newConnectedNodes.ToList();
                regions.Add(newNode);
                regions.ForEach(i => i.CheckConnectedNodesForUpdates());
            }
            return false;
        }
        private EfficientMineRegionPermutation MineRegionPermutationFromNumber((int x, int y) pos)
        {
            if (DiscoveredNumbers[pos.x, pos.y] == UndiscoveredNumber)
            {
                throw new Exception($"Called {nameof(MineRegionPermutationFromNumber)} in {nameof(EfficientSmartPermutationBuilderBoardSolver)} with a unopened square, which isn't allowed.");
            }
            var totalNeighbors = Board.GetNeighbors(pos).ToList();
            uint mines = (uint)DiscoveredNumbers[pos.x, pos.y] - (uint)totalNeighbors.Count(i => IsSetMine(i));
            var neighbors = totalNeighbors.Where(i => !IsSetMine(i) && !IsOpenedSquare(i)).ToList();
            return new EfficientMineRegionPermutation(mines, neighbors, VerboseLogging);
        }
        private class MineRegionPermutationNode
        {
            public List<MineRegionPermutationNode> ConnectedNodes = new List<MineRegionPermutationNode>();
            public EfficientMineRegionPermutation MineRegionPermutation;
            public MineRegionPermutationNode PointerToItself; //set this to the new node, when merging two nodes, such that references to this node can resolve the new merged node.
            public uint NumbersCombined => (uint)MineRegionPermutation.Squares.Count();
            public MineRegionPermutationNode(EfficientMineRegionPermutation mineRegionPermutation)
            {
                MineRegionPermutation = mineRegionPermutation;
                PointerToItself = this;
            }
            public void CheckConnectedNodesForUpdates()
            {
                ConnectedNodes = ConnectedNodes.Select(i => i.PointerToItself).ToList();
            }
        }
        private class LogicGraphNode
        {
            public HashSet<(int x, int y)> Numbers = new HashSet<(int x, int y)>();
            public HashSet<(uint connectivity, LogicGraphNode node)> ConnectedNodes = new HashSet<(uint connectivity, LogicGraphNode node)>();
            public LogicGraphNode PointerToItself; //set this to the new node, when merging two nodes, such that references to this node can resolve the new merged node.
            public LogicGraphNode() { PointerToItself = this; }
            public LogicGraphNode((int x, int y) pos)
            {
                Numbers.Add(pos);
                PointerToItself = this;
            }
            public void CheckConnectedNodesForUpdates()
            {
                ConnectedNodes = ConnectedNodes.Select(i => (i.connectivity, i.node.PointerToItself)).ToHashSet();
            }
            public static LogicGraphNode Merge(LogicGraphNode n1, LogicGraphNode n2)
            {
                var result = new LogicGraphNode();
                result.Numbers = n1.Numbers.Union(n2.Numbers).ToHashSet();
                result.ConnectedNodes = n1.ConnectedNodes.Union(n2.ConnectedNodes).Where(i => i.node != n1 && i.node != n2).ToHashSet();
                //todo combine edges with connectivity c1 & c2 of same other node => c1 + c2
                n1.PointerToItself = result;
                n2.PointerToItself = result;
                n1.ConnectedNodes.Foreach(i => i.node.CheckConnectedNodesForUpdates());
                n2.ConnectedNodes.Foreach(i => i.node.CheckConnectedNodesForUpdates());
                return result;
            }
        }
    }
}
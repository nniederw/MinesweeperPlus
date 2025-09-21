namespace Minesweeper
{
    public class SmarterPermutationBuilderBoardSolver : BaseBoardSolver
    {
        private uint BreakEarlyLogicChain = uint.MaxValue;
        private uint MaxMergablePermutationCount = uint.MaxValue;
        public SmarterPermutationBuilderBoardSolver() : base() { }
        public SmarterPermutationBuilderBoardSolver(IBoard board, bool verboseLogging = false) : base(board, verboseLogging) { }
        //public override IBoardSolver Construct(IBoard board, bool verboseLogging = false) => new SmarterPermutationBuilderBoardSolver(board, verboseLogging);
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
        private IEnumerable<MineRegionPermutationNode> GetRegionsFromActiveNumbers()
            => ActiveNumbers.Select(i => MineRegionPNodeFromNumber(i));
        private MineRegionPermutationNode MineRegionPNodeFromNumber((int x, int y) pos)
        {
            return new MineRegionPermutationNode(MineRegionPermutationFromNumber(pos), ConnectedNumbersWithConnectivity(pos));
        }
        private Dictionary<(int x, int y), MineRegionPermutationNode> RegionsDictionary = new Dictionary<(int x, int y), MineRegionPermutationNode>();
        private bool IsBuiltRegion((int x, int y) pos) => RegionsDictionary.ContainsKey(pos);
        private bool PermutationBuildingAlgorithm()
        {
            var time = DateTime.Now;
            if (!ActiveNumbers.Any())
            {
                return false;
            }
            RegionsDictionary.Clear();
            RegionsDictionary.EnsureCapacity(ActiveNumbers.Count);
            foreach (var mrpn in GetRegionsFromActiveNumbers())
            {
                var pos = mrpn.MineRegionPermutation.Numbers.Single(); ; //can only contain 1 number
                RegionsDictionary.Add(pos, mrpn);
                foreach (var edge in mrpn.ConnectedNodes.Where(i => IsBuiltRegion(i.pos)))
                {
                    if (edge.connectivity >= 2)
                    {
                        var other = RegionsDictionary[edge.pos];
                        var merged = mrpn.MergeWith(other);
                        RegionsDictionary[edge.pos] = merged;
                        RegionsDictionary[pos] = merged;
                        var infos = merged.MineRegionPermutation.GetInformation();
                        if (infos.Any())
                        {
                            foreach (var info in infos)
                            {
                                if (info.mine)
                                {
                                    SetMine(info.pos);
                                }
                                else
                                {
                                    ClickSquare(info.pos);
                                }
                            }
                            return true;
                        }
                        break;
                    }
                }
            }

            //var regionsDict = new Dictionary<(int x, int y), LogicGraphNode>();
            Dictionary<(int x, int y), List<((int x, int y) pos, uint connectivity)>> Edges = new Dictionary<(int x, int y), List<((int x, int y) pos, uint connectivity)>>();
            foreach (var pos in ActiveNumbers)
            {
                var edges = ConnectedNumbersWithConnectivity(pos);
                Edges[pos] = new List<((int x, int y) pos, uint connectivity)>();
                foreach (var edge in edges.Where(i => ActiveNumbers.Contains(i.pos)))
                {
                    Edges[pos].Add(edge);
                }
            }
            var logicGraphNodeLookup = new Dictionary<(int x, int y), LogicGraphNode>();
            var numbersNotVisited = ActiveNumbers.Select(i => new LogicGraphNode(i)).ToHashSet();
            foreach (var lgn in numbersNotVisited)
            {
                logicGraphNodeLookup.Add(lgn.Numbers.First(), lgn);
            }
            foreach (var i in numbersNotVisited)
            {
                if (Edges.ContainsKey(i.Numbers.First()))
                {
                    foreach (var edge in Edges[i.Numbers.First()])
                    {
                        i.ConnectedNodes.Add((edge.connectivity, logicGraphNodeLookup[edge.pos]));
                    }
                }
            }
            //todo, change this shit to lazy evaluation
            bool mergedSomething = true;
            while (mergedSomething)
            {
                mergedSomething = false;
                var numbersVisited = new HashSet<LogicGraphNode>();
                var weakEdgesOut = new HashSet<LogicGraphNode>();

                var newLogicNodes = new List<LogicGraphNode>();
                while (numbersNotVisited.Count() > 1)
                {
                    var startNumber = numbersNotVisited.First();
                    var front = new HashSet<LogicGraphNode> { startNumber };
                    var numbersInLogic = new List<(int x, int y)>();
                    var nodesInLogic = new List<LogicGraphNode>();
                    while (front.Any())
                    {
                        var number = front.First();
                        front.Remove(number);
                        number.Numbers.ForEach(i => numbersInLogic.Add(i));
                        nodesInLogic.Add(number);
                        numbersVisited.Add(number);
                        numbersNotVisited.Remove(number);
                        number.ConnectedNodes.ForEach(
                            i =>
                            {
                                if (!numbersVisited.Contains(i.node))
                                {
                                    if (i.connectivity > 1)
                                    {
                                        front.Add(i.node);
                                    }
                                    else
                                    {
                                        if (weakEdgesOut.Contains(i.node))
                                        {
                                            weakEdgesOut.Remove(i.node);
                                            front.Add(i.node);
                                        }
                                        else
                                        {
                                            weakEdgesOut.Add(i.node);
                                        }
                                    }
                                }
                            });
                        if (weakEdgesOut.Contains(number))
                        {
                            weakEdgesOut.Remove(number);
                        }
                    }
                    if (nodesInLogic.Count > 1)
                    {
                        mergedSomething = true;
                        var newNode = new LogicGraphNode();
                        newNode.Numbers = numbersInLogic;
                        nodesInLogic.ForEach(i => i.PointerToItself = newNode);
                        newNode.ConnectedNodes = weakEdgesOut.Select(i => ((uint)1, i)).ToList();
                        newLogicNodes.Add(newNode);
                    }
                }
                numbersNotVisited = newLogicNodes.Select(i => i.PointerToItself).ToHashSet();
                numbersNotVisited.Foreach(i => i.CheckConnectedNodesForUpdates());
            }
            var regionsToExplore = numbersNotVisited.ToList();
            foreach (var pos in logicGraphNodeLookup.Keys)
            {
                var lgn = logicGraphNodeLookup[pos];
                logicGraphNodeLookup[pos] = lgn.GetCurrentVersion();
            }
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
                    var lgn = logicGraphNodeLookup[reg.Key];
                    foreach (var edge in Edges[reg.Key])
                    {
                        if (logicGraphNodeLookup[edge.pos] == lgn)
                        {
                            reg.Value.ConnectedNodes.Add(regionsDict[edge.pos]);
                        }
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
                        Console.WriteLine($"Current logic had new information, breaking early. Had a total of {newMineRegionPermutation.Numbers.Count()} numbers in logic.");
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
            public List<((int x, int y) pos, uint connectivity)> ConnectedNodes = new List<((int x, int y) pos, uint connectivity)>();
            public EfficientMineRegionPermutation MineRegionPermutation;
            public uint NumbersCombined => (uint)MineRegionPermutation.Numbers.Count();
            public MineRegionPermutationNode(EfficientMineRegionPermutation mineRegionPermutation)
            {
                MineRegionPermutation = mineRegionPermutation;
            }
            public MineRegionPermutationNode(EfficientMineRegionPermutation mineRegionPermutation, IEnumerable<((int x, int y) pos, uint connectivity)> connectedNodes)
            {
                MineRegionPermutation = mineRegionPermutation;
                ConnectedNodes = connectedNodes.ToList();
            }
            public MineRegionPermutationNode MergeWith(MineRegionPermutationNode other)
                => Merge(this, other);
            public static MineRegionPermutationNode Merge(MineRegionPermutationNode n1, MineRegionPermutationNode n2)
            {
                var perm = n1.MineRegionPermutation.Intersection(n2.MineRegionPermutation);
                var numbers = perm.Numbers.ToHashSet();
                var newEdgesHash = new Dictionary<(int x, int y), uint>(); // pos, connectivity
                foreach (var edge in n1.ConnectedNodes.Union(n2.ConnectedNodes).Where(i => !numbers.Contains(i.pos)))
                {
                    if (newEdgesHash.ContainsKey(edge.pos))
                    {
                        newEdgesHash[edge.pos] += edge.connectivity;
                        continue;
                    }
                    newEdgesHash.Add(edge.pos, edge.connectivity);
                }
                return new MineRegionPermutationNode(perm, newEdgesHash.AsEnumerable().Select(i => (i.Key, i.Value)));
            }
        }
        private class LogicGraphNode
        {
            public List<(int x, int y)> Numbers = new List<(int x, int y)>();
            public List<(uint connectivity, LogicGraphNode node)> ConnectedNodes = new List<(uint connectivity, LogicGraphNode node)>();
            public LogicGraphNode PointerToItself; //set this to the new node, when merging two nodes, such that references to this node can resolve the new merged node.
            public LogicGraphNode() { PointerToItself = this; }
            public LogicGraphNode((int x, int y) pos)
            {
                Numbers.Add(pos);
                PointerToItself = this;
            }
            public void CheckConnectedNodesForUpdates()
            {
                for (int i = 0; i < ConnectedNodes.Count; i++)
                {
                    var node = ConnectedNodes[i];
                    var pti = node.node.GetCurrentVersion();
                    if (pti != node.node)
                    {
                        ConnectedNodes[i] = (node.connectivity, pti);
                    }
                }
            }
            public LogicGraphNode GetCurrentVersion()
            {
                var pti = PointerToItself;
                while (pti != pti.PointerToItself)
                {
                    pti = PointerToItself;
                }
                return pti;
            }
            public static LogicGraphNode Merge(LogicGraphNode n1, LogicGraphNode n2)
            {
                var result = new LogicGraphNode();
                result.Numbers = n1.Numbers.Union(n2.Numbers).ToList();
                result.ConnectedNodes = n1.ConnectedNodes.Union(n2.ConnectedNodes).Where(i => i.node != n1 && i.node != n2).ToList();
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
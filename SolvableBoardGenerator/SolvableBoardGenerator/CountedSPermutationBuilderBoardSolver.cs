namespace Minesweeper
{
    public class CountedSPermutationBuilderBoardSolver : BaseBoardSolver
    {
        private Dictionary<(int x, int y), MineRegionPermutationNode> RegionsDictionary = new Dictionary<(int x, int y), MineRegionPermutationNode>();
        private HashSet<MineRegionPermutationNode> LastActivePermutationNodes = new HashSet<MineRegionPermutationNode>();
        private List<List<MineRegionPermutationNode>> LastActiveNodesGroups = new List<List<MineRegionPermutationNode>>();
        public CountedSPermutationBuilderBoardSolver() : base() { }
        public CountedSPermutationBuilderBoardSolver(IBoard board, bool verboseLogging = false) : base(board, verboseLogging) { }
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            LastActiveNodesGroups.Clear();
            LastActivePermutationNodes.Clear();
            yield return PermutationBuildingAlgorithm;
            yield return MineCountTester;
        }
        private IEnumerable<MineRegionPermutationNode> GetRegionsFromActiveNumbers()
            => ActiveNumbers.Select(i => MineRegionPNodeFromNumber(i));
        private MineRegionPermutationNode MineRegionPNodeFromNumber((int x, int y) pos)
        {
            return new MineRegionPermutationNode(MineRegionPermutationFromNumber(pos), pos.AsSingleEnumerable(), ConnectedActiveNonMineNumbers(pos));
        }
        private bool IsBuiltRegion((int x, int y) pos) => RegionsDictionary.ContainsKey(pos);
        private void UseInfo(IEnumerable<((int x, int y) pos, bool mine)> infos)
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
        }
        private MineRegionPermutationNode MergeRegions(MineRegionPermutationNode mreg1, MineRegionPermutationNode mreg2)
        {
            var merged = mreg1.MergeWith(mreg2);
            foreach (var pos in mreg1.NumbersInLogic.Union(mreg2.NumbersInLogic))
            {
                RegionsDictionary[pos] = merged;
            }
            return merged;
        }
        private MineRegionPermutationNode MergeRegions(IEnumerable<MineRegionPermutationNode> regions)
        {
            MineRegionPermutationNode? mrpn = null;
            foreach (var region in regions)
            {
                if (mrpn == null)
                {
                    mrpn = region;
                    continue;
                }
                mrpn = MergeRegions(mrpn, region);
            }
            if (mrpn == null)
            {
                throw new Exception($"{nameof(MergeRegions)} was called with an empty {nameof(IEnumerable<MineRegionPermutationNode>)}, which isn't allowed.");
            }
            return mrpn;
        }
        private bool TryGettingInformation(MineRegionPermutationNode mrpn) => TryGettingInformation(mrpn.MineRegionPermutation);
        private bool TryGettingInformation(CountedEfficientMineRegionPermutation mrpn)
        {
            var infos = mrpn.GetInformation();
            if (infos.Any())
            {
                UseInfo(infos);
                if (VerboseLogging)
                {
                    Console.WriteLine($"Current logic had new information, breaking early. Had a total of {infos.Count()} PBF.");
                    PrintCurrentStateBoard();
                }
                return true;
            }
            return false;
        }
        private IEnumerable<MineRegionPermutationNode> OutEdges(MineRegionPermutationNode mrpn)
        {
            return mrpn.ConnectedNodes.Select(i => RegionsDictionary[i.pos]).Distinct();
        }
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
                var pos = mrpn.NumbersInLogic.Single(); ; //can only contain 1 number
                RegionsDictionary.Add(pos, mrpn);
                foreach (var edge in mrpn.ConnectedNodes.Where(i => IsBuiltRegion(i.pos)))
                {
                    if (edge.connectedSquares.Count >= 2)
                    {
                        var other = RegionsDictionary[edge.pos];
                        var merged = MergeRegions(mrpn, other);
                        if (TryGettingInformation(merged))
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            var activeRegions = RegionsDictionary.Values.ToHashSet();
            var inactiveRegions = new HashSet<MineRegionPermutationNode>();
            bool mergedSomething = true;
            while (mergedSomething)
            {
                mergedSomething = false;
                var toAdd = new List<MineRegionPermutationNode>();
                var toRemove = new List<MineRegionPermutationNode>();
                foreach (var mrpn in activeRegions)
                {
                    if (!mrpn.ConnectedNodes.Any() || mrpn.ConnectedNodes.Count == 1 && mrpn.ConnectedNodes.Single().connectedSquares.Count <= 1)
                    {
                        activeRegions.Remove(mrpn);
                        inactiveRegions.Add(mrpn);
                        continue;
                    }
                    var mergableInd = mrpn.ConnectedNodes.FindIndex(i => i.connectedSquares.Count >= 2);
                    if (mergableInd != -1)
                    {
                        var edge = mrpn.ConnectedNodes[mergableInd];
                        if (!RegionsDictionary.ContainsKey(edge.pos))
                        {

                        }
                        var other = RegionsDictionary[edge.pos];
                        var merged = MergeRegions(mrpn, other);
                        toAdd.Add(merged);
                        toRemove.Add(mrpn);
                        toRemove.Add(other);
                        if (TryGettingInformation(merged))
                        {
                            return true;
                        }
                        mergedSomething = true;
                        break;

                    }
                    //all connected nodes have connectivity 1
                    Dictionary<MineRegionPermutationNode, (int x, int y)> weakEdges = new Dictionary<MineRegionPermutationNode, (int x, int y)>();
                    MineRegionPermutationNode? toMerge = null;
                    foreach (var edge in mrpn.ConnectedNodes)
                    {
                        var other = RegionsDictionary[edge.pos];
                        if (weakEdges.ContainsKey(other))
                        {
                            if (weakEdges[other] != edge.connectedSquares.Single())
                            {
                                toMerge = other;
                                break;
                            }
                            continue;
                        }
                        weakEdges.Add(other, edge.connectedSquares.Single());
                    }
                    if (toMerge != null)
                    {
                        var merged = MergeRegions(mrpn, toMerge);
                        toAdd.Add(merged);
                        toRemove.Add(mrpn);
                        toRemove.Add(toMerge);
                        if (TryGettingInformation(merged))
                        {
                            return true;
                        }
                        mergedSomething = true;
                        break;
                    }
                }
                activeRegions.RemoveRange(toRemove);
                activeRegions.AddRange(toAdd);
            }
            LastActivePermutationNodes = activeRegions.Union(inactiveRegions).ToHashSet();

            //resolve cycles in logic
            var groups = new List<List<MineRegionPermutationNode>>();
            {
                var nodes = activeRegions.Union(inactiveRegions).ToHashSet();
                foreach (var reg in activeRegions.Union(inactiveRegions))
                {
                    if (nodes.Contains(reg))
                    {
                        nodes.Remove(reg);
                        if (reg.ConnectedNodes.Count == 0)
                        {
                            groups.Add(new List<MineRegionPermutationNode> { reg });
                            continue;
                        }
                        var group = new List<MineRegionPermutationNode>();
                        group.Add(reg);
                        Queue<(int x, int y)> outEdges = new Queue<(int x, int y)>();
                        outEdges.EnqueueRange(reg.ConnectedNodes.Select(i => i.pos).Where(i => nodes.Contains(RegionsDictionary[i])));
                        while (outEdges.Any())
                        {
                            var current = RegionsDictionary[outEdges.Dequeue()];
                            if (nodes.Contains(current))
                            {
                                nodes.Remove(current);
                                group.Add(current);
                                outEdges.EnqueueRange(current.ConnectedNodes.Select(i => i.pos).Where(i => nodes.Contains(RegionsDictionary[i])));
                            }
                        }
                        groups.Add(group);
                    }
                }
            }
            foreach (var group in groups)
            {
                if (group.Count > 2) //cyclic logic of size 2 should have been resolved before.
                {
                    var graph = new List<(MineRegionPermutationNode mrpn, List<MineRegionPermutationNode> edges)>();
                    graph = group.Select(i => (i, OutEdges(i).ToList())).ToList();
                    var cycleElements = GetCycleElements(graph).ToList();
                    if (cycleElements.Any())
                    {
                        if (VerboseLogging)
                        {
                            Console.WriteLine($"aw shit, we gotta merge {cycleElements.Count} logics just because they form a cycle (potential {cycleElements.Product(i => i.MineRegionPermutation.PermutationCount)} permutations).");
                        }
                        var merged = MergeRegions(cycleElements);
                        if (TryGettingInformation(merged))
                        {
                            return true;
                        }
                        LastActiveNodesGroups.Add(group.Except(cycleElements).Prepend(merged).ToList());
                        continue;
                    }
                    LastActiveNodesGroups.Add(group);
                }
                else
                {
                    LastActiveNodesGroups.Add(group);
                }
            }
            /*foreach (var reg in inactiveRegions)
            {
                LastActiveNodesGroups.Add(new List<MineRegionPermutationNode>() { reg });
            }*/
            return false;
        }
        private bool MineCountTester()
        {
            var FloatingSquares = Board.AllSquares().Where(i => !IsOpenedSquare(i) && !IsSetMine(i) && !ActiveUnopenedSquares.Contains(i)).ToList();
            uint FloatingSquaresCount = (uint)FloatingSquares.Count;
            uint trivialMin = 0; //lower bound of edge minecount
            uint trivialMax = 0;//upper bound of edge minecount
            var groups = LastActiveNodesGroups;
            if (groups.Count == 0)
            {
                if (MineCount == 0 && FloatingSquares.Any())
                {
                    foreach (var pos in FloatingSquares)
                    {
                        ClickSquare(pos);
                    }
                    return true;
                }
                return false;
            }
            foreach (var tbounds in groups.Select(i => CalculateTrivialCounts(i)))
            {
                trivialMin += tbounds.trivialMinMineCount;
                trivialMax += tbounds.trivialMaxMineCount;
            }
            if (MineCount > trivialMax)
            {
                return false; //trivial Max is an upper bound of edge mines
            }
            if (MineCount < trivialMin)
            {
                throw new Exception($"{nameof(MineCountTester)} counted a lower bound for edge mines that is higher than the current mine count. Something must truly be off.");
            }
            if (MineCount == trivialMin)
            {
                if (FloatingSquaresCount > 0)
                {
                    foreach (var pos in FloatingSquares)
                    {
                        ClickSquare(pos);
                    }
                    return true;
                }
            }
            uint minEdgeCount = 0;
            uint maxEdgeCount = 0;
            var countedGroups = groups.Select(i => CalculatePreciseCounts(i)).ToList();
            foreach (var group in countedGroups)
            {
                minEdgeCount += group.minMineCount;
                maxEdgeCount += group.maxMineCount;
            }
            if (MineCount > maxEdgeCount)
            {
                return false;
            }
            if (MineCount < minEdgeCount)
            {
                throw new Exception($"{nameof(MineCountTester)} counted a lower bound for edge mines that is higher than the current mine count. Something must truly be off.");
            }
            if (MineCount == minEdgeCount && FloatingSquares.Any())
            {
                foreach (var pos in FloatingSquares)
                {
                    ClickSquare(pos);
                }
                return true;
            }
            var validRestCount = MineCount;
            var restGroups = new List<(uint minMineCount, uint maxMineCount, CountedEfficientMineRegionPermutation mrpn)>();
            foreach (var group in countedGroups)
            {
                if (group.minMineCount == group.maxMineCount)
                {
                    validRestCount -= group.minMineCount;
                    continue;
                }
                restGroups.Add(group);
            }
            if (!restGroups.Any())
            {
                return false; //todo check if this right, should have all case covered where this can give information.
            }
            if (restGroups.Count == 1)
            {
                var node = restGroups.Single();
                var newReg = RestrictMineCountTo(node.mrpn, validRestCount);
                return TryGettingInformation(newReg);
            }
            // if (VerboseLogging)
            {
                Console.WriteLine("aw shit, mine count is really complicated, stopping here.");
            }
            //todo
            return false;
        }
        private (uint trivialMinMineCount, uint trivialMaxMineCount) CalculateTrivialCounts(List<MineRegionPermutationNode> acyclicGroup)
        {
            if (acyclicGroup.Count == 1)
            {
                var n = acyclicGroup.Single().MineRegionPermutation;
                return (n.MinMineCount, n.MaxMineCount);
            }
            uint trivialMinMineCount, trivialMaxMineCount;
            trivialMinMineCount = 0;
            trivialMaxMineCount = 0;
            HashSet<MineRegionPermutationNode> processedNodes = new HashSet<MineRegionPermutationNode>();
            foreach (var node in GetTreeFromAcyclicGraph(acyclicGroup.First()))
            {
                processedNodes.Add(node);
                HashSet<(int x, int y)> alreadyProcessedSquares = new HashSet<(int x, int y)>();
                var edges = OutEdges(node).Where(i => processedNodes.Contains(i)).ToList();
                foreach (var edge in edges)
                {
                    var squares = edge.MineRegionPermutation.SquaresInPermutation.Intersect(node.MineRegionPermutation.SquaresInPermutation);
                    alreadyProcessedSquares.AddRange(squares);
                }
                int sharedSquares = alreadyProcessedSquares.Count;
                trivialMinMineCount += (uint)Math.Max(0, node.MineRegionPermutation.MinMineCount - sharedSquares);
                trivialMaxMineCount += (node.MineRegionPermutation.MaxMineCount + (uint)sharedSquares);
            }
            return (trivialMinMineCount, trivialMaxMineCount);
        }
        private (uint minMineCount, uint maxMineCount, CountedEfficientMineRegionPermutation mrpn) CalculatePreciseCounts(List<MineRegionPermutationNode> acyclicGroup)
        {
            if (acyclicGroup.Count == 1)
            {
                var n = acyclicGroup.Single().MineRegionPermutation;
                return (n.MinMineCount, n.MaxMineCount, n);
            }
            var merged = MergeRegions(acyclicGroup);
            //todo optimize
            return (merged.MineRegionPermutation.MinMineCount, merged.MineRegionPermutation.MaxMineCount, merged.MineRegionPermutation);
        }
        private IEnumerable<MineRegionPermutationNode> GetTreeFromAcyclicGraph(MineRegionPermutationNode start, HashSet<MineRegionPermutationNode> visitedRegs = null)
        {
            if (visitedRegs == null)
            {
                visitedRegs = new HashSet<MineRegionPermutationNode>();
            }
            visitedRegs.Add(start);
            yield return start;
            foreach (var edge in start.ConnectedNodes)
            {
                var reg = RegionsDictionary[edge.pos];
                if (!visitedRegs.Contains(reg))
                {
                    foreach (var result in GetTreeFromAcyclicGraph(reg, visitedRegs))
                    {
                        yield return result;
                    }
                }
            }
        }
        private CountedEfficientMineRegionPermutation RestrictMineCountToMax(CountedEfficientMineRegionPermutation cemrp, uint mineCout)
        {
            if (cemrp.MinMineCount > mineCout || mineCout > cemrp.MaxMineCount)
            {
                throw new Exception($"Called {nameof(RestrictMineCountTo)} with invalid mine count: {mineCout}");
            }
            var validPermutations = cemrp.AllPermutations().Where(i => i.MineCount == mineCout).Select(i => (i.Permutation, i.MineCount));
            var indexlut = cemrp.AllPermutations().First().IndexLookupTable;
            return new CountedEfficientMineRegionPermutation(validPermutations, indexlut, cemrp.VerboseLogging);
        }
        private CountedEfficientMineRegionPermutation MineRegionPermutationFromNumber((int x, int y) pos)
        {
            if (DiscoveredNumbers[pos.x, pos.y] == UndiscoveredNumber)
            {
                throw new Exception($"Called {nameof(MineRegionPermutationFromNumber)} in {nameof(EfficientSmartPermutationBuilderBoardSolver)} with a unopened square, which isn't allowed.");
            }
            var totalNeighbors = Board.GetNeighbors(pos).ToList();
            uint mines = (uint)DiscoveredNumbers[pos.x, pos.y] - (uint)totalNeighbors.Count(i => IsSetMine(i));
            var neighbors = totalNeighbors.Where(i => !IsSetMine(i) && !IsOpenedSquare(i)).ToList();
            return new CountedEfficientMineRegionPermutation(mines, neighbors, VerboseLogging);
        }
        private class MineRegionPermutationNode
        {
            public List<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> ConnectedNodes = new List<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectivity)>();
            public IReadOnlyCollection<(int x, int y)> NumbersInLogic = new List<(int x, int y)>();
            public CountedEfficientMineRegionPermutation MineRegionPermutation;
            public MineRegionPermutationNode(CountedEfficientMineRegionPermutation mineRegionPermutation, IEnumerable<(int x, int y)> numbersInLogic)
            {
                MineRegionPermutation = mineRegionPermutation;
                NumbersInLogic = numbersInLogic.ToList();
            }
            public MineRegionPermutationNode(CountedEfficientMineRegionPermutation mineRegionPermutation, IEnumerable<(int x, int y)> numbersInLogic, IEnumerable<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> connectedNodes)
            {
                MineRegionPermutation = mineRegionPermutation;
                NumbersInLogic = numbersInLogic.ToList();
                ConnectedNodes = connectedNodes.ToList();
            }
            public MineRegionPermutationNode MergeWith(MineRegionPermutationNode other)
                => Merge(this, other);
            public static MineRegionPermutationNode Merge(MineRegionPermutationNode n1, MineRegionPermutationNode n2)
            {
                var perm = n1.MineRegionPermutation.Intersection(n2.MineRegionPermutation);
                var numbers = n1.NumbersInLogic.Union(n2.NumbersInLogic).ToHashSet();
                var newEdgesHash = new Dictionary<(int x, int y), List<(int x, int y)>>(); // pos, connectivity
                foreach (var edge in n1.ConnectedNodes.Union(n2.ConnectedNodes).Where(i => !numbers.Contains(i.pos)))
                {
                    if (newEdgesHash.ContainsKey(edge.pos))
                    {
                        newEdgesHash[edge.pos].AddRange(edge.connectedSquares);
                        continue;
                    }
                    newEdgesHash.Add(edge.pos, edge.connectedSquares.ToList());
                }
                return new MineRegionPermutationNode(perm, numbers, newEdgesHash.Select(i => (i.Key, (IReadOnlyList<(int x, int y)>)i.Value.Distinct().ToList())));
            }
        }
    }
}
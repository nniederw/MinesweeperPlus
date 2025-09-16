namespace Minesweeper
{
    public class EfficientSmartPermutationBuilderBoardSolver : BaseBoardSolver
    {
        private uint BreakEarlyLogicChain = uint.MaxValue;
        private uint MaxMergablePermutationCount = uint.MaxValue;
        public EfficientSmartPermutationBuilderBoardSolver(Board board, bool verboseLogging = false) : base(board, verboseLogging) { }
        public override IBoardSolver Construct(Board board, bool verboseLogging = false) => new EfficientSmartPermutationBuilderBoardSolver(board, verboseLogging);
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
            if (!ActiveNumbers.Any())
            {
                return false;
            }
            var regions = new List<MineRegionPermutationNode>(); //todo maybe figure out a better datastructure, but elements should remain very few regardless.
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
                    var connectedNumbers = ConnectedNumbers(reg.Key);
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
    }
}
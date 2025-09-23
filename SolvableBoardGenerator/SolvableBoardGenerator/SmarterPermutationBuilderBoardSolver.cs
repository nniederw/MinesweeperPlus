namespace Minesweeper
{
    public class SmarterPermutationBuilderBoardSolver : BaseBoardSolver
    {
        private Dictionary<(int x, int y), MineRegionPermutationNode> RegionsDictionary = new Dictionary<(int x, int y), MineRegionPermutationNode>();
        public SmarterPermutationBuilderBoardSolver() : base() { }
        public SmarterPermutationBuilderBoardSolver(IBoard board, bool verboseLogging = false) : base(board, verboseLogging) { }
        protected override IEnumerable<Func<bool>> PhaseSequence()
        {
            yield return TestPS1;
            yield return PermutationBuildingAlgorithm;
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
        private bool TryGettingInformation(MineRegionPermutationNode mrpn)
        {
            var infos = mrpn.MineRegionPermutation.GetInformation();
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
            //todo: consider loops of A - B - C, {AB,BC,AC} each sharing one square
            if (activeRegions.Count(i => i.ConnectedNodes.Count >= 2) >= 3)
            {
                Console.WriteLine($"Current execution of {nameof(SmarterPermutationBuilderBoardSolver)} might still be solvable, even though it will return false");
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
            public List<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> ConnectedNodes = new List<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectivity)>();
            public IReadOnlyCollection<(int x, int y)> NumbersInLogic = new List<(int x, int y)>();
            public EfficientMineRegionPermutation MineRegionPermutation;
            public MineRegionPermutationNode(EfficientMineRegionPermutation mineRegionPermutation, IEnumerable<(int x, int y)> numbersInLogic)
            {
                MineRegionPermutation = mineRegionPermutation;
                NumbersInLogic = numbersInLogic.ToList();
            }
            public MineRegionPermutationNode(EfficientMineRegionPermutation mineRegionPermutation, IEnumerable<(int x, int y)> numbersInLogic, IEnumerable<((int x, int y) pos, IReadOnlyList<(int x, int y)> connectedSquares)> connectedNodes)
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
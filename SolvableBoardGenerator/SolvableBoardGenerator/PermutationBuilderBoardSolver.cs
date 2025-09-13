namespace Minesweeper
{
    public class PermutationBuilderBoardSolver : BaseBoardSolver
    {
        public PermutationBuilderBoardSolver(Board board, bool verboseLogging = false) : base(board, verboseLogging) { }
        public override IBoardSolver Construct(Board board, bool verboseLogging = false) => new PermutationBuilderBoardSolver(board, verboseLogging);
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
            var unusedNumbers = ActiveNumbers.ToHashSet();
            var usedNumbers = new HashSet<(int x, int y)>();
            uint separateLogic = 0;
            while (unusedNumbers.Any())
            {
                separateLogic++;
                uint numbersInCurrentLogic = 0;
                var possibleContinuations = new HashSet<(int x, int y)>();
                possibleContinuations.Add(unusedNumbers.First());
                MineRegionPermutation? CurrentPermutationGroup = null;
                while (possibleContinuations.Any())
                {
                    var continuation = possibleContinuations.First();
                    possibleContinuations.Remove(continuation);
                    var region = MineRegionPermutationFromNumber(continuation);
                    usedNumbers.Add(continuation);
                    unusedNumbers.Remove(continuation);
                    numbersInCurrentLogic++;
                    var connectedNumbers = ConnectedNumbers(continuation).Where(i => unusedNumbers.Contains(i) && !usedNumbers.Contains(i)).ToList();
                    foreach (var cNumber in connectedNumbers)
                    {
                        possibleContinuations.Add(cNumber);
                    }
                    if (CurrentPermutationGroup == null)
                    {
                        CurrentPermutationGroup = region;
                    }
                    else
                    {
                        CurrentPermutationGroup = CurrentPermutationGroup.Intersection(region);
                    }
                    bool foundNewInfo = false;
                    foreach (var info in CurrentPermutationGroup.GetInformation())
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
                            Console.WriteLine($"Current logic had new information, breaking early. Had a total of {numbersInCurrentLogic} numbers in logic.");
                            PrintCurrentStateBoard();
                        }
                        return true;
                    }
                }
                if (VerboseLogging)
                {
                    Console.WriteLine($"Finnished a total of {separateLogic} separable logics, current one finished had a total of {numbersInCurrentLogic} numbers");
                }
            }
            return false;
        }
        private MineRegionPermutation MineRegionPermutationFromNumber((int x, int y) pos)
        {
            if (DiscoveredNumbers[pos.x, pos.y] == UndiscoveredNumber)
            {
                throw new Exception($"Called {nameof(MineRegionPermutationFromNumber)} in {nameof(PermutationBuilderBoardSolver)} with a unopened square, which isn't allowed.");
            }
            var totalNeighbors = Board.GetNeighbors(pos).ToList();
            uint mines = (uint)DiscoveredNumbers[pos.x, pos.y] - (uint)totalNeighbors.Count(i => IsSetMine(i));
            var neighbors = totalNeighbors.Where(i => !IsSetMine(i) && !IsOpenedSquare(i)).ToList();
            uint neighborsCount = (uint)neighbors.Count;
            var boolPermuts = Combinatorics.GetCombinationsIterative(mines, neighborsCount);
            var permuts = boolPermuts.Select(i => neighbors.Zip(i).ToList());
            return new MineRegionPermutation(permuts, VerboseLogging);

        }
    }
}
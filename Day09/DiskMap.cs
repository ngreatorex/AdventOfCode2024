namespace Day9;

public record DiskBlock(bool IsBlank, int FileID)
{
    public override string ToString()
    {
        return IsBlank ? "." : (FileID > 9 ? "X" : FileID.ToString());
    }

    public long ChecksumComponent => IsBlank ? 0 : FileID;
}

public class DiskMap
{
    private readonly List<DiskBlock> diskMap = [];

    public DiskMap(string input)
    {
        ParseDiskMap(input);
    }

    private void ParseDiskMap(string input)
    {
        var isFile = true;
        var currentFileId = 0;

        for (var i = 0; i < input.Length; i++)
        {
            var numberOfBlocks = ConvertChar(input[i]);
            for (var j = 0; j < numberOfBlocks; j++)
            {
                diskMap.Add(new DiskBlock(!isFile, currentFileId));
            }

            if (isFile)
                currentFileId++;
            isFile = !isFile;
        }
    }

    private int ConvertChar(char input)
    {
        return input switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            _ => throw new ArgumentOutOfRangeException(nameof(input)),
        };
    }

    public override string ToString()
    {
        return string.Join(string.Empty, diskMap.Take(30).Select(x => x.ToString()));
    }

    public void Fragment()
    {
        var i = 0; var j = diskMap.Count - 1;

        while (i < j)
        {
            if (!diskMap[i].IsBlank)
            {
                i++;
                continue;
            }
            if (diskMap[j].IsBlank)
            {
                j--;
                continue;
            }

            SwapBlocks(i, j);
        }
    }

    private void SwapBlocks(int i, int j)
    {
        var movedBlock1 = diskMap[j];
        var movedBlock2 = diskMap[i];
        diskMap.RemoveAt(j);
        diskMap.RemoveAt(i);
        diskMap.Insert(i, movedBlock1);
        diskMap.Insert(j, movedBlock2);
    }

    public void Defragment()
    {
        var firstFreeSpace = 0;
        var endPointer = diskMap.Count - 1;
        var currentFileID = int.MaxValue;
        var currentFileLength = 0;
        var isInFile = false;

        while (endPointer > firstFreeSpace)
        {
            if (!diskMap[firstFreeSpace].IsBlank)
            {
                firstFreeSpace++;
                continue;
            }

            if (isInFile)
            {
                if (!diskMap[endPointer].IsBlank)
                {
                    if (diskMap[endPointer].FileID == currentFileID)
                    {
                        endPointer--;
                        currentFileLength++;
                        continue;
                    }

                    isInFile = false;
                    currentFileLength--;
                    endPointer++;
                }
                else
                {
                    isInFile = false;
                    currentFileLength--;
                    endPointer++;
                }

                var freeSpacePointer = firstFreeSpace;
                var startOfCurrentFreeSpace = firstFreeSpace;
                var copiedFile = false;

                while (freeSpacePointer < endPointer)
                {
                    startOfCurrentFreeSpace = freeSpacePointer;

                    while (diskMap[freeSpacePointer].IsBlank)
                        freeSpacePointer++;

                    if (freeSpacePointer - startOfCurrentFreeSpace >= currentFileLength)
                    {
                        var j = endPointer;
                        for (var i = 0; i < currentFileLength; i++)
                        {
                            SwapBlocks(startOfCurrentFreeSpace + i, j);
                            j++;
                        }

                        copiedFile = true;
                        break;
                    }

                    while (freeSpacePointer < endPointer && !diskMap[freeSpacePointer].IsBlank)
                        freeSpacePointer++;
                }

                if (copiedFile)
                {
                    continue;
                }

                endPointer--;
            }
            else
            {
                if (!diskMap[endPointer].IsBlank)
                {
                    currentFileID = diskMap[endPointer].FileID;
                    currentFileLength = 1;
                    isInFile = true;
                    continue;
                }
                else
                {
                    endPointer--;
                    continue;
                }
            }
        }
    }

    public long Checksum => diskMap.Select((b, index) => b.ChecksumComponent *  index).Sum();
}
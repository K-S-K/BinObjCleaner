using System.Data;

namespace BinObjCleaner
{
    public class DirectoryTree
    {
        public readonly string Name;
        public readonly string Path;

        public readonly bool ContainsObjOrBin;
        public readonly bool HasObjOrBinDirectly;

        public bool IsObjOrBin =>
            Name.Equals("bin", comparisonType: StringComparison.OrdinalIgnoreCase) ||
            Name.Equals("obj", comparisonType: StringComparison.OrdinalIgnoreCase) ||
            Name.Equals("_obj", comparisonType: StringComparison.OrdinalIgnoreCase);

        public bool ToSkip => IsObjOrBin ||
            Name.Equals(".vs", comparisonType: StringComparison.OrdinalIgnoreCase) ||
            Name.Equals(".git", comparisonType: StringComparison.OrdinalIgnoreCase) ||
            Name.Equals(".nuget", comparisonType: StringComparison.OrdinalIgnoreCase) ||
            Name.Equals("packages", comparisonType: StringComparison.OrdinalIgnoreCase);

        public bool NeedNoAction =>
            (!IsObjOrBin && ToSkip)
            && !HasObjOrBinDirectly
            && !ContainsObjOrBin
            && !IsObjOrBin;

        public List<DirectoryTree> Branches { get; private set; }

        public override string ToString() => $"{Name}, [{Branches.Count}]{(IsObjOrBin ? " DEL!" : HasObjOrBinDirectly ? " Has!" : "")}";

        public void Grow()
        {
            if (ToSkip)
            {
                return;
            }

            var subdirs = Directory.GetDirectories(Path).OrderBy(x => x);
            foreach (var dir in subdirs)
            {
                DirectoryTree tree = new DirectoryTree(dir);
                tree.Grow();
                Branches.Add(tree);
            }
        }

        public IEnumerable<string> DirsToDel
        {
            get
            {
                List<string> dirs = new();
                dirs.AddRange(Branches.Where(x => x.IsObjOrBin).Select(x => x.Path));

                foreach (var tree in Branches)
                {
                    dirs.AddRange(tree.DirsToDel);
                }

                return dirs.Distinct();
            }
        }

        public DirectoryTree(string path)
        {
            Branches = new List<DirectoryTree>();
            Path = path ?? throw new ArgumentNullException(path);

            Name = Path.Substring(Path.LastIndexOf('\\')).TrimStart('\\');
            Grow();

            HasObjOrBinDirectly = Branches.Where(x => x.IsObjOrBin).Any();

            ContainsObjOrBin = Branches.Where(x => x.IsObjOrBin || x.ContainsObjOrBin).Any();
        }
    }
}

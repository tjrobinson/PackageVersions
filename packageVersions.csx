
    using System;
    using NuGet;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class PackageVersions
    {
        public static void Get()
        {
            string path = @"C:\Code";
            string[] files = Directory.GetFiles(path, "packages.config", SearchOption.AllDirectories);

            const string format = "{0},{1},{2}";
            StringBuilder builder = new StringBuilder();
            List<SearchResults> results = new List<SearchResults>();

            foreach (var fileName in files)
            {
                var file = new PackageReferenceFile(fileName);
                foreach (PackageReference packageReference in file.GetPackageReferences())
                {
                    SearchResults currentResult = results.FirstOrDefault(x => x.PackageId == packageReference.Id);
                    if (currentResult == null)
                    {
                        currentResult = new SearchResults
                        {
                            PackageId = packageReference.Id
                        };
                        results.Add(currentResult);
                    }

                    SearchResults.SearchVersion currentVersion = currentResult.Versions.FirstOrDefault(x => x.PackageVersion == packageReference.Version.ToString());
                    if (currentVersion == null)
                    {
                        currentVersion = new SearchResults.SearchVersion
                        {
                            PackageVersion = packageReference.Version.ToString()
                        };
                        currentResult.Versions.Add(currentVersion);
                    }

                    currentVersion.Path.Add(fileName);
                }
            }

            results.ForEach(result =>
            {
                if (result.Versions.Count > 1) // multiple versions of same package
                {
                    result.Versions.ForEach(version =>
                    {
                        version.Path.ForEach(packagePath =>
                        {
                            builder.AppendFormat(format, result.PackageId, packagePath, version.PackageVersion);
                            builder.AppendLine();
                        });
                    });
                }
            });
            File.WriteAllText(Path.Combine(path, "packages-duplicates.csv"), builder.ToString());
        }

    }

    public class SearchResults
    {
        public string PackageId { get; set; }
        public List<SearchVersion> Versions { get; set; }
        public SearchResults()
        {
            Versions = new List<SearchResults.SearchVersion>();
        }

        public class SearchVersion
        {
            public string PackageVersion { get; set; }
            public List<string> Path { get; set; }
            public SearchVersion()
            {
                Path = new List<string>();
            }
        }
    }

    PackageVersions.Get();
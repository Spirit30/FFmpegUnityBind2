using System.Collections.Generic;

namespace GigaFileBrowser.Internal
{
    static class FileExtensionsUtil
    {
        public static List<string> Permute(List<string> fileExtensions)
        {
            List<string> fileExtensionsAllCases = new List<string>();

            foreach (string fileExtension in fileExtensions)
            {
                fileExtensionsAllCases.AddRange(Permute(fileExtension.ToLower()));
            }

            return fileExtensionsAllCases;
        }

        static List<string> Permute(string extension)
        {
            List<string> listPermutations = new List<string>();

            char[] array = extension.ToLower().ToCharArray();
            int iterations = (1 << array.Length) - 1;

            for (int i = 0; i <= iterations; i++)
            {
                for (int j = 0; j < array.Length; j++)
                    array[j] = (i & (1 << j)) != 0
                                  ? char.ToUpper(array[j])
                                  : char.ToLower(array[j]);
                listPermutations.Add(new string(array));
            }
            return listPermutations;
        }
    }
}
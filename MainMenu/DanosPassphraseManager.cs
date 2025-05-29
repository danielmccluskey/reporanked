using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public static class DanosPassphraseManager
    {
        private static string _passphrase;
        private static readonly string FolderPath = Path.Combine(Application.persistentDataPath, "reporanked");
        private static readonly string FilePath = Path.Combine(FolderPath, "passphrase.txt");

        public static string GetPassphrase()
        {
            if (File.Exists(FilePath))
                _passphrase = File.ReadAllText(FilePath);


            //lets set a default passphrase if none is set by using SetPassphrase
            if (string.IsNullOrEmpty(_passphrase))
            {
                _passphrase = "default";
                SetPassphrase(_passphrase);
            }
            else
            {
                //We are going to set the passphrase to ensure that the length is 7 characters
                SetPassphrase(_passphrase);
            }

            return _passphrase ?? string.Empty;
        }


        public static void SetPassphrase(string passphrase)
        {
                passphrase = passphrase.Substring(0, Math.Min(passphrase.Length, 7));

            // Ensure the passphrase contains only letters and numbers
            if (!System.Text.RegularExpressions.Regex.IsMatch(passphrase, @"^[a-zA-Z0-9]+$"))
            {
                //remove all non-alphanumeric characters
                passphrase = System.Text.RegularExpressions.Regex.Replace(passphrase, @"[^a-zA-Z0-9]", "");
            }


            _passphrase = passphrase;

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            File.WriteAllText(FilePath, passphrase);
        }
    }
}
